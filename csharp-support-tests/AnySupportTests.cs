using System.Linq;
using Com.Example.Shoppingcart;
using Google.Protobuf;
using Google.Protobuf.Reflection;
using Google.Protobuf.WellKnownTypes;
using io.cloudstate.csharpsupport;
using io.cloudstate.csharpsupport.impl;
using Xunit;

namespace csharp_support_tests
{
    public class AnySupportTests
    {
        AnySupport AnySupport { get; } = new AnySupport(new[] {
                Com.Example.Shoppingcart.Persistence.DomainReflection.Descriptor,
                Cloudstate.Eventsourced.EventSourcedReflection.Descriptor
            }.ToArray(),
            "com.example"
        );

        AddLineItem AddLineItem { get; } = new Com.Example.Shoppingcart.AddLineItem()
        {
            Name = "item",
            ProductId = "id",
            Quantity = 10
        };

        [Fact]
        public void SupportSerializingAndDeserializingProtobufs()
        {
            var any = AnySupport.Encode(AddLineItem);
            // TODO: not sure why the prefixes are not consistent.
            Assert.Equal("type.googleapis.com/" + Com.Example.Shoppingcart.AddLineItem.Descriptor.FullName, any.TypeUrl);
            Assert.Equal(AddLineItem, AnySupport.Decode(any));
        }

        [Fact]
        public void ResolveServiceDescriptor()
        {
            var methods = AnySupport.ResolveServiceDescriptor(
                ShoppingcartReflection.Descriptor.FindTypeByName<ServiceDescriptor>("ShoppingCart")
            );

            Assert.Equal(3, methods.Count);

            var method = methods["AddItem"].Method;

            Assert.Equal(Com.Example.Shoppingcart.AddLineItem.Descriptor.FullName, method.InputType.FullName);
            Assert.Equal(typeof(Com.Example.Shoppingcart.AddLineItem), method.InputType.ClrType);
            var bytes = AddLineItem.ToByteString();
            Assert.Equal(AddLineItem, method.InputType.Parser.ParseFrom(bytes));

            Assert.Equal(Empty.Descriptor.FullName, method.OutputType.FullName);
            Assert.Equal(typeof(Empty), method.OutputType.ClrType);
            var defaultEmpty = new Empty();
            bytes = defaultEmpty.ToByteString();
            Assert.Equal(defaultEmpty, method.OutputType.Parser.ParseFrom(bytes));

        }

        [Theory]
        [InlineData("string", "foo", "")]
        [InlineData("int32", 10, 0)]
        [InlineData("int64", 10L, 0L)]
        [InlineData("float", 10.5f, 0f)]
        [InlineData("double", 10.5, 0)]
        [InlineData("bool", true, false)]
        public void TestPrimitive(string name, object value, object defaultValue)
        {

            var any = AnySupport.Encode(value);
            var res = AnySupport.Decode(any);
            Assert.Equal(value, res);

            any = AnySupport.Encode(null);
            res = AnySupport.Decode(any);
            Assert.Equal(defaultValue, res);
        }

        [Fact]
        public void TestPrimitiveBytes()
        {

            var str = "foo";
            var bytes = ByteString.CopyFromUtf8(str);
            var any = AnySupport.Encode(bytes);
            var res = AnySupport.Decode(any) as ByteString;
            Assert.NotNull(res);
            Assert.Equal(bytes, res);

            var val = res.ToStringUtf8();
            Assert.Equal(str, val);

        }

        [Fact]
        public void TestPrimitiveBytesDefaultValue()
        {

            var str = "";
            var bytes = ByteString.CopyFromUtf8(str);
            var any = AnySupport.Encode(ByteString.Empty);
            var res = AnySupport.Decode(any) as ByteString;
            Assert.NotNull(res);
            Assert.Equal(bytes, res);

            var val = res.ToStringUtf8();
            Assert.Equal(str, val);

        }


        /*
        "support se/deserializing json" in {
      val myJsonable = new MyJsonable
      myJsonable.field = "foo"
      val any = anySupport.encodeScala(myJsonable)
      any.typeUrl should ===(AnySupport.CloudStateJson + classOf[MyJsonable].getName)
      anySupport.decode(any).asInstanceOf[MyJsonable].field should ===("foo")
    }

    @Jsonable
class MyJsonable {
  @BeanProperty var field: String = _
}


     */

    }
}
