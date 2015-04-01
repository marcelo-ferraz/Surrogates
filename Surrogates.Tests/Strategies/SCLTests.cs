using NUnit.Framework;
using Surrogates.Tactics;
using Surrogates.Tests.Expressions.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Surrogates.Tests.Strategies
{
    public class SCLTests : StrategiesTests
    {
        private MethodInfo _parseStrCmdMethod;
        
        [SetUp]
        protected void SetUp()
        {
            _parseStrCmdMethod = typeof(BaseContainer4Surrogacy)
                .GetMethod("ParseStrCmd", BindingFlags.Instance | BindingFlags.NonPublic);        
        }

        private Surrogates.Tactics.Strategies GetStrategies(string cmd, Type baseType, Type[] interceptors, string[] aliases)
        {
            try
            {
                return (Surrogates.Tactics.Strategies)_parseStrCmdMethod.Invoke(
                    new SurrogatesContainer(), new object[] { cmd, baseType, interceptors, aliases });
            }
            catch (TargetInvocationException e)
            {
                throw e.InnerException;
            }
        }

        [Test] 
        public void ReplaceCommand()
        {
            var aliases = (string[])
                Array.CreateInstance(typeof(string), 0);

            var strats = GetStrategies(
                "as d, i replace d.SetPropText_simple with i.AccomplishNothing",
                typeof(Dummy), 
                new[] { typeof(InterferenceObject) }, 
                aliases);

            var strategy = 
                FirstStrategy<Strategy.ForMethods>(strats);

            Assert.AreEqual("SetPropText_simple", strategy.Methods[0].Name);
            Assert.AreEqual("AccomplishNothing", strategy.Interceptor.Method.Name);
            Assert.AreEqual(InterferenceKind.Replace, strategy.Kind);
        }

        [Test]
        public void WrongCommand()
        {
            var aliases = (string[])
                Array.CreateInstance(typeof(string), 0);

            var strats = GetStrategies(
                "as d, i replace d.SetPropText_simple = i.AccomplishNothing",
                typeof(Dummy),
                new[] { typeof(InterferenceObject) },
                aliases);

            var strategy =
                FirstStrategy<Strategy.ForMethods>(strats);

            Assert.AreEqual("SetPropText_simple", strategy.Methods[0].Name);
            
            //will throw an exception when strategy parsed into a type
            Assert.IsNull(strategy.Interceptor);
            
            Assert.AreEqual(InterferenceKind.Replace, strategy.Kind);
        }

        [Test, ExpectedException(typeof(NotSupportedException))]
        public void NotSupportedCommand()
        {
            var aliases = (string[])
                Array.CreateInstance(typeof(string), 0);

            var strats = GetStrategies(
                "as d, i notSuported d.SetPropText_simple = i.AccomplishNothing",
                typeof(Dummy),
                new[] { typeof(InterferenceObject) },
                aliases);
        }

        [Test]
        public void DisableCommand()
        {
            var aliases = (string[])
                Array.CreateInstance(typeof(string), 0);

            var strats = GetStrategies(
                "as d disable d.SetPropText_simple",
                typeof(Dummy),
                new[] { typeof(InterferenceObject) },
                aliases);

            var strategy =
                FirstStrategy<Strategy.ForMethods>(strats);

            Assert.AreEqual("SetPropText_simple", strategy.Methods[0].Name);
            Assert.IsNull(strategy.Interceptor);
            Assert.AreEqual(InterferenceKind.Disable, strategy.Kind);
        }

        [Test]
        public void VisitCommand()
        {
            var aliases = (string[])
                Array.CreateInstance(typeof(string), 0);

            var strats = GetStrategies(
                "as d, i vISit d.SetPropText_simple with i.AccomplishNothing",
                typeof(Dummy),
                new[] { typeof(InterferenceObject) },
                aliases);

            var strategy =
                FirstStrategy<Strategy.ForMethods>(strats);

            Assert.AreEqual("SetPropText_simple", strategy.Methods[0].Name);
            Assert.AreEqual("AccomplishNothing", strategy.Interceptor.Method.Name);
            Assert.AreEqual(InterferenceKind.Visit, strategy.Kind);
        }
    }
}
