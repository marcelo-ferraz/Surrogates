using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Surrogates.Tactics;

namespace Surrogates.Expressions
{
    public class DisableExpression<TBase>
      : InterferenceExpression<TBase, AndExpression<TBase>>
    {
        public DisableExpression(Strategy currentStrategy, Strategies strategies)
            : base(currentStrategy, strategies) { }
        public override AndExpression<TBase> Methods(params string[] methodNames)
        {
            var result = base.Methods(methodNames);
            Strategies.Add(CurrentStrategy);
            return result;
        }

        public override AndExpression<TBase> These(params Func<TBase, Delegate>[] methods)
        {
            var result = base.These(methods);
            Strategies.Add(CurrentStrategy);
            return result;
        }

        public override AndExpression<TBase> Properties(params string[] propNames)
        {
            var result = base.Properties(propNames);
            Strategies.Add(CurrentStrategy);
            return result;            
        }

        public override AndExpression<TBase> These(params Func<TBase, object>[] props)
        {
            var result = base.These(props);
            Strategies.Add(CurrentStrategy);
            return result;            
        }
    }
}
