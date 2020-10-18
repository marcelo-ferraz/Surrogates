using NUnit.Framework;
using Surrogates.Tests.Expressions.Entities;
using System;

namespace Surrogates.Tests.Expressions.Methods.Substitute
{
    [TestFixture]
    public class Substitute_ActionUsingActionTest : IInterferenceTest
    {
        [Test]
        public void BothParameterLess()
        {
            var container = new SurrogatesContainer();

            container.Map(m => m
                .From<Dummy>()
                .Replace
                .This(d => (Action) d.SetPropText_simple)
                .Using<InterferenceObject>(r => (Action) r.AccomplishNothing));

            var dummy =
                new Dummy();

            var proxy =
                container.Invoke<Dummy>();

            dummy.SetPropText_simple();
            proxy.SetPropText_simple();

            Assert.AreEqual("simple", dummy.Text);
            Assert.That(proxy.Text, Is.Null.Or.Empty);
        }

        [Test]
        public void  PassingBaseParameters()
        {
            var container = new SurrogatesContainer();

            container.Map(m =>
                m.From<Dummy>()
                .Replace
                .This(d => (Action<string, DateTime, Dummy.EvenMore>) d.SetPropText_complex)
                .Using<InterferenceObject>(r => (Action<string, Dummy, DateTime, string, Dummy.EvenMore>)r.AddToPropText__MethodName));

            var dummy =
                new Dummy();

            var proxy =
                container.Invoke<Dummy>();

            // just to show that the rest of the object behaves as expected
            dummy.SetPropText_simple();
            proxy.SetPropText_simple();

            Assert.That(dummy.Text, Is.Not.Null.Or.Empty);
            Assert.AreEqual("simple", proxy.Text);
            Assert.That(proxy.Text, Is.Not.Null.Or.Empty);
            Assert.AreEqual("simple", proxy.Text);

            //and now, the comparison between the two methods
            dummy.SetPropText_complex("this call was not made by the original property", DateTime.Now, new Dummy.EvenMore());
            proxy.SetPropText_complex("this call was not made by the original property", DateTime.Now, new Dummy.EvenMore());

            Assert.That(dummy.Text,  Is.Not.Null.Or.Empty);
            Assert.AreEqual("complex", dummy.Text);
            Assert.That(proxy.Text, Is.Not.Null.Or.Empty);
            Assert.AreEqual("simple, this call was not made by the original property - property: SetPropText_complex", proxy.Text);
        }

        [Test]
        public void NotPassingBaseParameters()
        {
            var container = new SurrogatesContainer();

            container.Map(m =>
                m.From<Dummy>()
                .Replace
                .This(d => (Action<string, DateTime, Dummy.EvenMore>)d.SetPropText_complex)
                .Using<InterferenceObject>("Void_VariousParametersWithDifferentNames"));
            
            var dummy =
                new Dummy();
            
            var proxy =
                container.Invoke<Dummy>();

            Assert.Throws<NullReferenceException>(() =>
            {
                dummy.SetPropText_complex("text", DateTime.Now, new Dummy.EvenMore());
                proxy.SetPropText_complex("text", DateTime.Now, new Dummy.EvenMore());
            });
        }

        [Test]
        public void  PassingInstanceAndMethodName() 
        {
            var container = new SurrogatesContainer();

            container.Map(m => m
                .From<Dummy>()
                .Replace
                .This(d => (Action) d.SetPropText_simple)
                .Using<InterferenceObject>(r => (Action<Dummy, string>) r.SetPropText_InstanceAndMethodName));

            var dummy =
                new Dummy();

            var proxy =
                container.Invoke<Dummy>();

            dummy.SetPropText_simple();
            proxy.SetPropText_simple();

            Assert.That(dummy.Text, Is.Not.Null.Or.Empty);
            Assert.AreEqual("simple", dummy.Text);

            Assert.That(proxy.Text, Is.Not.Null.Or.Empty);
            Assert.AreEqual(typeof(Dummy).Name + "Proxy+SetPropText_simple", proxy.Text);
        }
    }
}
