
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

namespace Surrogates.Tests.Unit
{
    public class UnitTests<T> : UnitTests
        where T : Strategy
    {
        protected T Strategy;
        protected Strategies Strategies;
        
        [SetUp]
        public void SetUp()
        {
            Strategies =
                CreateStrategies4<Dummy>();

            Strategy = (T)Activator.CreateInstance(typeof(T), new object[] { Strategies });

            Strategies.Add(Strategy);
        }   
    }

    public class UnitTests
    {
        protected Strategies CreateStrategies4<T>(string name = null)
        {
            if (string.IsNullOrEmpty(name))
            { name = typeof(T).Name + "Proxy"; }

            var modField = typeof(SurrogatesContainer)
                .GetField("ModuleBuilder", BindingFlags.Instance | BindingFlags.NonPublic);

            var mod = (ModuleBuilder)modField
                .GetValue(new SurrogatesContainer());

            var ctr = typeof(Strategies).GetConstructor(
                BindingFlags.Instance | BindingFlags.NonPublic, 
                null, 
                new Type[] {typeof(Type), typeof(string), typeof(ModuleBuilder)},
                null);

            return (Strategies) ctr.Invoke(new object[] { typeof(T), name, mod });
        }

        protected static T FirstStrategy<T>(Strategies strats)
            where T : Strategy
        {
            return (T)GetStrategies(strats)[0];
        }

        protected static IList<Strategy> GetStrategies(Strategies strats)
        {
            var stratsField = typeof(Strategies)
                .GetField("_strategies", BindingFlags.Instance | BindingFlags.NonPublic);

            var strategies = (IList<Strategy>)
                stratsField.GetValue(strats);
            return strategies;
        }
    }
}
