using Surrogates.Expressions;
using Surrogates.Tactics;

namespace Surrogates.Utilities
{
    public static class Pass
    {
        public static void On<T>(ApplyExpression<T> baseExp, IExtension<T> ext)
        {
            ext.Strategies = baseExp.Strategies;
            ext.Factory = baseExp.Factory;
        }

        public static T Current<T>(Expression<T> exp)
            where T: Strategy
        {
            return exp.CurrentStrategy;
        }
    }
}
