using NUnit.Framework;
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

namespace Surrogates.Tests.Expressions
{
    [TestFixture]
    public class ExpressionTests
    {
        protected T GetStrategy<T>(object exp)
            where T : Strategy
        {
            var field = typeof(AndExpression<Dummy>)
                .GetField("<CurrentStrategy>k__BackingField", BindingFlags.Instance | BindingFlags.NonPublic);

            return (T)field.GetValue(exp);
        }

        protected NewExpression GetExp()
        {
            var modField = typeof(SurrogatesContainer)
                .GetField("ModuleBuilder", BindingFlags.Instance | BindingFlags.NonPublic);

            var mod = (ModuleBuilder) modField
                .GetValue(new SurrogatesContainer());
          
            return new NewExpression(mod);
        }

        [Test]
        public void ReplaceExpressionsTests() 
        {
            var exp = GetExp()
                .From<Dummy>()
                .Replace
                .Methods("SetPropText_simple", "GetTheNumberTwo")
                .Using<InterferenceObject>("AccomplishNothing");

            var strategy = GetStrategy<Strategy.ForMethods>(exp);

            Assert.AreEqual("SetPropText_simple", strategy.Methods[0].Name);
            Assert.AreEqual("GetTheNumberTwo", strategy.Methods[1].Name);
            Assert.AreEqual("AccomplishNothing", strategy.Interceptor.Method.Name);
            Assert.AreEqual(InterferenceKind.Replace, strategy.Kind);

            exp = GetExp()
                .From<Dummy>()
                .Replace
                .These(d => (Action) d.SetPropText_simple, d => (Func<int>) d.GetTheNumberTwo)
                .Using<InterferenceObject>("AccomplishNothing");

            strategy = GetStrategy<Strategy.ForMethods>(exp);

            Assert.AreEqual("SetPropText_simple", strategy.Methods[0].Name);
            Assert.AreEqual("GetTheNumberTwo", strategy.Methods[1].Name);
            Assert.AreEqual("AccomplishNothing", strategy.Interceptor.Method.Name);
            Assert.AreEqual(InterferenceKind.Replace, strategy.Kind);
        }
    }
}
