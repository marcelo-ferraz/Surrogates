using Surrogates.Expressions;
using Surrogates.Tactics;

namespace Surrogates.Utilities
{
    public static class Pass
    {
        public static IExtension<T> On<T>(ApplyExpression<T> baseExp, IExtension<T> to)
        {
            to.Container = baseExp.Container;
            to.Strategies = baseExp.Strategies;
            to.Factory = baseExp.Factory;
            
            return to;
        }

        public static T Current<T>(Expression<T> exp)
            where T : Strategy
        {
            return exp.CurrentStrategy;
        }
        public static T Current<T>(AndExpression<T> exp)
            where T : Strategy
        {
            return (T) exp.CurrentStrategy;
        }
    }
}
