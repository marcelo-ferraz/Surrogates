using Surrogates;
using NUnit.Framework;
using Surrogates.Tests.Simple.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Surrogates.Tests.Simple.Methods.Visit
{
    public class Visit_FuncWithActionTest : IInterferenceTest
    {
        [Test]
        public void BothParameterLess()
        {
            var container = new SurrogatesContainer();

            container.Map(m => m
                .Throughout<Dummy>()
                .Visit
                .ThisMethod<int>(d => d.Int_1_ParameterLess)
                .Using<InterferenceObject>()
                .ThisMethod(r => r.Void_ParameterLess))
                ;

            var dummy =
                new Dummy();

            var proxy =
                container.Invoke<Dummy>();

            var dummyRes = 
                dummy.Int_1_ParameterLess();
            var proxyRes = 
                proxy.Int_1_ParameterLess();

            Assert.AreEqual("simple", dummy.Text);
            Assert.AreEqual("simple", proxy.Text);
            Assert.AreEqual(dummyRes, proxyRes);
        }

        [Test]
        public void PassingBaseParameters()
        {
            var container = new SurrogatesContainer();

            container.Map(m =>
                m.Throughout<Dummy>()
                .Visit.ThisMethod<string, DateTime, Dummy.EvenMore, int>(d => d.Int_1_VariousParameters)
                .Using<InterferenceObject>()
                .ThisMethod<string, Dummy, DateTime, string, Dummy.EvenMore>(r => r.Void_VariousParametersPlusIntanceAndMethodName));

            var dummy =
                new Dummy();

            var proxy =
                container.Invoke<Dummy>();

            // just to show that the rest of the object behaves as expected
            dummy.Void_ParameterLess();
            proxy.Void_ParameterLess();

            Assert.IsNotNullOrEmpty(dummy.Text);
            Assert.AreEqual("simple", dummy.Text);
            Assert.IsNotNullOrEmpty(proxy.Text);
            Assert.AreEqual("simple", proxy.Text);

            //and now, the comparison between the two methods
            var dummyRes = 
                dummy.Int_1_VariousParameters("this call was not made by the original method", DateTime.Now, new Dummy.EvenMore());
            
            var proxyRes = 
                proxy.Int_1_VariousParameters("this call was not made by the original method", DateTime.Now, new Dummy.EvenMore());

            Assert.IsNotNullOrEmpty(dummy.Text);
            Assert.AreEqual("complex", dummy.Text);
            Assert.AreEqual("complex", proxy.Text);
            Assert.IsNotNullOrEmpty(proxy.Text);
            Assert.AreEqual(dummyRes, proxyRes);
        }

        [Test, ExpectedException(typeof(NullReferenceException))]
        public void NotPassingBaseParameters()
        {
            var container = new SurrogatesContainer();

            container.Map(m =>
                m.Throughout<Dummy>()
                .Visit.ThisMethod<string, DateTime, Dummy.EvenMore, int>(d => d.Int_1_VariousParameters)
                .Using<InterferenceObject>()
                .ThisMethod<string, Dummy, DateTime, string, Dummy.EvenMore>(r => r.Void_VariousParametersWithDifferentNames))
                ;

            var dummy =
                new Dummy();

            var proxy =
                container.Invoke<Dummy>();

            dummy.Int_1_VariousParameters("text", DateTime.Now, new Dummy.EvenMore());
            proxy.Int_1_VariousParameters("text", DateTime.Now, new Dummy.EvenMore());
        }

        [Test]
        public void PassingInstanceAndMethodName()
        {
            var container = new SurrogatesContainer();

            container.Map(m => m
                .Throughout<Dummy>()
                .Visit
                .ThisMethod<int>(d => d.Int_1_ParameterLess)
                .Using<InterferenceObject>()
                .ThisMethod<Dummy, string>(r => r.Void_InstanceAndMethodName));

            var dummy =
                new Dummy();

            var proxy =
                container.Invoke<Dummy>();

            var dummyRes =
                dummy.Int_1_ParameterLess();
            var proxyRes =
                proxy.Int_1_ParameterLess();

            Assert.AreEqual("simple", dummy.Text);
            Assert.AreEqual("simple", proxy.Text);

            Assert.AreEqual(dummyRes, proxyRes);
        }
    }
}
