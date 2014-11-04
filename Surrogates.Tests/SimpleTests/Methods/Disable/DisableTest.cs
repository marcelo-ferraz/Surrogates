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
                .ThisMethod(d => d.Void_ParameterLess));

            var dummy =
                new Dummy();

            var proxy =
                container.Invoke<Dummy>();

            dummy.Void_ParameterLess();
            proxy.Void_ParameterLess();

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
                .ThisMethod<int>(d => d.Int_1_ParameterLess));

            var dummy =
                new Dummy();

            var proxy =
                container.Invoke<Dummy>();

            var dummyRes =
                dummy.Int_1_ParameterLess();
            var proxyRes =
                proxy.Int_1_ParameterLess();

            Assert.AreEqual("simple", dummy.Text);
            Assert.IsNullOrEmpty(proxy.Text);
            Assert.AreNotEqual(dummyRes, proxyRes);
            Assert.AreEqual(0, proxyRes);
        }
    }
}
