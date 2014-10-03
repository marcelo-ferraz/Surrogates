using System;
using Surrogates.Expressions.Classes;
using Surrogates.Mappers.Entities;

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