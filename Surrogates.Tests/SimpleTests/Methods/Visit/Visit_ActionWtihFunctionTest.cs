using System;
using NUnit.Framework;
using Surrogates.Tests.Simple.Entities;

namespace Surrogates.Tests.Simple.Methods.Visit
{
    class Visit_ActionWtihFunctionTest : IInterferenceTest
    {
        [Test]
        public void BothParameterLess()
        {
            var container = new SurrogatesContainer();

            container.Map(m => m
                .Throughout<Dummy>()
                .Visit
                .ThisMethod(d => d.SetPropText_simple)
                .Using<InterferenceObject>()
                .ThisMethod<int>(r => r.AccomplishNothing_Return2));

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
                m.Throughout<Dummy>()
                .Visit.ThisMethod<string, DateTime, Dummy.EvenMore>(d => d.SetPropText_complex)
                .Using<InterferenceObject>()
                .ThisMethod<string, Dummy, DateTime, string, Dummy.EvenMore, int>(r => r.AddToPropText__MethodName_Return2));

            var dummy =
                new Dummy();

            var proxy =
                container.Invoke<Dummy>();

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
                m.Throughout<Dummy>()
                .Visit.ThisMethod<string, DateTime, Dummy.EvenMore>(d => d.SetPropText_complex)
                .Using<InterferenceObject>()
                .ThisMethod<string, Dummy, DateTime, string, Dummy.EvenMore, int>(r => r.DontAddToPropText__MethodName_Return2));

            var dummy =
                new Dummy();

            var proxy =
                container.Invoke<Dummy>();

            dummy.SetPropText_complex("text", DateTime.Now, new Dummy.EvenMore());
            proxy.SetPropText_complex("text", DateTime.Now, new Dummy.EvenMore());
        }

        [Test]
        public void PassingInstanceAndMethodName()
        {
            var container = new SurrogatesContainer();

            container.Map(m => m
                .Throughout<Dummy>()
                .Visit
                .ThisMethod(d => d.SetPropText_simple)
                .Using<InterferenceObject>()
                .ThisMethod<Dummy, string, int>(r => r.SetPropText_InstanceAndMethodName_Return2))
                ;

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