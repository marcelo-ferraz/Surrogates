using Surrogates.Mappers.Entities;

namespace Surrogates.OldExpressions.Properties.Accessors
{
    public class AccessorAndExpression<TBase, TInstance>
        : Expression<TBase, TInstance>
    {
        private InterferenceKind _kind { get; set; }

        internal AccessorAndExpression(InterferenceKind kind, IMappingExpression<TBase> mapper, MappingState state)
            : base(mapper, state)
        {
            _kind = kind;
        }

        public AccessorChangeExpression<TBase> And
        {
            get { return new AccessorChangeExpression<TBase>(_kind, Mapper, State); }
        }
    }
}
