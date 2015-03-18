using System;
using NUnit.Framework;
using Surrogates.Tests.Simple.Entities;

namespace Surrogates.Tests.Simple.Methods.Visit
{
    [TestFixture]
    public class Visit_FuncWithFuncTest : IInterferenceTest
    {
        [Test]
        public void BothParameterLess()
        {
            var container = new SurrogatesContainer();

            container.Map(m => m
                .Throughout<Dummy>()
                .Visit
                .ThisMethod<int>(d => d.Call_SetPropText_simple_Return_1)
                .Using<InterferenceObject>()
                .ThisMethod<int>(r => r.AccomplishNothing_Return2))
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
            Assert.AreEqual("simple", proxy.Text);
            Assert.AreEqual(dummyRes, proxyRes);
        }

        [Test]
        public void PassingBaseParameters()
        {
            var container = new SurrogatesContainer();

            container.Map(m =>
                m.Throughout<Dummy>()
                .Visit
                .ThisMethod<string, DateTime, Dummy.EvenMore, int>(d => d.Call_SetPropText_complex_Return_1)
                .Using<InterferenceObject>()
                .ThisMethod<string, Dummy, DateTime, string, Dummy.EvenMore, int>(r => r.AddToPropText__MethodName_Return2))
                ;

            var dummy =
                new Dummy();

            var proxy =
                container.Invoke<Dummy>();

            //and now, the comparison between the two methods
            var dummyRes =
                dummy.Call_SetPropText_complex_Return_1("this call was not made by the original property", DateTime.Now, new Dummy.EvenMore());

            var proxyRes =
                proxy.Call_SetPropText_complex_Return_1("this call was not made by the original property", DateTime.Now, new Dummy.EvenMore());

            Assert.IsNotNullOrEmpty(dummy.Text);
            Assert.AreEqual("complex", dummy.Text);
            Assert.IsNotNullOrEmpty(proxy.Text);
            Assert.AreEqual("complex", proxy.Text);
            Assert.AreEqual(dummyRes, proxyRes);
        }

        [Test, ExpectedException(typeof(NullReferenceException))]
        public void NotPassingBaseParameters()
        {
            var container = new SurrogatesContainer();

            container.Map(m =>
                m.Throughout<Dummy>()
                .Visit.ThisMethod<string, DateTime, Dummy.EvenMore, int>(d => d.Call_SetPropText_complex_Return_1)
                .Using<InterferenceObject>()
                .ThisMethod<string, Dummy, DateTime, string, Dummy.EvenMore, int>(r => r.DontAddToPropText__MethodName_Return2))
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
                .Throughout<Dummy>()
                .Visit
                .ThisMethod<int>(d => d.Call_SetPropText_simple_Return_1)
                .Using<InterferenceObject>()
                .ThisMethod<Dummy, string, int>(r => r.SetPropText_InstanceAndMethodName_Return2));

            var dummy =
                new Dummy();

            var proxy =
                container.Invoke<Dummy>();

            var dummyRes =
                dummy.Call_SetPropText_simple_Return_1();
            var proxyRes =
                proxy.Call_SetPropText_simple_Return_1();

            Assert.AreEqual("simple", dummy.Text);
            Assert.IsNotNullOrEmpty(proxy.Text);
            Assert.AreEqual("simple", proxy.Text);
            Assert.AreEqual(dummyRes, proxyRes);
        }
    }
}
