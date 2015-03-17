using System;
using Surrogates.Mappers.Entities;
using Surrogates.Expressions;

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
