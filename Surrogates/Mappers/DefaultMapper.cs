using System;
using Surrogates.OldExpressions.Classes;
using Surrogates.Mappers.Entities;
using Surrogates.OldExpressions;

namespace Surrogates.Mappers
{
    public class OldDefaultMapper : BaseMapper
    {
        public OldDefaultMapper(MappingState state)
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

    public class DefaultExpressionMapper
    { 

    }
}