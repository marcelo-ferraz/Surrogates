using NUnit.Framework;
using Surrogates.Tests.Simple.Entities;

namespace Surrogates.Tests.Simple.Methods.Disable
{
    [TestFixture]
    public class DisableTest
    {
        [Test]
        public void Void()
        {
            var container = new SurrogatesContainer();

            container.Map(m => m
                .Throughout<Dummy>()
                .Disable
                .ThisMethod(d => d.SetPropText_simple));

            var dummy =
                new Dummy();

            var proxy =
                container.Invoke<Dummy>();

            dummy.SetPropText_simple();
            proxy.SetPropText_simple();

            Assert.AreEqual("simple", dummy.Text);
            Assert.IsNullOrEmpty(proxy.Text);
        }
        [Test]
        public void WithReturn()
        {
            var container = new SurrogatesContainer();

            container.Map(m => m
                .Throughout<Dummy>()
                .Disable
                .ThisMethod<int>(d => d.Call_SetPropText_simple_Return_1));

            var dummy =
                new Dummy();

            var proxy =
                container.Invoke<Dummy>();

            var dummyRes =
                dummy.Call_SetPropText_simple_Return_1();
            var proxyRes =
                proxy.Call_SetPropText_simple_Return_1();

            Assert.AreEqual("simple", dummy.Text);
            Assert.IsNullOrEmpty(proxy.Text);
            Assert.AreNotEqual(dummyRes, proxyRes);
            Assert.AreEqual(0, proxyRes);
        }
    }
}
