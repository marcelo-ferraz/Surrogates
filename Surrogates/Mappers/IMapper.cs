using Surrogates.Expressions.Classes;
using Surrogates.Expressions.Methods;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;

namespace Surrogates.Mappers
{
    public interface IMapper : IFlushTypes
    {
        IMappingExpression<T> Throughout<T>(string name = null);
    }
}