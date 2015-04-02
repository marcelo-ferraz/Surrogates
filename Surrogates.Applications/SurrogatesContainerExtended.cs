using Surrogates.Expressions;
using Surrogates.Tactics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Surrogates.Applications
{
    public static class SurrogatesContainerExtended
    {
        public static AndExpression<T> ReadAndWrite<T>(this ExtensionExpression<T> exp, params Func<T, object>[] property)
        {
            var strat = new Strategy
                .ForProperties(exp.CurrentStrategy);

            strat.KindExtended = "interlocking";
            //strat.Properties.Add()
            //new AndExpression<T>(exp.Strategies);
            return null;
        }
    }
}
