using Surrogates.Expressions.Classes;
using Surrogates.Mappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Surrogates.Expressions.Methods
{
    public class AndExpression<TBase>
    {
        private IMappingExpression<TBase> Mapper;

        public AndExpression(IMappingExpression<TBase> mapper)
        {
            this.Mapper = mapper;
        }

        public IMappingExpression<TBase> And { get { return Mapper; } }
    }
}
