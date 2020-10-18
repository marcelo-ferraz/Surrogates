using NUnit.Framework;
using Surrogates.Tests.Expressions.Entities;
using Surrogates.Utilities;
using System;

namespace Surrogates.Tests.Strategies.Methods.Visit
{
    public class Visit_FuncWithActionTest : IInterferenceTest
    {
        [Test]
        public void BothParameterLess()
        {
            var container = new SurrogatesContainer();

            container.Map(m => m
                .From<Dummy>()
                .Visit
                .This(d => (Func<int>)d.Call_SetPropText_simple_Return_1)
                .Using<InterferenceObject>(r => (Action) r.AccomplishNothing))
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
                m.From<Dummy>()
                .Visit
                .This(d => (Func<string, DateTime, Dummy.EvenMore, int>) d.Call_SetPropText_complex_Return_1)
                //.Using(new Action<dynamic>(_ => { })));
                //.Using<InterferenceObject>(r => (Action<string, Dummy, DateTime, string, Dummy.EvenMore>) r.AddToPropText__MethodName));
                .Using<InterferenceObject>(r => (Action<dynamic>) r.Nhonho));

            var dummy =
                new Dummy();
            
            container.Save();

            var proxy =
                container.Invoke<Dummy>();

            // just to show that the rest of the object behaves as expected
            dummy.SetPropText_simple();
            proxy.SetPropText_simple();

            Assert.That(dummy.Text, Is.Not.Null.Or.Empty);
            Assert.AreEqual("simple", dummy.Text);
            Assert.That(proxy.Text, Is.Not.Null.Or.Empty);
            Assert.AreEqual("simple", proxy.Text);

            //and now, the comparison between the two methods
            var dummyRes = 
                dummy.Call_SetPropText_complex_Return_1("this call was not made by the original property", DateTime.Now, new Dummy.EvenMore());
            
            var proxyRes = 
                proxy.Call_SetPropText_complex_Return_1("this call was not made by the original property", DateTime.Now, new Dummy.EvenMore());

            Assert.That(dummy.Text, Is.Not.Null.Or.Empty);
            Assert.AreEqual("complex", dummy.Text);
            Assert.AreEqual("complex", proxy.Text);
            Assert.That(proxy.Text, Is.Not.Null.Or.Empty);
            Assert.AreEqual(dummyRes, proxyRes);
        }
        
        [Test]
        public void NotPassingBaseParameters()
        {
            var container = new SurrogatesContainer();

            container.Map(m =>
                m.From<Dummy>()
                .Visit
                .Method("Call_SetPropText_complex_Return_1")
                .Using<InterferenceObject>("Void_VariousParametersWithDifferentNames"));

            var dummy =
                new Dummy();

            var proxy =
                container.Invoke<Dummy>();

            Assert.Throws<NullReferenceException>(() => { 
                dummy.Call_SetPropText_complex_Return_1("text", DateTime.Now, new Dummy.EvenMore());
                proxy.Call_SetPropText_complex_Return_1("text", DateTime.Now, new Dummy.EvenMore());
            });
        }

        [Test]
        public void PassingInstanceAndMethodName()
        {
            var container = new SurrogatesContainer();

            container.Map(m => m
                .From<Dummy>()
                .Visit
                .This(d => (Func<int>)d.Call_SetPropText_simple_Return_1)
                .Using<InterferenceObject>("SetPropText_InstanceAndMethodName", typeof(Dummy), TypeOf.String));

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
