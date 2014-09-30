using Surrogates.Expressions.Classes;
using Surrogates.Expressions.Methods;
using Surrogates.Mappers.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;

namespace Surrogates.Mappers
{
    public class DefaultMapper : BaseMapper
    {
        public DefaultMapper(MappingState state)
            :base(state) { }

        public override IMappingExpression<T> Throughout<T>(string name = null)
        {
            return (IMappingExpression<T>)
                (MapExpression = new ClassMappingExpression<T>(name, State));
        }

        internal static string CreateName4<T>()
        {
            return CreateName4(typeof(T));
        }

        internal static string CreateName4(Type type)
        {
            return string.Concat(type.Name, "Proxy");
        }
    }
}