
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
    public class UnitTests
    {
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
