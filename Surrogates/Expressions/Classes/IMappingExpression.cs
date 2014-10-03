using Surrogates.Expressions.Methods;

namespace Surrogates.Expressions.Classes
{
    /// <summary>
    /// Provides all the supported expressions to interverne a method or property
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IMappingExpression<T> : IFlushTypes
    {
        /// <summary>
        /// Starts a Disabling expression, which means that any property or method will return the default value and do not do a thing
        /// </summary>
        MethodDisableExpression<T> Disable { get; }

        /// <summary>
        /// Starts a Replacing expression, which means that any property or method will return the value of a provided method, if it is able to even box that value, if not, it will be returned a default value
        /// </summary>
        InterferenceExpression<T> Replace { get; }

        /// <summary>
        /// Starts a visitation expression, which means that any property or method will be visited by a provided method
        /// </summary>
        InterferenceExpression<T> Visit { get; }
    }
}
