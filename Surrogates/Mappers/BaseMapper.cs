using Surrogates.Expressions.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;

namespace Surrogates.Mappers
{
    public abstract class BaseMapper : IMapper
    {
        protected MappingState State;

        protected Type ConstructedType;

        public BaseMapper()
        {
            this.State = new MappingState();
        }

        public abstract Type Flush();

        public abstract IMappingExpression<T> Throughout<T>(string name = null);
    }
}
