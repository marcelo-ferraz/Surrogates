using Surrogates.Expressions;
using Surrogates.Tactics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Surrogates.Mappers
{
    public class DefaultExpressionMapper: IFlushTypes
    {
        public Tactics.Strategies Strategies { get; set; }

        public void Accept(NewExpression input, Strategies strategies)
        {
            Strategies = input.Strategies;
        }

        public Type Flush()
        {
            return Strategies.Apply();
        }
    }
}
