using Surrogates.Tactics;

namespace Surrogates.Expressions
{
    public class ExpressionFactory<TBase> : Expression<TBase, Strategy>
    {
        internal ExpressionFactory(BaseContainer4Surrogacy container, Strategy current, Strategies strategies)
            : base(container, current, strategies)
        { }

        public ReplaceExpression<TBase> Replace
        {
            get
            {
                CurrentStrategy.Kind = InterferenceKind.Replace;
                return new ReplaceExpression<TBase>(Container, CurrentStrategy, Strategies);
            }
        }

        public DisableExpression<TBase> Disable
        {
            get
            {
                CurrentStrategy.Kind = InterferenceKind.Disable;
                return new DisableExpression<TBase>(Container, CurrentStrategy, Strategies);
            }
        }

        public VisitExpression<TBase> Visit
        {
            get
            {
                CurrentStrategy.Kind = InterferenceKind.Visit;
                return new VisitExpression<TBase>(Container, CurrentStrategy, Strategies);
            }
        }

        public ApplyExpression<TBase> Apply
        {
            get
            {
                CurrentStrategy.Kind = InterferenceKind.Extensions;
                return new ApplyExpression<TBase>(Container, Strategies, this);
            }
        }
    }
}
