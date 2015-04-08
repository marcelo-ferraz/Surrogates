using NUnit.Framework;
using Surrogates.Expressions;
using Surrogates.Tactics;
using Surrogates.Tests.Expressions.Entities;
using System;
using System.Reflection;
using System.Reflection.Emit;

namespace Surrogates.Tests.Strategies
{
    [TestFixture]
    public class ExpressionTests : StrategiesTests
    {
        protected static T First<T>(AndExpression<Dummy> exp)
            where T : Strategy
        {
            var prop = typeof(AndExpression<Dummy>)
                .GetProperty("Strategies", BindingFlags.Instance | BindingFlags.NonPublic);

            return FirstStrategy<T>((Surrogates.Tactics.Strategies)prop.GetValue(exp));
        }

        protected NewExpression GetExp()
        {
            var modField = typeof(SurrogatesContainer)
                .GetField("ModuleBuilder", BindingFlags.Instance | BindingFlags.NonPublic);

            var mod = (ModuleBuilder)modField
                .GetValue(new SurrogatesContainer());

            return new NewExpression(mod);
        }

        [Test]
        public void ReplaceTests() 
        {
            var exp = GetExp()
                .From<Dummy>()
                .Replace
                .Methods("SetPropText_simple", "GetTheNumberTwo")
                .Using<InterferenceObject>("AccomplishNothing");

            var strategy = First<Strategy.ForMethods>(exp);

            Assert.AreEqual("SetPropText_simple", strategy.Methods[0].Name);
            Assert.AreEqual("GetTheNumberTwo", strategy.Methods[1].Name);
            Assert.AreEqual("AccomplishNothing", strategy.Interceptor.Method.Name);
            Assert.AreEqual(InterferenceKind.Replace, strategy.Kind);

            exp = GetExp()
                .From<Dummy>()
                .Replace
                .These(d => (Action) d.SetPropText_simple, d => (Func<int>) d.GetTheNumberTwo)
                .Using<InterferenceObject>(i => (Action) i.AccomplishNothing);

            strategy = First<Strategy.ForMethods>(exp);

            Assert.AreEqual("SetPropText_simple", strategy.Methods[0].Name);
            Assert.AreEqual("GetTheNumberTwo", strategy.Methods[1].Name);
            Assert.AreEqual("AccomplishNothing", strategy.Interceptor.Method.Name);
            Assert.AreEqual(InterferenceKind.Replace, strategy.Kind);
        }

        [Test]
        public void VisitTests()
        {
            var exp = GetExp()
                .From<Dummy>()
                .Visit
                .Methods("SetPropText_simple", "GetTheNumberTwo")
                .Using<InterferenceObject>("AccomplishNothing");

            var strategy = First<Strategy.ForMethods>(exp);

            Assert.AreEqual("SetPropText_simple", strategy.Methods[0].Name);
            Assert.AreEqual("GetTheNumberTwo", strategy.Methods[1].Name);
            Assert.AreEqual("AccomplishNothing", strategy.Interceptor.Method.Name);
            Assert.AreEqual(InterferenceKind.Visit, strategy.Kind);

            exp = GetExp()
                .From<Dummy>()
                .Visit
                .These(d => (Action)d.SetPropText_simple, d => (Func<int>)d.GetTheNumberTwo)
                .Using<InterferenceObject>(i => (Action)i.AccomplishNothing);

            strategy = First<Strategy.ForMethods>(exp);

            Assert.AreEqual("SetPropText_simple", strategy.Methods[0].Name);
            Assert.AreEqual("GetTheNumberTwo", strategy.Methods[1].Name);
            Assert.AreEqual("AccomplishNothing", strategy.Interceptor.Method.Name);
            Assert.AreEqual(InterferenceKind.Visit, strategy.Kind);
        }

        [Test]
        public void DisableTests()
        {
            var exp = GetExp()
                .From<Dummy>()
                .Disable
                .Methods("SetPropText_simple", "GetTheNumberTwo");

            var strategy = First<Strategy.ForMethods>(exp);

            Assert.AreEqual("SetPropText_simple", strategy.Methods[0].Name);
            Assert.AreEqual("GetTheNumberTwo", strategy.Methods[1].Name);
            Assert.IsNull(strategy.Interceptor);
            Assert.AreEqual(InterferenceKind.Disable, strategy.Kind);

            exp = GetExp()
                .From<Dummy>()
                .Disable
                .These(d => (Action)d.SetPropText_simple, d => (Func<int>)d.GetTheNumberTwo);

            strategy = First<Strategy.ForMethods>(exp);

            Assert.AreEqual("SetPropText_simple", strategy.Methods[0].Name);
            Assert.AreEqual("GetTheNumberTwo", strategy.Methods[1].Name);
            Assert.IsNull(strategy.Interceptor);
            Assert.AreEqual(InterferenceKind.Disable, strategy.Kind);
        }
    }
}
