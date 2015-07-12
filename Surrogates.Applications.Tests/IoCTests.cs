using System.Collections.Generic;
using NUnit.Framework;

namespace Surrogates.Applications.Tests
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
        public void SimpleInjection()
        {
            Container.Map(m =>
                m.From<Simpleton>()
                .Apply
                .IoCFor(s => s.List)
                .Implying<InjectedList<int>>()).Save();

            var proxy = Container.Invoke<Simpleton>();

            Assert.AreEqual(typeof(InjectedList<int>), proxy.List.GetType()); 
        }


        [Test]
        public void SimpleInjectionWithParams()
        {
            Container.Map(m =>
                m.From<Simpleton>()
                .Apply
                .IoCFor(s => s.List)
                .Implying<InjectedList<int>>(123));

            var proxy = Container.Invoke<Simpleton>();

            Assert.AreEqual(typeof(InjectedList<int>), proxy.List.GetType());

            var injList = 
                proxy.List as InjectedList<int>;

            Assert.AreEqual(123, injList.Value);
        }
    }
}
