using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

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
                .Throughout<SimpleModel>()
                .Replace
                .ThisProperty(d => d.Id)
                .Accessors(a => a
                    .Getter.Using<IdLazyLoader>("idLoader").ThisMethod<string, int>(l => l.Load)
                    .And
                    .Setter.Using<IdLazyLoader>("idLoader").ThisMethod<int>(l => l.MarkAsDirty))
                .And
                .Replace
                .ThisProperty(d => d.OutterId)
                .Accessors(a => a
                    .Getter.Using<IdLazyLoader>("idLoader").ThisMethod<string, int>(l => l.Load)
                    .And
                    .Setter.Using<IdLazyLoader>("idLoader").ThisMethod<int>(l => l.MarkAsDirty))                    
                ).Save();
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

    public class SimpleModelProxy2 : SimpleModel
    {
        private IdLazyLoader _id_loader_0;

        private int _id;

        public override int Id
        {
            get
            {
                return this._id_loader_0.Load("Id");
            }
            set
            {
                this._id_loader_0.MarkAsDirty(value);
            }
        }

        public SimpleModelProxy2()
        {
            this._id_loader_0 = new IdLazyLoader();
        }
    }
}
