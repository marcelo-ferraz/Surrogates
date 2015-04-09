using NUnit.Framework;
using Surrogates.Tactics;
using Surrogates.Tests.Expressions.Entities;
using System;

namespace Surrogates.Tests.Strategies.Methods.Disable
{
    [TestFixture]
    public class DisableTest : StrategiesTests<Strategy.ForMethods>
    {
        [Test]
        public void Void()
        {     
            Strategy.Kind = InterferenceKind.Disable;
            this.Strategy.Methods.Add(((Action)new Dummy().SetPropText_simple).Method);

            var dummy =
                new Dummy();

            var proxy = (Dummy)
                Activator.CreateInstance(Strategies.Apply().Type);

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
                Activator.CreateInstance(Strategies.Apply().Type);

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
