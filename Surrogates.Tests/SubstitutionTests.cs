using DynamicProxy;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Surrogates.Tests
{
    [TestFixture]
    public class SubstitutionTests
    {
        [Test]
        public void SimpleVoidMethodReplacedWithVoid()
        {
            var container = new SurrogatesContainer(); 
         
            container.Map(m =>
                m.Throughout<Dummy>()
                .Substitute.This(d => d.VoidParameterless).With<Replacement>().This(r => r.VoidParameterless));

            var dummy = 
                new Dummy();

            var proxy = 
                container.Invoke<Dummy>();

            dummy.VoidParameterless();
            proxy.VoidParameterless();

            Assert.IsNotNullOrEmpty(dummy.Text);
            Assert.AreEqual("simple", proxy.Text);
            Assert.IsNullOrEmpty(proxy.Text);
        }

        [Test]
        public void SimpleVoidMethodReplacedWithVoidPassingInstance()
        {
            var container = new SurrogatesContainer();

            container.Map(m =>
                m.Throughout<Dummy>()
                .Substitute.This(d => d.VoidParameterless).With<Replacement>().This<Dummy>(r => r.VoidAsking4Instance));

            var dummy =
                new Dummy();

            var proxy =
                container.Invoke<Dummy>();

            dummy.VoidParameterless();
            proxy.VoidParameterless();

            Assert.IsNotNullOrEmpty(dummy.Text);
            Assert.AreEqual("simple", proxy.Text);
            Assert.IsNotNullOrEmpty(proxy.Text);
            Assert.AreEqual(typeof(Dummy).Name, proxy.Text);
        }

        [Test]
        public void SimpleVoidMethodReplacedWithVoidPassingBaseMethodName()
        {
            var container = new SurrogatesContainer();

            container.Map(m =>
                m.Throughout<Dummy>()
                .Substitute.This(d => d.VoidParameterless).With<Replacement>().This<string>(r => r.VoidThrowExceptionWithOriginalMethodName));

            var proxy =
                container.Invoke<Dummy>();

            string msg = "";

            try
            {
                proxy.VoidParameterless();
            }
            catch (Exception ex)
            {
                if ((msg = ex.Message) == "VoidParameterless")
                { Assert.Pass(); }
            }

            Assert.Fail("The test for the base method's name being passed as a parameter failed. The message contained: " + msg);
        }


        [Test]
        public void ComplexVoidMethodReplacedWithVoid()
        {
            var container = new SurrogatesContainer();

            container.Map(m =>
                m.Throughout<Dummy>()
                .Substitute.This<string, DateTime, Dummy.EvenMore>(d => d.ComplexVoid).With<Replacement>().This<Dummy>(r => r.VoidAsking4Instance));

            var dummy =
                new Dummy();

            var proxy =
                container.Invoke<Dummy>();

            dummy.VoidParameterless();
            proxy.VoidParameterless();

            Assert.IsNotNullOrEmpty(dummy.Text);
            Assert.AreEqual("complex", proxy.Text);
            Assert.IsNotNullOrEmpty(proxy.Text);
            Assert.AreEqual("replaced", proxy.Text);
        }

        [Test]
        public void ComplexVoidMethodReplacedWithComplexVoid()
        {
            var container = new SurrogatesContainer();

            container.Map(m =>
                m.Throughout<Dummy>()
                .Substitute.This<string, DateTime, Dummy.EvenMore>(d => d.ComplexVoid)
                .With<Replacement>()
                .This<string, Dummy, DateTime, string, Dummy.EvenMore>(r => r.ComplexVoid));

            var dummy =
                new Dummy();

            var proxy =
                container.Invoke<Dummy>();

            // just to show that the rest of the object behaves as expected
            dummy.VoidParameterless();
            proxy.VoidParameterless();

            Assert.IsNotNullOrEmpty(dummy.Text);
            Assert.AreEqual("simple", proxy.Text);
            Assert.IsNotNullOrEmpty(proxy.Text);
            Assert.AreEqual("simple", proxy.Text);

            //and now, the comparison between the two methods
            dummy.ComplexVoid("this call was not made by the original method", DateTime.Now, new Dummy.EvenMore());
            proxy.ComplexVoid("this call was not made by the original method", DateTime.Now, new Dummy.EvenMore());

            Assert.IsNotNullOrEmpty(dummy.Text);
            Assert.AreEqual("more complex", proxy.Text);
            Assert.IsNotNullOrEmpty(proxy.Text);
            Assert.AreEqual("this call was not made by the original method-ComplexVoid", proxy.Text);
        }

        [Test]
        public void ComplexVoidMethodReplacedWithVoidWithDifferentParameterNames()
        {
            var container = new SurrogatesContainer();

            container.Map(m =>
                m.Throughout<Dummy>()
                .Substitute.This<string, DateTime, Dummy.EvenMore>(d => d.ComplexVoid)
                .With<Replacement>()
                .This<string, Dummy, DateTime, string, Dummy.EvenMore>(r => r.ComplexVoidWithDifferentParameterNames));

            var dummy =
                new Dummy();

            var proxy =
                container.Invoke<Dummy>();

            // just to show that the rest of the object behaves as expected
            dummy.VoidParameterless();
            proxy.VoidParameterless();

            Assert.IsNotNullOrEmpty(dummy.Text);
            Assert.AreEqual("simple", proxy.Text);
            Assert.IsNotNullOrEmpty(proxy.Text);
            Assert.AreEqual("simple", proxy.Text);

            //and now, the comparison between the two methods
            dummy.ComplexVoid("this call was not made by the original method", DateTime.Now, new Dummy.EvenMore());
            proxy.ComplexVoid("this call was not made by the original method", DateTime.Now, new Dummy.EvenMore());

            Assert.IsNotNullOrEmpty(dummy.Text);
            Assert.AreEqual("more complex", proxy.Text);
            Assert.IsNotNullOrEmpty(proxy.Text);
            Assert.AreEqual("-", proxy.Text);
        }
    }
}
