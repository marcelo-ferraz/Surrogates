using Surrogates.Expressions.Methods;

namespace Surrogates.Expressions.Classes
{
    public interface IMappingExpression<T> : IFlushTypes
    {
        MethodDisableExpression<T> Disable { get; }

        MethodInterferenceExpression<T> Substitute { get; }

        MethodInterferenceExpression<T> Visit { get; }
    }
}
