using System.Reflection;
using System.Runtime.Serialization;
using Surrogates.Mappers.Entities;

namespace Surrogates.OldExpressions
{
    public abstract class Expression<TBase>
        : Expression<TBase, TBase>
    {
        internal Expression(IMappingExpression<TBase> mapper)
            : base(mapper)
        { }
        
        internal Expression(IMappingExpression<TBase> mapper, MappingState state)
            : base(mapper, state)
        { }
    }

    public abstract class Expression<TBase, TInstance>
    {
        protected IMappingExpression<TBase> Mapper;
        internal MappingState State;
        protected TInstance NotInitializedInstance;
        protected FieldInfo PrivateField { get; set; }

        internal Expression(
            IMappingExpression<TBase> mapper)
            : this(mapper, null) { }

        internal Expression(
            IMappingExpression<TBase> mapper, MappingState state)
        {
            this.Mapper = mapper;
            this.State = state;
            this.NotInitializedInstance = (TInstance)
                FormatterServices.GetSafeUninitializedObject(typeof(TInstance));
        }

        protected virtual FieldInfo GetField4<TInterference>(string name = null)
        {
            if (PrivateField != null &&
                PrivateField.FieldType == typeof(TInterference))
            {
                return PrivateField;
            }

            return PrivateField =
                State.Fields.Get<TInterference>(name);
        }
    }
}