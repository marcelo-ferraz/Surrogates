using Surrogates.Expressions.Methods;

namespace Surrogates.Expressions.Classes
{
    public interface IMappingExpression<T> : IFlushTypes
    {
        MethodDisableExpression<T> Disable { get; }

        MethodInterceptionExpression<T> Substitute { get; }

        MethodInterceptionExpression<T> Visit { get; }
    }
}
