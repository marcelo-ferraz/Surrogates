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
    }
}
