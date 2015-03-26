using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Surrogates.Tactics;
using Surrogates.Model;

namespace Surrogates.Expressions.Accessors
{
    public class ModifierExpression
    {
        internal Strategy.ForProperties Strategy { get; set; }
        
        public ModifierExpression(Strategy.ForProperties strategy)
        {
            this.Strategy = strategy;
        }

        public Accessors.UsingExpression Getter
        {
            get
            {
                return new Accessors.UsingExpression(Strategy, PropertyAccessor.Get);
            }
        }

        public Accessors.UsingExpression Setter
        {
            get
            {
                return new Accessors.UsingExpression(Strategy, PropertyAccessor.Set);
            }
        }

    }
}