using Surrogates.Expressions.Classes;
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

        /// <summary>
        /// Exposes the getter of that property that will be interfered
        /// </summary>
        public UsingInterferenceExpression<TBase> Getter
        {
            get { return new UsingInterferenceExpression<TBase>(Kind, PropertyAccessor.Get, Mapper, State); }
        }

        /// <summary>
        /// Exposes the setter of that property that will be interfered
        /// </summary>
        public UsingInterferenceExpression<TBase> Setter
        {
            get { return new UsingInterferenceExpression<TBase>(Kind, PropertyAccessor.Set, Mapper, State); }
        }

        /// <summary>
        /// Expose both getter and setter of that property that will be interfered
        /// </summary>
        public UsingInterferenceExpression<TBase> BothGetterAndSetter
        {
            get { return new UsingInterferenceExpression<TBase>(Kind, PropertyAccessor.Get | PropertyAccessor.Set, Mapper, State); }
        }
    }
}
