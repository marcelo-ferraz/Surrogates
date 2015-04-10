using Surrogates.Tactics;

namespace Surrogates.Expressions
{
    public class ApplyExpression<TBase>      
    {
        internal BaseContainer4Surrogacy Container { get; set; }        
        internal ExpressionFactory<TBase> Factory { get; set; }
        internal Strategies Strategies { get; set; }

        public ApplyExpression(BaseContainer4Surrogacy container, Strategies strategies, ExpressionFactory<TBase> factory)            
        {
            this.Factory = factory;
            this.Strategies = strategies;
            this.Container = container;
        }
    }    
}
