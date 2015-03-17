using Surrogates.Expressions;

namespace Surrogates.Mappers
{
    public interface IMapper : IFlushTypes
    {
        /// <summary>
        /// Exposes a given type to be intervened
        /// </summary>
        /// <typeparam name="T">The type to be exposed</typeparam>
        /// <param name="name">The name of this map</param>
        /// <returns></returns>
        IMappingExpression<T> Throughout<T>(string name = null);
    }
}