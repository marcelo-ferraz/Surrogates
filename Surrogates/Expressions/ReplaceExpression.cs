using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Surrogates.Tactics;

namespace Surrogates.Expressions
{
    public class ReplaceExpression<TBase>
      : InterferenceExpression<TBase, Accessors.InterferenceExpression<TBase>, UsingInterferenceExpression<TBase>>
    {
        public ReplaceExpression(Strategy current, Strategies strategies)
            : base(current, strategies) { }
    }
}
