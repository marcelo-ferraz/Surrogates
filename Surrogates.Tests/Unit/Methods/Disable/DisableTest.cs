using NUnit.Framework;
using Surrogates.Tactics;
using Surrogates.Tests.Scenarios.Entities;
using Surrogates.Tests.Unit;
using System;

namespace Surrogates.Tests.Unit.Methods.Disable
{
    [TestFixture]
    public class DisableTest : UnitTests<Strategy.ForMethods>
    {
        [Test]
        public void Void()
        {     
            Strategy.Kind = InterferenceKind.Disable;
            this.Strategy.Methods.Add(((Action)new Dummy().SetPropText_simple).Method);

            var dummy =
                new Dummy();

            var proxy = (Dummy)
                Activator.CreateInstance(Strategies.Apply());

            dummy.SetPropText_simple();
            proxy.SetPropText_simple();

            Assert.AreEqual("simple", dummy.Text);
            Assert.IsNullOrEmpty(proxy.Text);
        }
        [Test]
        public void WithReturn()
        {
            Strategy.Kind = InterferenceKind.Disable;
            this.Strategy.Methods.Add(((Func<int>)new Dummy().Call_SetPropText_simple_Return_1).Method);

            var dummy =
                new Dummy();

            var proxy = (Dummy)
                Activator.CreateInstance(Strategies.Apply());

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
