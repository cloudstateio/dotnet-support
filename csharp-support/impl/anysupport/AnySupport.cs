using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using Google.Protobuf;
using Google.Protobuf.Reflection;
using Google.Protobuf.WellKnownTypes;
using Microsoft.Extensions.Logging;
using Optional;
using Type = System.Type;

namespace io.cloudstate.csharpsupport.impl
{
    public class AnySupport
    {

        public static readonly uint CloudStatePrimitiveFieldNumber = 1;
        public static readonly string CloudStatePrimitive = "p.cloudstate.io/";
        public static readonly string CloudStateJson = "json.cloudstate.io/";
        public static readonly string DefaultTypeUrlPrefix = "type.googleapis.com";

        FileDescriptor[] AdditionalDescriptors { get; }
        string TypeUrlPrefix { get; }

        ImmutableDictionary<string, FileDescriptor> AllDescriptors { get; }
        Dictionary<string, MessageDescriptor> AllTypes { get; }
        ConcurrentDictionary<String, IResolvedType> ReflectionCache { get; }

        public IDictionary<System.Type, IPrimitive> ClassToPrimitives { get; }
        public IDictionary<String, IPrimitive> TypeUrlToPrimitives { get; }

        public AnySupport(IEnumerable<FileDescriptor> additionalDescriptors,
            String? typeUrlPrefix = null)
        {
            ClassToPrimitives = new Dictionary<System.Type, IPrimitive>() {
                { typeof(string), new StringPrimitive() },
                { typeof(Int32), new Int32Primitive() },
                { typeof(Int64), new Int64Primitive() },
                { typeof(float), new FloatPrimitive() },
                { typeof(double), new DoublePrimitive() },
                { typeof(ByteString), new BytesPrimitive() },
                { typeof(bool), new BoolPrimitive() }
            };
            TypeUrlToPrimitives = ClassToPrimitives
                .ToDictionary(
                    x => x.Value.FullName,
                    x => x.Value
                );

            AdditionalDescriptors = additionalDescriptors.ToArray();
            TypeUrlPrefix = AnySupport.DefaultTypeUrlPrefix;
            AllDescriptors = FlattenDescriptors(AdditionalDescriptors);

            AllTypes = AllDescriptors.Values.SelectMany(
                    descriptor => descriptor.MessageTypes.Select(
                        messageType => KeyValuePair.Create(messageType.FullName, messageType)
                    )
                ).ToDictionary(x => x.Key, x => x.Value);

            ReflectionCache = new ConcurrentDictionary<String, IResolvedType>();
        }

        public Dictionary<string, IResolvedServiceMethod> ResolveServiceDescriptor(ServiceDescriptor descriptor)
        {
            return descriptor.Methods.Select(method =>
            {
                return (IResolvedServiceMethod)typeof(ResolvedServiceMethod<,>).MakeGenericType(
                        method.InputType.ClrType,
                        method.OutputType.ClrType
                    ).GetConstructor(
                        new[] {
                            method.GetType(),
                            typeof(IResolvedType<>).MakeGenericType(method.InputType.ClrType),
                            typeof(IResolvedType<>).MakeGenericType(method.OutputType.ClrType)
                        }
                    ).Invoke(
                        new object[] {
                            method,
                            this.GetType()
                                .GetMethod("ResolveTypeDescriptor")
                                .MakeGenericMethod(method.InputType.ClrType)
                                .Invoke(this, new [] { method.InputType }),
                            this.GetType()
                                .GetMethod("ResolveTypeDescriptor")
                                .MakeGenericMethod(method.OutputType.ClrType)
                                .Invoke(this, new [] { method.OutputType })
                        }
                    );
            }).ToDictionary(x => x.Name, x => x);
        }

