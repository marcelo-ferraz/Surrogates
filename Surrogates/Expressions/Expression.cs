using Surrogates.Tactics;
using System.Runtime.Serialization;

namespace Surrogates.Expressions
{
    public abstract class Expression<TBase, TStrat>
    {
        protected Strategies Strategies;
        protected virtual TStrat CurrentStrategy { get; set; }

        protected Expression(TStrat strategy, Strategies strategies)
        {
            this.CurrentStrategy = strategy;
            this.Strategies = strategies;
        }

        protected T GetNotInit<T>()
        {
            return (T) FormatterServices.GetSafeUninitializedObject(typeof(T));
        }
    }
}
