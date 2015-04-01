using NUnit.Framework;
using Surrogates.Tests.Expressions.Entities;
using System;

namespace Surrogates.Tests.Strategies.Methods.Visit
{
    public class Visit_ActionWtihFunctionTest : VisitStrategiesTests, IInterferenceTest
    {
        [Test]
        public void BothParameterLess()
        {
            var dummy =
                new Dummy();

            var proxy = Replace<Dummy, InterferenceObject>(
                 (Action)new Dummy().SetPropText_simple,
                null,
                (Func<int>)new InterferenceObject().AccomplishNothing_Return2);

            dummy.SetPropText_simple();
            proxy.SetPropText_simple();

            Assert.AreEqual("simple", dummy.Text);
            Assert.AreEqual("simple", proxy.Text);
        }

        [Test]
        public void PassingBaseParameters()
        {
            var dummy =
                new Dummy();

            var proxy = Replace<Dummy, InterferenceObject>(
                (Action<string, DateTime, Dummy.EvenMore>)new Dummy().SetPropText_complex,
                null,
                (Func<string, Dummy, DateTime, string, Dummy.EvenMore, int>)new InterferenceObject().AddToPropText__MethodName_Return2);

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
            var dummy =
                new Dummy();

            var proxy = Replace<Dummy, InterferenceObject>(
                 (Action<string, DateTime, Dummy.EvenMore>)new Dummy().SetPropText_complex,
                null,
                (Func<string, Dummy, DateTime, string, Dummy.EvenMore, int>)new InterferenceObject().DontAddToPropText__MethodName_Return2);

            dummy.SetPropText_complex("text", DateTime.Now, new Dummy.EvenMore());
            proxy.SetPropText_complex("text", DateTime.Now, new Dummy.EvenMore());
        }

        [Test]
        public void PassingInstanceAndMethodName()
        {
            var dummy =
                new Dummy();

            var proxy = Replace<Dummy, InterferenceObject>(
                (Action)new Dummy().SetPropText_simple,
                null,
                (Func<Dummy, string, int>)new InterferenceObject().SetPropText_InstanceAndMethodName_Return2);

            dummy.SetPropText_simple();
            proxy.SetPropText_simple();


            Assert.AreEqual("simple", dummy.Text);
            Assert.AreEqual("simple", proxy.Text);
        }
    }
}