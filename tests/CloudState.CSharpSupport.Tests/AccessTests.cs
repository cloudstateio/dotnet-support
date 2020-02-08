using System;
using System.Linq;
using System.Reflection;
using Xunit;

namespace CloudState.CSharpSupport.Tests
{
    public class AccessTests
    {
        [Fact]
        public void InternalLibraryHasNoPublicClasses()
        {
            AssertTypeFilterReturnsEmpty(typeof(CloudStateWorker), x => x.IsClass);
        }
        
        [Fact]
        public void InternalLibraryHasNoPublicInterfaces()
        {
            AssertTypeFilterReturnsEmpty(typeof(CloudStateWorker), x => x.IsInterface);   
        }
        
        [Fact]
        public void InternalLibraryHasNoPublicEnums()
        {
            AssertTypeFilterReturnsEmpty(typeof(CloudStateWorker), x => x.IsEnum);   
        }

        private static void AssertTypeFilterReturnsEmpty(Type referenceClass, Func<Type, bool> predicate)
        {
            var assembly = referenceClass.Assembly;
            var types = assembly.GetTypes()
                .Where(predicate)
                .Where(x => x.IsVisible);
            Assert.Empty(types);
        }
        
        
        
    }
}