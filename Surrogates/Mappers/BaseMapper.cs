using System;
using Surrogates.Expressions.Classes;
using Surrogates.Mappers.Entities;

namespace Surrogates.Mappers
{
    public abstract class BaseMapper : IMapper
    {
        protected IFlushTypes MapExpression;

        protected MappingState State;

        protected Type ConstructedType;

        public BaseMapper(MappingState state)
        {
            this.State = state;
        }

        public Type Flush()
        {
            return MapExpression.Flush();
        }

        public abstract IMappingExpression<T> Throughout<T>(string name = null);
    }
}
