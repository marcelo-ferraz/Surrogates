using Surrogates;
using NUnit.Framework;
using Surrogates.Tests.Entities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Surrogates.Tests.Visit
{
    [TestFixture]
    public class Visit_ActionWithActionTest : IInterferenceTest
    {
        [Test]
        public void BothParameterLess()
        {
            var container = new SurrogatesContainer();

            container.Map(m => m
                .Throughout<Dummy>()
                .Visit
                .This(d => d.Void_ParameterLess)
                .With<InterferenceObject>()
                .This(r => r.Void_ParameterLess));

            var dummy =
                new Dummy();

            var proxy =
                container.Invoke<Dummy>();

            dummy.Void_ParameterLess();
            proxy.Void_ParameterLess();

            Assert.AreEqual("simple", dummy.Text);
            Assert.AreEqual("simple", proxy.Text);
        }

        [Test]
        public void  PassingBaseParameters()
        {
            var container = new SurrogatesContainer();

            container.Map(m =>
                m.Throughout<Dummy>()
                .Visit
                .This<string, DateTime, Dummy.EvenMore>(d => d.Void_VariousParameters)
                .With<InterferenceObject>()
                .This<string, Dummy, DateTime, string, Dummy.EvenMore>(r => r.Void_VariousParametersPlusIntanceAndMethodName));

            var dummy =
                new Dummy();

            var proxy =
                container.Invoke<Dummy>();

            // just to show that the rest of the object behaves as expected
            dummy.Void_ParameterLess();
            proxy.Void_ParameterLess();

            Assert.IsNotNullOrEmpty(dummy.Text);
            Assert.AreEqual("simple", proxy.Text);
            Assert.IsNotNullOrEmpty(proxy.Text);
            Assert.AreEqual("simple", proxy.Text);

            //and now, the comparison between the two methods
            dummy.Void_VariousParameters("this call was not made by the original method", DateTime.Now, new Dummy.EvenMore());
            proxy.Void_VariousParameters("this call was not made by the original method", DateTime.Now, new Dummy.EvenMore());

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
                .Visit
                .This<string, DateTime, Dummy.EvenMore>(d => d.Void_VariousParameters)
                .With<InterferenceObject>()
                .This<string, Dummy, DateTime, string, Dummy.EvenMore>(r => r.Void_VariousParametersWithDifferentNames));
            
            var dummy =
                new Dummy();
            
            var proxy =
                container.Invoke<Dummy>();
            
            dummy.Void_VariousParameters("text", DateTime.Now, new Dummy.EvenMore());
            proxy.Void_VariousParameters("text", DateTime.Now, new Dummy.EvenMore());
        }

        [Test]
        public void  PassingInstanceAndMethodName() 
        {
            var container = new SurrogatesContainer();

            container.Map(m => m
                .Throughout<Dummy>()
                .Visit
                .This(d => d.Void_ParameterLess)
                .With<InterferenceObject>()
                .This<Dummy, string>(r => r.Void_InstanceAndMethodName));

            var dummy =
                new Dummy();

            var proxy =
                container.Invoke<Dummy>();

            dummy.Void_ParameterLess();
            proxy.Void_ParameterLess();

            Assert.IsNotNullOrEmpty(dummy.Text);
            Assert.AreEqual("simple", dummy.Text);
            Assert.AreEqual("simple", proxy.Text);
        }
    }
}
