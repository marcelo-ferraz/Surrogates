using NUnit.Framework;
using System;

namespace Surrogates.Tests.Github.Examples.LazyLoadIng
{
    [TestFixture]
    public class LazyLoadIngTest
    {
        private SurrogatesContainer _container = new SurrogatesContainer();

        [SetUp]
        public void Map()
        {
            _container.Map(m => m
                .From<SimpleModel>()
                .Replace
                .These(d => d.Id, d => d.OutterId)
                .Accessors(a => a
                    .Getter.Using<IdLazyLoader>("idLoader", l => (Func<string, int>) l.Load)
                    .And
                    .Setter.Using<IdLazyLoader>("idLoader", l => (Action<int>) l.MarkAsDirty))                
                );
        }

        [Test]
        public void Test()
        {
            var model = 
                _container.Invoke<SimpleModel>();
            
            try
            {
                var id = model.Id;
                Assert.Fail();
            }
            catch (NotImplementedException)
            {
                Assert.Pass(); 
            }
        }
    }
}
