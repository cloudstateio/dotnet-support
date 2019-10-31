using CloudState.CSharpSupport.Attributes;
using CloudState.CSharpSupport.Attributes.EventSourced;
using CloudState.CSharpSupport.Exceptions;
using CloudState.CSharpSupport.Interfaces.EventSourced;
using CloudState.CSharpSupport.Interfaces.EventSourced.Contexts;
using CloudState.CSharpSupport.Tests.AttributeBasedEntityFactory.CommandHandler;
using Xunit;

namespace CloudState.CSharpSupport.Tests.AttributeBasedEntityFactory
{
    public class EntityConstructionTests
    {
        [Fact]
        public void ShouldSupportNoArgConstructorTest()
        {
            Create<NoArgConstructorEntity>();
        }

        [Fact]
        public void ShouldSupportEntityIdConstructorTest()
        {
            Create<EntityIdConstructorEntity>();
        }

        [Fact]
        public void ShouldSupportCreationContextArgConstructorTest()
        {
            Create<CreationContextArgConstructorEntity>();
        }

        [Fact]
        public void ShouldSupportMultiArgConstructorTest()
        {
            Create<ShouldSupportMultiArgConstructor>();
        }

        [Fact]
        public void ShouldNotSupportUnsupportedConstructorParameterEntityTest()
        {
            var ex = Assert.Throws<InvalidEntityConstructorParameterException>(() =>
            {
                Create<UnsupportedConstructorParameterEntity>();
            });
            Assert.Equal(typeof(string), ex.ArgumentType);
        }

        [Fact]
        public void ShouldNotSupportUnsupportedMultipleConstructorsParameterEntityTest()
        {
            var ex = Assert.Throws<MultipleEntityConstructorsFoundException>(() =>
            {
                Create<UnsupportedMultipleConstructorsParameterEntity>();
            });
            Assert.Equal(typeof(UnsupportedMultipleConstructorsParameterEntity), ex.EntityType);
        }

        private IEntityHandler Create<T>()
        {
            return AttributeBasedEntityFactoryCommandHandlerTests.CreateHandler<T>();
        }
        
    }

    [EventSourcedEntity]
    public class NoArgConstructorEntity
    {
    }

    [EventSourcedEntity]
    public class EntityIdConstructorEntity
    {
        public EntityIdConstructorEntity([EntityId] string entityId)
        {
            Assert.Equal("foo", entityId);
        }
    }

    [EventSourcedEntity]
    public class CreationContextArgConstructorEntity
    {
        public CreationContextArgConstructorEntity(IEventSourcedContext context)
        {
            Assert.Equal("foo", context.EntityId);
        }
    }

    [EventSourcedEntity]
    public class ShouldSupportMultiArgConstructor
    {
        public ShouldSupportMultiArgConstructor(IEventSourcedContext context, [EntityId] string entityId)
        {
            Assert.Equal("foo", context.EntityId);
            Assert.Equal("foo", entityId);
        }
    }

    [EventSourcedEntity]
    public class UnsupportedMultipleConstructorsParameterEntity
    {
        public UnsupportedMultipleConstructorsParameterEntity(IEventSourcedContext context)
        {
        }

        public UnsupportedMultipleConstructorsParameterEntity([EntityId] string entity)
        {
        }
    }

    [EventSourcedEntity]
    public class UnsupportedConstructorParameterEntity
    {
        public UnsupportedConstructorParameterEntity(string entity)
        {
        }
    }
}