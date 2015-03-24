using Surrogates.Expressions;
using Surrogates.Tactics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Surrogates.Mappers
{
    public class DefaultExpressionMapper: IMapper<NewExpression>
    {
        private Strategies _strategies;

        public Tactics.Strategies Strategies { get; set; }

        public void Accept<T>(NewExpression input, params Type[] interceptors)
        {
            throw new NotImplementedException();
        }

        public Type Flush()
        {
            return _strategies.Apply();
        }
    }
}
