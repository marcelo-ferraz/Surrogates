using System;
using NUnit.Framework;
using Surrogates.Tests.Simple.Entities;

namespace Surrogates.Tests.Simple.Methods.Visit
{
    public class Visit_FuncWithActionTest : IInterferenceTest
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
                .ThisMethod(r => r.AccomplishNothing))
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
                .Visit.ThisMethod<string, DateTime, Dummy.EvenMore, int>(d => d.Call_SetPropText_complex_Return_1)
                .Using<InterferenceObject>()
                .ThisMethod<string, Dummy, DateTime, string, Dummy.EvenMore>(r => r.AddToPropText__MethodName));

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
            Assert.AreEqual("complex", proxy.Text);
            Assert.IsNotNullOrEmpty(proxy.Text);
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
                .ThisMethod<string, Dummy, DateTime, string, Dummy.EvenMore>(r => r.Void_VariousParametersWithDifferentNames))
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
                .ThisMethod<Dummy, string>(r => r.SetPropText_InstanceAndMethodName));

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
    }
}
