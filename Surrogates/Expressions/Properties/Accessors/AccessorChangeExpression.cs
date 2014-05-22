using Surrogates.Expressions.Classes;
using Surrogates.Mappers;

namespace Surrogates.Expressions.Properties.Accessors
{
    public class AccessorChangeExpression<TBase>
        : Expression<TBase>
    {
        private InterferenceKind _kind;
        
        public AccessorChangeExpression(InterferenceKind kind, IMappingExpression<TBase> mapper, MappingState state)
            : base(mapper, state)
        {
            this._kind = kind;
        }

        public UsingInterferenceExpression<TBase> Getter
        {
            get { return new UsingInterferenceExpression<TBase>(_kind, PropertyAccessor.Get, Mapper, State); }
        }

        public UsingInterferenceExpression<TBase> Setter
        {
            get { return new UsingInterferenceExpression<TBase>(_kind, PropertyAccessor.Set, Mapper, State); }
        }

        public UsingInterferenceExpression<TBase> BothGetterAndSetter
        {
            get { return new UsingInterferenceExpression<TBase>(_kind, PropertyAccessor.Get | PropertyAccessor.Set, Mapper, State); }
        }
    }
}