        public Option<IResolvedType<TInput>> TryResolveCSharpPbType<TInput>(MessageDescriptor typeDescriptor)
            where TInput : IMessage
        {
            var fileDescriptor = typeDescriptor.File;
            return new CSharpResolvedType<TInput>(
                typeDescriptor.FullName,
                typeDescriptor.Parser
            ).Some<IResolvedType<TInput>>();

            //     val options = fileDescriptor.getOptions
            //     // Firstly, determine the java package
            //     val packageName =
            //       if (options.hasJavaPackage) options.getJavaPackage + "."
            //       else if (fileDescriptor.getPackage.nonEmpty) fileDescriptor.getPackage + "."
            //       else ""

            //     val outerClassName =
            //       if (options.hasJavaMultipleFiles && options.getJavaMultipleFiles) ""
            //       else if (options.hasJavaOuterClassname) options.getJavaOuterClassname + "$"
            //       else if (fileDescriptor.getName.nonEmpty)
            //         CaseFormat.LOWER_UNDERSCORE.to(CaseFormat.UPPER_CAMEL, strippedFileName(fileDescriptor.getName)) + "$"
            //       else ""

            //     val className = packageName + outerClassName + typeDescriptor.getName
            //     try {
            //       log.debug("Attempting to load com.google.protobuf.Message class {}", className)
            //       val clazz = classLoader.loadClass(className)
            //       if (classOf[com.google.protobuf.Message].isAssignableFrom(clazz)) {
            //         val parser = clazz.getMethod("parser").invoke(null).asInstanceOf[Parser[com.google.protobuf.Message]]
            //         Some(
            //           new JavaPbResolvedType(clazz.asInstanceOf[Class[com.google.protobuf.Message]],
            //                                  typeUrlPrefix + "/" + typeDescriptor.getFullName,
            //                                  parser)
            //         )
            //       } else {
            //         None
            //       }
            //     } catch {
            //       case cnfe: ClassNotFoundException =>
            //         log.debug("Failed to load class", cnfe)
            //         None
            //       case nsme: NoSuchElementException =>
            //         throw SerializationException(
            //           s"Found com.google.protobuf.Message class $className to deserialize protobuf ${typeDescriptor.getFullName} but it didn't have a static parser() method on it.",
            //           nsme
            //         )
            //       case iae @ (_: IllegalAccessException | _: IllegalArgumentException) =>
            //         throw SerializationException(s"Could not invoke $className.parser()", iae)
            //       case cce: ClassCastException =>
            //         throw SerializationException(s"$className.parser() did not return a ${classOf[Parser[_]]}", cce)
            //     }
            //   }
        }

        public IResolvedType<TInput> ResolveTypeDescriptor<TInput>(MessageDescriptor typeDescriptor)
            where TInput : IMessage
        {
            return (IResolvedType<TInput>)ReflectionCache.GetOrAdd(
                typeDescriptor.FullName,
                name => TryResolveCSharpPbType<TInput>(typeDescriptor)
                    .Match(
                        some: x => x,
                        none: () => throw new Exception(
                            $"Could not resolve instance of type {typeDescriptor.FullName}"
                        )
                    )
                );
        }

        public ByteString PrimitiveToBytes<T>(IPrimitive primitive, T value)
        {

            var typedPrimitive = (Primitive<T>)primitive;
            if (!Equals(typedPrimitive.DefaultValue, value))
            {
                using (MemoryStream stream = new MemoryStream())
                {
                    CodedOutputStream s = new CodedOutputStream(stream);
                    s.WriteTag(typedPrimitive.Tag);
                    typedPrimitive.Write(s, value);
                    s.Flush();
                    stream.Position = 0;
                    return ByteString.FromStream(stream);
                }
            }
            else
            {
                return ByteString.Empty;
            }
        }

        public T BytesToPrimitive<T>(IPrimitive primitive, ByteString value)
        {
            var typedPrimitive = (Primitive<T>)primitive;
            using (var stream = value.CreateCodedInput())
            {
                bool hasValue = false;
                uint tag = 0;
                while ((tag = stream.ReadTag()) != 0)
                {
                    if (tag == primitive.Tag)
                    {
                        hasValue = true;
                        break;
                    }
                    stream.SkipLastField();
                }
                return hasValue ? typedPrimitive.Read(stream) : typedPrimitive.DefaultValue;
            }
        }

