using NUnit.Framework;
using Surrogates.Tests.Scenarios.Entities;
using System;

namespace Surrogates.Tests.Scenarios.Methods.Substitute
{
    [TestFixture]
    public class Substitute_FuncWithFuncTest : IInterferenceTest
    {
        [Test]
        public void BothParameterLess()
        {
            var container = new SurrogatesContainer();

            container.Map(m => m
                .From<Dummy>()
                .Replace
                .This(d => (Func<int>)d.Call_SetPropText_simple_Return_1)
                .Using<InterferenceObject>(r => (Func<int>)r.AccomplishNothing_Return2))
                ;

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
            Assert.AreEqual(2, proxyRes);
        }

        [Test]
        public void PassingBaseParameters()
        {
            var container = new SurrogatesContainer();

            container.Map(m =>
                m.From<Dummy>()
                .Replace.This(d => (Func<string, DateTime, Dummy.EvenMore, int>)d.Call_SetPropText_complex_Return_1)
                .Using<InterferenceObject>(r => (Func<string, Dummy, DateTime, string, Dummy.EvenMore, int> ) r.AddToPropText__MethodName_Return2));

            var dummy =
                new Dummy();

            var proxy =
                container.Invoke<Dummy>();

            // just to show that the rest of the object behaves as expected
            dummy.SetPropText_simple();
            proxy.SetPropText_simple();

            Assert.IsNotNullOrEmpty(dummy.Text);
            Assert.AreEqual("simple", dummy.Text);
            Assert.IsNotNullOrEmpty(proxy.Text);
            Assert.AreEqual("simple", proxy.Text);

            //and now, the comparison between the two methods
            var dummyRes =
                dummy.Call_SetPropText_complex_Return_1("this call was not made by the original property", DateTime.Now, new Dummy.EvenMore());

            var proxyRes =
                proxy.Call_SetPropText_complex_Return_1("this call was not made by the original property", DateTime.Now, new Dummy.EvenMore());

            Assert.IsNotNullOrEmpty(dummy.Text);
            Assert.AreEqual("complex", dummy.Text);
            Assert.IsNotNullOrEmpty(proxy.Text);
            Assert.AreEqual("simple, this call was not made by the original property - property: Call_SetPropText_complex_Return_1", proxy.Text);
            Assert.AreNotEqual(dummyRes, proxyRes);
            Assert.AreEqual(2, proxyRes);
        }

        [Test, ExpectedException(typeof(NullReferenceException))]
        public void NotPassingBaseParameters()
        {
            var container = new SurrogatesContainer();

            container.Map(m =>
                m.From<Dummy>()
                .Replace
                .Method("Call_SetPropText_complex_Return_1")
                .Using<InterferenceObject>(r => 
                    (Func<string, Dummy, DateTime, string, Dummy.EvenMore, int>)r.DontAddToPropText__MethodName_Return2))
                ;

            var dummy =
                new Dummy();

            var proxy =
                container.Invoke<Dummy>();

            dummy.Call_SetPropText_complex_Return_1("text", DateTime.Now, new Dummy.EvenMore());
            proxy.Call_SetPropText_complex_Return_1("text", DateTime.Now, new Dummy.EvenMore());
        }

        [Test]
        public void PassingInstanceAndMethodName()
        {
            var container = new SurrogatesContainer();

            container.Map(m => m
                .From<Dummy>()
                .Replace
                .This(d => (Func<int>)d.Call_SetPropText_simple_Return_1)
                .Using<InterferenceObject>(r => (Func<Dummy, string, int>) r.SetPropText_InstanceAndMethodName_Return2));

            var dummy =
                new Dummy();

            var proxy =
                container.Invoke<Dummy>();

            var dummyRes =
                dummy.Call_SetPropText_simple_Return_1();
            var proxyRes =
                proxy.Call_SetPropText_simple_Return_1();

            Assert.IsNotNullOrEmpty(dummy.Text);
            Assert.AreEqual("simple", dummy.Text);

            Assert.IsNotNullOrEmpty(proxy.Text);
            Assert.AreEqual(typeof(Dummy).Name + "Proxy+Call_SetPropText_simple_Return_1", proxy.Text);
            Assert.AreNotEqual(dummyRes, proxyRes);
            Assert.AreEqual(2, proxyRes);
        }
    }
}
