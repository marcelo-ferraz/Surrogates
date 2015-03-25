using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Surrogates.Tactics;

namespace Surrogates.Expressions.Accessors
{
    public class ModifierExpression
    {
        private Strategy.ForProperties _strategy;

        public ModifierExpression(Strategy.ForProperties strategy)
        {
            this._strategy = strategy;
        }

        public Accessors.WithExpression Getter
        {
            get
            {
                return new Accessors.WithExpression(_strategy, PropertyAccessor.Get);
            }
        }

        public Accessors.WithExpression Setter
        {
            get
            {
                return new Accessors.WithExpression(_strategy, PropertyAccessor.Set);
            }
        }
    }
}