        public Any Encode(object value)
        {
            switch (value)
            {
                case Any any: return any;
                case IMessage message: return Any.Pack(message);
                case ByteString byteString:
                    return new Any()
                    {
                        TypeUrl = new BytesPrimitive().FullName, // HACK: Shouldn't need to instantiate this every time..
                        Value = PrimitiveToBytes(new BytesPrimitive(), byteString)
                    };
                case object _ when ClassToPrimitives.ContainsKey(value.GetType()):
                    var primitive = ClassToPrimitives[value.GetType()];
                    var bytestring = this.GetType().GetMethod("PrimitiveToBytes")
                        ?.MakeGenericMethod(value.GetType())
                        .Invoke(this, new[] { primitive, value }) as ByteString
                            ?? throw new Exception("Couldn't cast to typed primitive");
                    return new Any()
                    {
                        Value = bytestring,
                        TypeUrl = primitive.FullName
                    };

                //   case _: AnyRef if value.getClass.getAnnotation(classOf[Jsonable]) != null =>
                //     val json = UnsafeByteOperations.unsafeWrap(objectMapper.writeValueAsBytes(value))
                //     ScalaPbAny(CloudStateJson + value.getClass.getName, primitiveToBytes(BytesPrimitive, json))

                default:
                    throw new Exception($"Don't know how to serialize object of type {value.GetType()}. Try passing a protobuf, using a primitive type, or using a type annotated with @Jsonable.");

            }
        }

        public Object Decode(Any any)
        {

            var typeUrl = any.TypeUrl;
            var bytes = any.Value;

            if (typeUrl.StartsWith(AnySupport.CloudStatePrimitive))
            {
                return TypeUrlToPrimitives.FirstOrDefault(x => x.Key == typeUrl).Some().Match(
                    some: primitive => this.GetType().GetMethod("BytesToPrimitive")
                            ?.MakeGenericMethod(primitive.Value.ClassType)
                            .Invoke(this, new object[] { primitive.Value, bytes })
                                ?? throw new Exception("Couldn't cast to primitive type"),
                    none: () => throw new Exception($"Unknown primitive type url: {typeUrl}")
                );
            }
            else if (typeUrl.StartsWith(AnySupport.CloudStateJson))
            {
                throw new NotImplementedException();
            }
            else
            {
                ILogger<AnySupport> Logger = new LoggerFactory().CreateLogger<AnySupport>();

                var typeName = typeUrl.Split('/', 2).Some().Match(
                    some: split =>
                    {
                        if (split[0] != TypeUrlPrefix)
                        {
                            Logger.LogWarning($"Message type [{typeUrl}] does not match configured type url prefix [{TypeUrlPrefix}]");
                        }
                        return split[1];
                    },
                    none: () =>
                    {
                        Logger.LogWarning($"Message type [{typeUrl}] does not have a url prefix, it should have one that matchers the configured type url prefix [{TypeUrlPrefix}]");
                        return typeUrl;
                    }
                );

                var resolvedType = ResolveTypeUrl(typeName);
                return resolvedType.Parser.ParseFrom(bytes);

            }

        }

        private IResolvedType ResolveTypeUrl(string typeName)
        {
            var messageDescriptor = AllTypes[typeName];
            return this.GetType()
                .GetMethod("ResolveTypeDescriptor")
                ?.MakeGenericMethod(messageDescriptor.ClrType)
                .Invoke(this, new object[] { messageDescriptor }) as IResolvedType
                    ?? throw new Exception("Reflection failed");
        }

        /// <summary>
        /// Flatten all descriptors in the given list
        /// </summary>
        /// <param name="descriptors"></param>
        /// <returns></returns>
        public ImmutableDictionary<string, FileDescriptor> FlattenDescriptors(
            IEnumerable<FileDescriptor> descriptors)
        {
            return FlattenDescriptors(
                ImmutableDictionary<String, FileDescriptor>.Empty,
                descriptors
            );
        }


        private ImmutableDictionary<string, FileDescriptor> FlattenDescriptors(
            ImmutableDictionary<String, FileDescriptor> seen,
            IEnumerable<FileDescriptor> descriptors)
        {

            return descriptors.Aggregate(seen, (results, descriptor) =>
            {

                var descriptorName = descriptor.Name;
                if (results.ContainsKey(descriptorName))
                {
                    return results;
                }
                else
                {
                    var withDesc = results.Add(descriptorName, descriptor);
                    return FlattenDescriptors(
                        withDesc,
                        descriptor.Dependencies.Concat(descriptor.PublicDependencies)
                    );
                }

            });
        }

    }

}