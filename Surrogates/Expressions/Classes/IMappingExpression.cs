using Surrogates.Expressions.Methods;

namespace Surrogates.Expressions.Classes
{
    public interface IMappingExpression<T> : IFlushTypes
    {
        MethodDisableExpression<T> Disable { get; }

        InterferenceExpression<T> Replace { get; }

        InterferenceExpression<T> Visit { get; }
    }
}
