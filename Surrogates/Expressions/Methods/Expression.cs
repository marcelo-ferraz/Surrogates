using Surrogates.Expressions.Classes;
using Surrogates.Mappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Surrogates.Expressions.Methods
{
    public abstract class Expression<TBase, TInstance>
    {
        protected IMappingExpression<TBase> Mapper;
        protected MappingState State;
        protected TInstance NotInitializedInstance;
        
        internal Expression(
            IMappingExpression<TBase> mapper, MappingState state)
        {
            this.Mapper = mapper;
            this.State = state;
            this.NotInitializedInstance = (TInstance)
                FormatterServices.GetSafeUninitializedObject(typeof(TInstance));
        }

        protected abstract void RegisterAction(Func<TInstance, Delegate> action);

        protected abstract void RegisterFunction(Func<TInstance, Delegate> function);
    }
}
