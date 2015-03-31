﻿using NUnit.Framework;
using Surrogates.Expressions;
using Surrogates.Tactics;
using Surrogates.Tests.Scenarios.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace Surrogates.Tests.Unit
{
    [TestFixture]
    public class ExpressionTests : UnitTests
    {
        protected static T FirstStrategy<T>(AndExpression<Dummy> exp)
            where T : Strategy
        {
            var field = typeof(AndExpression<Dummy>)
                .GetField("Strategies", BindingFlags.Instance | BindingFlags.NonPublic);

            return FirstStrategy<T>((Strategies)field.GetValue(exp));
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

            var strategy = FirstStrategy<Strategy.ForMethods>(exp);

            Assert.AreEqual("SetPropText_simple", strategy.Methods[0].Name);
            Assert.AreEqual("GetTheNumberTwo", strategy.Methods[1].Name);
            Assert.AreEqual("AccomplishNothing", strategy.Interceptor.Method.Name);
            Assert.AreEqual(InterferenceKind.Replace, strategy.Kind);

            exp = GetExp()
                .From<Dummy>()
                .Replace
                .These(d => (Action) d.SetPropText_simple, d => (Func<int>) d.GetTheNumberTwo)
                .Using<InterferenceObject>(i => (Action) i.AccomplishNothing);

            strategy = FirstStrategy<Strategy.ForMethods>(exp);

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

            var strategy = FirstStrategy<Strategy.ForMethods>(exp);

            Assert.AreEqual("SetPropText_simple", strategy.Methods[0].Name);
            Assert.AreEqual("GetTheNumberTwo", strategy.Methods[1].Name);
            Assert.AreEqual("AccomplishNothing", strategy.Interceptor.Method.Name);
            Assert.AreEqual(InterferenceKind.Visit, strategy.Kind);

            exp = GetExp()
                .From<Dummy>()
                .Visit
                .These(d => (Action)d.SetPropText_simple, d => (Func<int>)d.GetTheNumberTwo)
                .Using<InterferenceObject>(i => (Action)i.AccomplishNothing);

            strategy = FirstStrategy<Strategy.ForMethods>(exp);

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

            var strategy = FirstStrategy<Strategy.ForMethods>(exp);

            Assert.AreEqual("SetPropText_simple", strategy.Methods[0].Name);
            Assert.AreEqual("GetTheNumberTwo", strategy.Methods[1].Name);
            Assert.IsNull(strategy.Interceptor);
            Assert.AreEqual(InterferenceKind.Disable, strategy.Kind);

            exp = GetExp()
                .From<Dummy>()
                .Disable
                .These(d => (Action)d.SetPropText_simple, d => (Func<int>)d.GetTheNumberTwo);

            strategy = FirstStrategy<Strategy.ForMethods>(exp);

            Assert.AreEqual("SetPropText_simple", strategy.Methods[0].Name);
            Assert.AreEqual("GetTheNumberTwo", strategy.Methods[1].Name);
            Assert.IsNull(strategy.Interceptor);
            Assert.AreEqual(InterferenceKind.Disable, strategy.Kind);
        }
    }
}
