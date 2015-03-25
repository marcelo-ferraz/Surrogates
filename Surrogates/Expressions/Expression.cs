using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Surrogates.Tactics;

namespace Surrogates.Expressions
{
    public abstract class Expression<TBase, TStrat>
    {
        protected Strategies Strategies;
        protected virtual TStrat CurrentStrategy { get; set; }

        protected Expression(TStrat strategy, Strategies strategies)
        {
            this.CurrentStrategy = strategy;
        }

        protected T GetNotInit<T>()
        {
            var dummy =
                FormatterServices.GetSafeUninitializedObject(typeof(T));
            return __refvalue( __makeref(dummy),T);
        }
    }
}
