using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using CloudState.CSharpSupport.Exceptions;
using CloudState.CSharpSupport.Reflection;
using CloudState.CSharpSupport.Reflection.Interfaces;
using CloudState.CSharpSupport.Serialization.Primitives;
using CloudState.CSharpSupport.Serialization.Primitives.Interfaces;
using Google.Protobuf;
using Google.Protobuf.Reflection;
using Google.Protobuf.WellKnownTypes;
using Microsoft.Extensions.Logging;
using Optional;
using FileOptions = System.IO.FileOptions;

namespace CloudState.CSharpSupport.Serialization
{
    internal class AnySupport
    {
        public const uint CloudStatePrimitiveFieldNumber = 1;
        public const string CloudStatePrimitive = "p.cloudstate.io/";
        public const string CloudStateJson = "json.cloudstate.io/";
        public const string DefaultTypeUrlPrefix = "type.googleapis.com";

        private FileDescriptor[] AdditionalDescriptors { get; }
        private string TypeUrlPrefix { get; }

        private ImmutableDictionary<string, FileDescriptor> AllDescriptors { get; }
        private Dictionary<string, MessageDescriptor> AllTypes { get; }
        private ConcurrentDictionary<string, IResolvedType> ReflectionCache { get; }

        private IDictionary<System.Type, IPrimitive> ClassToPrimitives { get; }
        private IDictionary<string, IPrimitive> TypeUrlToPrimitives { get; }

