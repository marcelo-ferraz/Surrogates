using System;
using NUnit.Framework;
using Surrogates.Tests.Simple.Entities;

namespace Surrogates.Tests.Simple.Methods.Substitute
{
    [TestFixture]
    public class Substitute_ActionUsingActionTest : IInterferenceTest
    {
        [Test]
        public void BothParameterLess()
        {
            var container = new SurrogatesContainer();

            container.Map(m => m
                .Throughout<Dummy>()
                .Replace
                .ThisMethod(d => d.SetPropText_simple)
                .Using<InterferenceObject>()
                .ThisMethod(r => r.AccomplishNothing));

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
        public void  PassingBaseParameters()
        {
            var container = new SurrogatesContainer();

            container.Map(m =>
                m.Throughout<Dummy>()
                .Replace.ThisMethod<string, DateTime, Dummy.EvenMore>(d => d.SetPropText_complex)
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
            Assert.AreEqual("simple", proxy.Text);
            Assert.IsNotNullOrEmpty(proxy.Text);
            Assert.AreEqual("simple", proxy.Text);

            //and now, the comparison between the two methods
            dummy.SetPropText_complex("this call was not made by the original method", DateTime.Now, new Dummy.EvenMore());
            proxy.SetPropText_complex("this call was not made by the original method", DateTime.Now, new Dummy.EvenMore());

            Assert.IsNotNullOrEmpty(dummy.Text);
            Assert.AreEqual("complex", dummy.Text);
            Assert.IsNotNullOrEmpty(proxy.Text);
            Assert.AreEqual("simple, this call was not made by the original method - method: SetPropText_complex", proxy.Text);
        }

        [Test, ExpectedException(typeof(NullReferenceException))]
        public void NotPassingBaseParameters()
        {
            var container = new SurrogatesContainer();

            container.Map(m =>
                m.Throughout<Dummy>()
                .Replace.ThisMethod<string, DateTime, Dummy.EvenMore>(d => d.SetPropText_complex)
                .Using<InterferenceObject>()
                .ThisMethod<string, Dummy, DateTime, string, Dummy.EvenMore>(r => r.Void_VariousParametersWithDifferentNames));
            
            var dummy =
                new Dummy();
            
            var proxy =
                container.Invoke<Dummy>();
            
            dummy.SetPropText_complex("text", DateTime.Now, new Dummy.EvenMore());
            proxy.SetPropText_complex("text", DateTime.Now, new Dummy.EvenMore());
        }

        [Test]
        public void  PassingInstanceAndMethodName() 
        {
            var container = new SurrogatesContainer();

            container.Map(m => m
                .Throughout<Dummy>()
                .Replace
                .ThisMethod(d => d.SetPropText_simple)
                .Using<InterferenceObject>()
                .ThisMethod<Dummy, string>(r => r.SetPropText_InstanceAndMethodName));

            var dummy =
                new Dummy();

            var proxy =
                container.Invoke<Dummy>();

            dummy.SetPropText_simple();
            proxy.SetPropText_simple();

            Assert.IsNotNullOrEmpty(dummy.Text);
            Assert.AreEqual("simple", dummy.Text);

            Assert.IsNotNullOrEmpty(proxy.Text);
            Assert.AreEqual(typeof(Dummy).Name + "Proxy+SetPropText_simple", proxy.Text);
        }
    }
}
