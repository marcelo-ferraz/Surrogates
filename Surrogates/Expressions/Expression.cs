using Surrogates.Expressions.Classes;
using Surrogates.Mappers;
using Surrogates.Mappers.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Surrogates.Expressions
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

        protected virtual FieldInfo GetField4<TSubstitute>()
        {
            if (PrivateField == null)
            {
                ushort fieldCount = 0;

                for (fieldCount = 0; fieldCount < State.Fields.Count; fieldCount++)
                {
                    if (State.Fields[fieldCount].FieldType == typeof(TSubstitute))
                    { return State.Fields[fieldCount]; }
                }

                string name = string.Concat(
                    "_interference_", fieldCount.ToString());

                PrivateField = State.TypeBuilder
                    .DefineField(name, typeof(TSubstitute), FieldAttributes.Private);

                State.Fields.Add(PrivateField);
            }

            return PrivateField;
        }
    }
}