using NUnit.Framework;
using Surrogates.Tests.Simple.Entities;
using System;

namespace Surrogates.Tests.Simple.Methods.Visit
{
    [TestFixture]
    public class Visit_ActionWithActionTest : IInterferenceTest
    {
        [Test]
        public void BothParameterLess()
        {
            var container = new SurrogatesContainer();

            container.Map(m => m
                .From<Dummy>()
                .Visit
                .This(d => (Action) d.SetPropText_simple)
                .Using<InterferenceObject>("AccomplishNothing"));

            var dummy =
                new Dummy();

            var proxy =
                container.Invoke<Dummy>();

            dummy.SetPropText_simple();
            proxy.SetPropText_simple();

            Assert.AreEqual("simple", dummy.Text);
            Assert.AreEqual("simple", proxy.Text);
        }

        [Test]
        public void  PassingBaseParameters()
        {
            var container = new SurrogatesContainer();

            container.Map(m =>
                m.From<Dummy>()
                .Visit
                .This(d => (Action<string, DateTime, Dummy.EvenMore>)d.SetPropText_complex)
                .Using<InterferenceObject>(r => (Action<string, Dummy, DateTime, string, Dummy.EvenMore>)r.AddToPropText__MethodName));

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
            dummy.SetPropText_complex("this call was not made by the original property", DateTime.Now, new Dummy.EvenMore());
            proxy.SetPropText_complex("this call was not made by the original property", DateTime.Now, new Dummy.EvenMore());

            Assert.IsNotNullOrEmpty(dummy.Text);
            Assert.AreEqual("complex", dummy.Text);
            Assert.IsNotNullOrEmpty(proxy.Text);
            Assert.AreEqual("complex", proxy.Text);
        }

        [Test, ExpectedException(typeof(NullReferenceException))]
        public void NotPassingBaseParameters()
        {
            var container = new SurrogatesContainer();

            container.Map(m =>
                m.From<Dummy>()
                .Visit
                .This(d => (Action<string, DateTime, Dummy.EvenMore>) d.SetPropText_complex)
                .Using<InterferenceObject>("Void_VariousParametersWithDifferentNames")).Save();
            
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
                .From<Dummy>()
                .Visit
                .This(d => (Action) d.SetPropText_simple)
                .Using<InterferenceObject>(r => (Action<Dummy, string>)r.SetPropText_InstanceAndMethodName));

            var dummy =
                new Dummy();

            var proxy =
                container.Invoke<Dummy>();

            dummy.SetPropText_simple();
            proxy.SetPropText_simple();

            Assert.IsNotNullOrEmpty(dummy.Text);
            Assert.AreEqual("simple", dummy.Text);
            Assert.AreEqual("simple", proxy.Text);
        }
    }
}
