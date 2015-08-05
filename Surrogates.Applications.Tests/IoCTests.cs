using System.Collections.Generic;
using NUnit.Framework;

namespace Surrogates.Aspects.Tests
{
    public class IoCTests : AppTestsBase
    {
        public class InjectedList<T> : List<T>
        {
            public int Value { get; set; }

            public InjectedList()
            { }

            public InjectedList(int value)
            {
                Value = value;
            }
        }

        [Test]
        public void SimpleInjectionTest()
        {
            Container.Map(m =>
                m.From<Simpleton>()
                .Apply
                .IoCFor(s => s.List)
                .Injecting<InjectedList<int>>());

            var proxy = Container.Invoke<Simpleton>();

            Assert.AreEqual(typeof(InjectedList<int>), proxy.List.GetType()); 
        }


        [Test]
        public void SimpleInjectionWithParamsTest()
        {
            Container.Map(m =>
                m.From<Simpleton>()
                .Apply
                .IoCFor(s => s.List)
                .Injecting<InjectedList<int>>(123));

            var proxy = Container.Invoke<Simpleton>();

            Assert.AreEqual(typeof(InjectedList<int>), proxy.List.GetType());

            var injList = 
                proxy.List as InjectedList<int>;

            Assert.AreEqual(123, injList.Value);
        }

        [Test]
        public void DoubleInjectionTest()
        {
            Container.Map(m =>
                m.From<Simpleton>()
                .Apply
                .IoCFor(s => s.PropListVal, s=> s.List).Injecting<InjectedList<int>>(123)
                .And
                .Apply
                .IoCFor(s => s.PropListRef).Injecting<InjectedList<Dummy>>());

            var proxy = Container.Invoke<Simpleton>();

            Assert.AreEqual(typeof(InjectedList<int>), proxy.PropListVal.GetType());

            var injList1 =
                proxy.PropListVal as InjectedList<int>;
            
            Assert.AreEqual(123, injList1.Value);

            Assert.AreEqual(typeof(InjectedList<Dummy>), proxy.PropListRef.GetType());

            Assert.AreEqual(typeof(InjectedList<int>), proxy.List.GetType());

            var injList2 =
                proxy.List as InjectedList<int>;

            Assert.AreEqual(123, injList2.Value);
        }

    }
}
