using Surrogates.Tactics;

namespace Surrogates.Expressions
{
    public class ApplyExpression<TBase>      
    {
        internal ExpressionFactory<TBase> Factory { get; set; }
        internal Strategies Strategies { get; set; }

        public ApplyExpression(Strategies strategies, ExpressionFactory<TBase> factory)            
        {
            this.Factory = factory;
        }
    }    
}
