using Surrogates.Expressions.Classes;
using Surrogates.Mappers;
using Surrogates.Mappers.Entities;

namespace Surrogates.Expressions.Properties.Accessors
{
    public class AccessorChangeExpression<TBase>
        : Expression<TBase>
    {
        internal InterferenceKind Kind;
        
        public AccessorChangeExpression(InterferenceKind kind, IMappingExpression<TBase> mapper, MappingState state)
            : base(mapper, state)
        {
            this.Kind = kind;
        }

        public UsingInterferenceExpression<TBase> Getter
        {
            get { return new UsingInterferenceExpression<TBase>(Kind, PropertyAccessor.Get, Mapper, State); }
        }

        public UsingInterferenceExpression<TBase> Setter
        {
            get { return new UsingInterferenceExpression<TBase>(Kind, PropertyAccessor.Set, Mapper, State); }
        }

        public UsingInterferenceExpression<TBase> BothGetterAndSetter
        {
            get { return new UsingInterferenceExpression<TBase>(Kind, PropertyAccessor.Get | PropertyAccessor.Set, Mapper, State); }
        }
    }
}
