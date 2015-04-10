using Surrogates.Tactics;
using System.Runtime.Serialization;

namespace Surrogates.Expressions
{
    public abstract class Expression<T>
    {
        internal virtual Strategies Strategies { get; set; }
        internal virtual T CurrentStrategy { get; set; }
        internal virtual BaseContainer4Surrogacy Container { get; set; }
    }

    public abstract class Expression<TBase, TStrat> : Expression<TStrat> 
    {
        protected Expression(BaseContainer4Surrogacy container, TStrat strategy, Strategies strategies)
        {
            this.Container = container;
            this.CurrentStrategy = strategy;
            this.Strategies = strategies;
        }

        protected T GetNotInit<T>()
        {
            return (T) FormatterServices.GetSafeUninitializedObject(typeof(T));
        }
    }
}