        public AnySupport(IEnumerable<FileDescriptor> additionalDescriptors, string typeUrlPrefix = null)
        {
            TypeUrlPrefix = typeUrlPrefix ?? DefaultTypeUrlPrefix;
            ClassToPrimitives = new Dictionary<System.Type, IPrimitive>
            {
                { typeof(string), new StringPrimitive() },
                { typeof(int), new Int32Primitive() },
                { typeof(long), new Int64Primitive() },
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
            TypeUrlPrefix = DefaultTypeUrlPrefix;
            AllDescriptors = FlattenDescriptors(AdditionalDescriptors);

            AllTypes = AllDescriptors.Values.SelectMany(
                    descriptor => descriptor.MessageTypes.Select(
                        messageType => KeyValuePair.Create(messageType.FullName, messageType)
                    )
                ).ToDictionary(x => x.Key, x => x.Value);

            ReflectionCache = new ConcurrentDictionary<string, IResolvedType>();
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
                        new[] {
                            method,
                            GetType()
                                .GetMethod("ResolveTypeDescriptor")
                                ?.MakeGenericMethod(method.InputType.ClrType)
                                .Invoke(this, new object [] { method.InputType })
                                    ?? throw new ArgumentNullException(
                                        $"Generic reflection failed to access `ResolveTypeDescriptor` on {nameof(AnySupport)}"
                                    ),
                            GetType()
                                .GetMethod("ResolveTypeDescriptor")
                                ?.MakeGenericMethod(method.OutputType.ClrType)
                                .Invoke(this, new object [] { method.OutputType })
                                    ?? throw new ArgumentNullException(
                                        $"Generic reflection failed to access `ResolveTypeDescriptor` on {nameof(AnySupport)}"
                                    )
                        }
                    ) ?? throw new ArgumentNullException();
            }).ToDictionary(x => x.Name, x => x);
        }

        private static Option<IResolvedType<TInput>> TryResolveCSharpPbType<TInput>(MessageDescriptor typeDescriptor)
            where TInput : IMessage
        {
            var fileDescriptor = typeDescriptor.File;

            var csharpNamespace = "";
            string packageName, outerClassName = "", className;
            if (fileDescriptor?.CustomOptions.TryGetString(
                Google.Protobuf.Reflection.FileOptions.CsharpNamespaceFieldNumber,
                out csharpNamespace) == true)
            {
                packageName = csharpNamespace + ".";
            }
            else if (!string.IsNullOrEmpty(fileDescriptor?.Package))
            {
                packageName = fileDescriptor.Package + ".";
            }
            else
            {
                packageName = "";
            }

            // Note: not applicable to csharp
            //     val outerClassName =
            //       if (options.hasJavaMultipleFiles && options.getJavaMultipleFiles) ""
            //       else if (options.hasJavaOuterClassname) options.getJavaOuterClassname + "$"
            //       else if (fileDescriptor.getName.nonEmpty)
            //         CaseFormat.LOWER_UNDERSCORE.to(CaseFormat.UPPER_CAMEL, strippedFileName(fileDescriptor.getName)) + "$"
            //       else ""

            className = packageName + outerClassName + typeDescriptor.Name;

            try
            {
                var instance = Activator.CreateInstance(typeDescriptor.ClrType);
                if (instance is IMessage)
                {
                    return new CSharpResolvedType<TInput>(
                        DefaultTypeUrlPrefix + "/" + typeDescriptor.FullName,
                        typeDescriptor.Parser
                    ).Some<IResolvedType<TInput>>();
                }
                return Optional.Option.None<IResolvedType<TInput>>();
            }
            catch (Exception)
            {
                return Optional.Option.None<IResolvedType<TInput>>();
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
            }

        }

        public IResolvedType<TInput> ResolveTypeDescriptor<TInput>(MessageDescriptor typeDescriptor)
            where TInput : IMessage
        {
            return (IResolvedType<TInput>)ReflectionCache.GetOrAdd(
                typeDescriptor.FullName,
                name => TryResolveCSharpPbType<TInput>(typeDescriptor)
                    .Match(
                        some: x => x,
                        none: () => throw new CloudStateException(
                            $"Could not resolve instance of type {typeDescriptor.FullName}"
                        )
                    )
                );
        }
        
        [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
        [SuppressMessage("ReSharper", "MemberCanBeMadeStatic.Global")]
        public ByteString PrimitiveToBytes<T>(IPrimitive primitive, T value)
        {

            var typedPrimitive = (Primitive<T>)primitive;
            if (!Equals(typedPrimitive.DefaultValue, value))
            {
                using (var stream = new MemoryStream())
                {
                    var s = new CodedOutputStream(stream);
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

        [SuppressMessage("ReSharper", "MemberCanBeMadeStatic.Local")]
        public T BytesToPrimitive<T>(IPrimitive primitive, ByteString value)
        {
            var typedPrimitive = (Primitive<T>)primitive;
            using (var stream = value.CreateCodedInput())
            {
                var hasValue = false;
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
                    return new Any
                    {
                        TypeUrl = new BytesPrimitive().FullName, // HACK: Shouldn't need to instantiate this every time..
                        Value = PrimitiveToBytes(new BytesPrimitive(), byteString)
                    };
                case object _ when ClassToPrimitives.ContainsKey(value.GetType()):
                    var primitive = ClassToPrimitives[value.GetType()];
                    var bytestring = GetType().GetMethod("PrimitiveToBytes")
                        ?.MakeGenericMethod(value.GetType())
                        .Invoke(this, new[] { primitive, value }) as ByteString
                            ?? throw new CloudStateException("Couldn't cast to typed primitive");
                    return new Any
                    {
                        Value = bytestring,
                        TypeUrl = primitive.FullName
                    };

                //   case _: AnyRef if value.getClass.getAnnotation(classOf[Jsonable]) != null =>
                //     val json = UnsafeByteOperations.unsafeWrap(objectMapper.writeValueAsBytes(value))
                //     ScalaPbAny(CloudStateJson + value.getClass.getName, primitiveToBytes(BytesPrimitive, json))

                default:
                    throw new CloudStateException($"Don't know how to serialize object of type {value.GetType()}. Try passing a protobuf, using a primitive type, or using a type annotated with @Jsonable.");

            }
        }

        public object Decode(Any any)
        {

            var typeUrl = any.TypeUrl;
            var bytes = any.Value;

            if (typeUrl.StartsWith(CloudStatePrimitive))
            {
                return TypeUrlToPrimitives.FirstOrDefault(x => x.Key == typeUrl).Some().Match(
                    primitive => GetType().GetMethod("BytesToPrimitive")
                            ?.MakeGenericMethod(primitive.Value.ClassType)
                            .Invoke(this, new object[] { primitive.Value, bytes })
                                ?? throw new CloudStateException("Couldn't cast to primitive type"),
                    () => throw new CloudStateException($"Unknown primitive type url: {typeUrl}")
                );
            }

            if (typeUrl.StartsWith(CloudStateJson))
            {
                throw new NotImplementedException();
            }

            var logger = new LoggerFactory().CreateLogger<AnySupport>();

            var typeName = typeUrl.Split('/', 2).Some().Match(
                some: split =>
                {
                    if (split[0] != TypeUrlPrefix)
                    {
                        logger.LogWarning($"Message type [{typeUrl}] does not match configured type url prefix [{TypeUrlPrefix}]");
                    }
                    return split[1];
                },
                none: () =>
                {
                    logger.LogWarning($"Message type [{typeUrl}] does not have a url prefix, it should have one that matchers the configured type url prefix [{TypeUrlPrefix}]");
                    return typeUrl;
                }
            );

            var resolvedType = ResolveTypeUrl(typeName);
            return resolvedType.ParseFrom(bytes);



        }

        private IResolvedType ResolveTypeUrl(string typeName)
        {
            var messageDescriptor = AllTypes[typeName];
            return GetType()
                .GetMethod("ResolveTypeDescriptor")
                ?.MakeGenericMethod(messageDescriptor.ClrType)
                .Invoke(this, new object[] { messageDescriptor }) as IResolvedType
                    ?? throw new CloudStateException("Reflection failed");
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
                ImmutableDictionary<string, FileDescriptor>.Empty,
                descriptors
            );
        }


        private ImmutableDictionary<string, FileDescriptor> FlattenDescriptors(
            ImmutableDictionary<string, FileDescriptor> seen,
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