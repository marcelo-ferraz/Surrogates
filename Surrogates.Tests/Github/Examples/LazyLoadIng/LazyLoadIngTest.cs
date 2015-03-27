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

    //public class SimpleModelProxy2 : SimpleModel
    //{
    //    private IdLazyLoader _id_loader_0;

    //    private int _id;

    //    public override int Id
    //    {
    //        get
    //        {
    //            return this._id_loader_0.Load("Id");
    //        }
    //        set
    //        {
    //            this._id_loader_0.MarkAsDirty(value);
    //        }
    //    }

    //    public SimpleModelProxy2()
    //    {
    //        this._id_loader_0 = new IdLazyLoader();
    //    }
    //}
}
