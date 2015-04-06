using NUnit.Framework;
using Surrogates.Tests.Expressions.Entities;
using System;

namespace Surrogates.Tests.Strategies.Methods.Substitute
{
    [TestFixture]
    public class Substitute_ActionUsingActionTest : SubstituteStrategiesTests, IInterferenceTest
    {      
        [Test]
        public void BothParameterLess()
        {
            var dummy =
                new Dummy();

            var proxy = Replace<Dummy, InterferenceObject>(
                (Action)new Dummy().SetPropText_simple, 
                null, 
                (Action) new InterferenceObject().AccomplishNothing);

            dummy.SetPropText_simple();
            proxy.SetPropText_simple();

            Assert.AreEqual("simple", dummy.Text);
            Assert.IsNullOrEmpty(proxy.Text);
        }

        [Test]
        public void  PassingBaseParameters()
        {
            var dummy =
                new Dummy();

            var proxy = Replace<Dummy, InterferenceObject>(
                (Action<string, DateTime, Dummy.EvenMore>) new Dummy().SetPropText_complex,
                null,
                (Action<string, Dummy, DateTime, string, Dummy.EvenMore>) new InterferenceObject().AddToPropText__MethodName);

            // just to show that the rest of the object behaves as expected
            dummy.SetPropText_simple();
            proxy.SetPropText_simple();

            Assert.IsNotNullOrEmpty(dummy.Text);
            Assert.AreEqual("simple", proxy.Text);
            Assert.IsNotNullOrEmpty(proxy.Text);
            Assert.AreEqual("simple", proxy.Text);

            //and now, the comparison between the two methods
            dummy.SetPropText_complex("this call was not made by the original property", DateTime.Now, new Dummy.EvenMore());
            proxy.SetPropText_complex("this call was not made by the original property", DateTime.Now, new Dummy.EvenMore());

            Assert.IsNotNullOrEmpty(dummy.Text);
            Assert.AreEqual("complex", dummy.Text);
            Assert.IsNotNullOrEmpty(proxy.Text);
            Assert.AreEqual("simple, this call was not made by the original property - property: SetPropText_complex", proxy.Text);
        }

        [Test, ExpectedException(typeof(NullReferenceException))]
        public void NotPassingBaseParameters()
        {
            var dummy =
                new Dummy();
            
            var proxy = Replace<Dummy, InterferenceObject>(
                (Action<string, DateTime, Dummy.EvenMore>) new Dummy().SetPropText_complex,
                null,
                (Action<string, Dummy, DateTime, string, Dummy.EvenMore>)new InterferenceObject().Void_VariousParametersWithDifferentNames);
            
            dummy.SetPropText_complex("text", DateTime.Now, new Dummy.EvenMore());
            proxy.SetPropText_complex("text", DateTime.Now, new Dummy.EvenMore());
        }

        [Test]
        public void  PassingInstanceAndMethodName() 
        {
            var dummy =
                new Dummy();

            var proxy = Replace<Dummy, InterferenceObject>(
                (Action) new Dummy().SetPropText_simple,
                null,
                (Action<Dummy, string>) new InterferenceObject().SetPropText_InstanceAndMethodName);

            dummy.SetPropText_simple();
            proxy.SetPropText_simple();

            Assert.IsNotNullOrEmpty(dummy.Text);
            Assert.AreEqual("simple", dummy.Text);

            Assert.IsNotNullOrEmpty(proxy.Text);
            Assert.AreEqual(typeof(Dummy).Name + "Proxy+SetPropText_simple", proxy.Text);
        }
    }
}
