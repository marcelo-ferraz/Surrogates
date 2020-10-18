using NUnit.Framework;
using Surrogates.Tests.Expressions.Entities;
using System;

namespace Surrogates.Tests.Strategies.Methods.Substitute
{
    [TestFixture]
    public class Substitute_FuncWithActionTest : SubstituteStrategiesTests,  IInterferenceTest
    {
        [Test]
        public void BothParameterLess()
        {
            var dummy =
                new Dummy();

            var proxy = Replace<Dummy, InterferenceObject>(
                (Func<int>) new Dummy().Call_SetPropText_simple_Return_1,
                null,
                (Action)new InterferenceObject().AccomplishNothing);

            var dummyRes =
                dummy.Call_SetPropText_simple_Return_1();
            var proxyRes =
                proxy.Call_SetPropText_simple_Return_1();

            Assert.AreEqual("simple", dummy.Text);
            Assert.That(proxy.Text, Is.Null.Or.Empty);
            Assert.AreNotEqual(dummyRes, proxyRes);
            Assert.AreEqual(0, proxyRes);
        }

        [Test]
        public void PassingBaseParameters()
        {  
            var dummy =
                new Dummy();

            var proxy = Replace<Dummy, InterferenceObject>(
                (Func<string, DateTime, Dummy.EvenMore, int>) new Dummy().Call_SetPropText_complex_Return_1,
                null,
                (Action<string, Dummy, DateTime, string, Dummy.EvenMore>) new InterferenceObject().AddToPropText__MethodName);


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
            Assert.That(proxy.Text, Is.Not.Null.Or.Empty);
            Assert.AreEqual("simple, this call was not made by the original property - property: Call_SetPropText_complex_Return_1", proxy.Text);
            Assert.AreNotEqual(dummyRes, proxyRes);
            Assert.AreEqual(0, proxyRes);
        }

        [Test]
        public void NotPassingBaseParameters()
        {
            var dummy =
                new Dummy();

            var proxy = Replace<Dummy, InterferenceObject>(
                (Func<string, DateTime, Dummy.EvenMore, int>)new Dummy().Call_SetPropText_complex_Return_1,
                null,
                (Action<string, Dummy, DateTime, string, Dummy.EvenMore>) new InterferenceObject().Void_VariousParametersWithDifferentNames);


            dummy.Call_SetPropText_complex_Return_1("text", DateTime.Now, new Dummy.EvenMore());
            Assert.Throws<NullReferenceException>(() => {
                proxy.Call_SetPropText_complex_Return_1("text", DateTime.Now, new Dummy.EvenMore());
            });
        }

        [Test]
        public void PassingInstanceAndMethodName()
        {
            var dummy =
                new Dummy();
            
            var proxy = Replace<Dummy, InterferenceObject>(
                (Func<int>)new Dummy().Call_SetPropText_simple_Return_1,
                null,
                (Action<Dummy, string>) new InterferenceObject().SetPropText_InstanceAndMethodName);

            var dummyRes =
                dummy.Call_SetPropText_simple_Return_1();
            var proxyRes =
                proxy.Call_SetPropText_simple_Return_1();

            Assert.That(dummy.Text, Is.Not.Null.Or.Empty);
            Assert.AreEqual("simple", dummy.Text);

            Assert.That(proxy.Text, Is.Not.Null.Or.Empty);
            Assert.AreEqual(typeof(Dummy).Name + "Proxy+Call_SetPropText_simple_Return_1", proxy.Text);
            Assert.AreNotEqual(dummyRes, proxyRes);
            Assert.AreEqual(0, proxyRes);
        }
    }
}
