using Surrogates.Tactics;

namespace Surrogates.Expressions
{
    public class ExtensionExpression<TBase>
      : Expression<TBase, Strategy>
    {
        public Strategies Strategies { get; set; }
        public Strategy CurrentStrategy { get; set; }

        public ExtensionExpression(Strategy current, Strategies strategies)
            : base(current, strategies) { }        
    }
}
