using NUnit.Framework;
using Surrogates.Tests.Expressions.Entities;
using System;

namespace Surrogates.Tests.Expressions.Methods.Visit
{
    class Visit_ActionWtihFunctionTest : IInterferenceTest
    {
        [Test]
        public void BothParameterLess()
        {
            var container = new SurrogatesContainer();

            container.Map(m => m
                .From<Dummy>()
                .Visit
                .This(d => (Action) d.SetPropText_simple)
                .Using<InterferenceObject>(r => (Func<int>)r.AccomplishNothing_Return2));

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
        public void PassingBaseParameters()
        {
            var container = new SurrogatesContainer();

            container.Map(m =>
                m.From<Dummy>()
                .Visit
                .This(d => (Action<string, DateTime, Dummy.EvenMore>)d.SetPropText_complex)
                .Using<InterferenceObject>(r => (Func<string, Dummy, DateTime, string, Dummy.EvenMore, int>)r.AddToPropText__MethodName_Return2));

            var dummy =
                new Dummy();

            var proxy =
                container.Invoke<Dummy>();

            //and now, the comparison between the two methods
            dummy.SetPropText_complex("this call was not made by the original property", DateTime.Now, new Dummy.EvenMore());
            proxy.SetPropText_complex("this call was not made by the original property", DateTime.Now, new Dummy.EvenMore());

            Assert.That(dummy.Text, Is.Not.Null.Or.Empty);
            Assert.AreEqual("complex", dummy.Text);
            Assert.That(proxy.Text, Is.Not.Null.Or.Empty);
            Assert.AreEqual("complex", proxy.Text);
        }

        [Test]
        public void NotPassingBaseParameters()
        {
            var container = new SurrogatesContainer();

            container.Map(m =>
                m.From<Dummy>()
                .Visit
                .This(d => (Action<string, DateTime, Dummy.EvenMore>) d.SetPropText_complex)
                .Using<InterferenceObject>("DontAddToPropText__MethodName_Return2"));

            var dummy =
                new Dummy();

            var proxy =
                container.Invoke<Dummy>();

            dummy.SetPropText_complex("text", DateTime.Now, new Dummy.EvenMore());

            Assert.Throws<NullReferenceException>(() => { 
                proxy.SetPropText_complex("text", DateTime.Now, new Dummy.EvenMore());
            });
        }

        [Test]
        public void PassingInstanceAndMethodName()
        {
            var container = new SurrogatesContainer();

            container.Map(m => m
                .From<Dummy>()
                .Visit
                .This(d => (Action)d.SetPropText_simple)
                .Using<InterferenceObject>(r => (Func<Dummy, string, int>)r.SetPropText_InstanceAndMethodName_Return2));

            var dummy =
                new Dummy();

            var proxy =
                container.Invoke<Dummy>();

            dummy.SetPropText_simple();
            proxy.SetPropText_simple();


            Assert.AreEqual("simple", dummy.Text);
            Assert.AreEqual("simple", proxy.Text);
        }
    }
}