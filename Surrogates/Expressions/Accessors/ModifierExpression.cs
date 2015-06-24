using Surrogates.Tactics;

namespace Surrogates.Expressions.Accessors
{
    public class ModifierExpression
    {
        internal Strategy.ForProperties Strategy { get; set; }
        
        public ModifierExpression(Strategy.ForProperties strategy)
        {
            this.Strategy = strategy;
        }

        /// <summary>
        /// Relevates the getter accessor to be changed
        /// </summary>
        public Accessors.UsingExpression Getter
        {
            get
            {
                return new Accessors.UsingExpression(Strategy, PropertyAccessor.Get);
            }
        }

        /// <summary>
        /// Relevates the setter accessor to be changed
        /// </summary>
        public Accessors.UsingExpression Setter
        {
            get
            {
                return new Accessors.UsingExpression(Strategy, PropertyAccessor.Set);
            }
        }

    }
}