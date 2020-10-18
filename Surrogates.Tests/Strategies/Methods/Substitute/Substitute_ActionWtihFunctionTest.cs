using NUnit.Framework;
using Surrogates.Tests.Expressions.Entities;
using System;

namespace Surrogates.Tests.Strategies.Methods.Substitute
{
    [TestFixture]
    public class Substitute_ActionWtihFunctionTest : SubstituteStrategiesTests, IInterferenceTest
    {
        [Test]
        public void BothParameterLess()
        {
            var dummy =
                new Dummy();

            var proxy = Replace<Dummy, InterferenceObject>(
                (Action) new Dummy().SetPropText_simple,
                null,
                (Func<int>) new InterferenceObject().AccomplishNothing_Return2);

            dummy.SetPropText_simple();
            proxy.SetPropText_simple();

            Assert.AreEqual("simple", dummy.Text);
            Assert.That(proxy.Text, Is.Null.Or.Empty);
        }

        [Test]
        public void PassingBaseParameters()
        {
            var dummy =
                new Dummy();

            var proxy = Replace<Dummy, InterferenceObject>(
                (Action<string, DateTime, Dummy.EvenMore>) new Dummy().SetPropText_complex,
                null,
                (Func<string, Dummy, DateTime, string, Dummy.EvenMore, int>) new InterferenceObject().AddToPropText__MethodName_Return2);

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

            Assert.That(dummy.Text, Is.Not.Null.Or.Empty);
            Assert.AreEqual("complex", dummy.Text);
            Assert.That(proxy.Text, Is.Not.Null.Or.Empty);
            Assert.AreEqual("simple, this call was not made by the original property - property: SetPropText_complex", proxy.Text);
        }

        [Test]
        public void NotPassingBaseParameters()
        {
            var dummy =
                new Dummy();

            var proxy = Replace<Dummy, InterferenceObject>(
                (Action<string, DateTime, Dummy.EvenMore>) new Dummy().SetPropText_complex,
                null,
                (Func<string, Dummy, DateTime, string, Dummy.EvenMore, int>) new InterferenceObject().DontAddToPropText__MethodName_Return2);

            dummy.SetPropText_complex("text", DateTime.Now, new Dummy.EvenMore());
            Assert.Throws<NullReferenceException>(() => {
                proxy.SetPropText_complex("text", DateTime.Now, new Dummy.EvenMore());
            });
        }

        [Test]
        public void PassingInstanceAndMethodName()
        {
            var dummy =
                new Dummy();

            var proxy = Replace<Dummy, InterferenceObject>(
                (Action)new Dummy().SetPropText_simple,
                null,
                new Func<Dummy, string, int>(new InterferenceObject().SetPropText_InstanceAndMethodName_Return2));

            dummy.SetPropText_simple();
            proxy.SetPropText_simple();

            Assert.That(dummy.Text, Is.Not.Null.Or.Empty);
            Assert.AreEqual("simple", dummy.Text);

            Assert.That(proxy.Text, Is.Not.Null.Or.Empty);
            Assert.AreEqual(typeof(Dummy).Name + "Proxy+SetPropText_simple", proxy.Text);
        }
    }
}