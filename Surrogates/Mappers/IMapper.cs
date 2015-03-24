using Surrogates.OldExpressions;
using Surrogates.Tactics;
using System;

namespace Surrogates.Mappers
{
    public interface IOldMapper : IFlushTypes
    {
        /// <summary>
        /// Exposes a given type to be intervened
        /// </summary>
        /// <typeparam name="T">The type to be exposed</typeparam>
        /// <param name="name">The name of this map</param>
        /// <returns></returns>
        IMappingExpression<T> Throughout<T>(string name = null);
    }

    public interface IMapper<TInput>
    {
        public Strategies Strategies { get; set; }
        void Accept<T>(TInput input, params Type[] interceptors);

        Type Flush();
    }
}