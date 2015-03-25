using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Surrogates.Tactics;

namespace Surrogates.Expressions
{
    public class ApplyExpression<TBase>
      : Expression<TBase, Strategy>
    {
        public ApplyExpression(Strategy current, Strategies strategies)
            : base(current, strategies) { }
    }
}
