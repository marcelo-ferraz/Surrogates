using Surrogates.Expressions;
using Surrogates.Tactics;
using System;

namespace Surrogates.Utilities
{
    public static class InternalsInspector
    {
        public static IExtension<T> GetInternals<T, TExt>(ApplyExpression<T> baseExp)
            where TExt : IExtension<T>
        {
            var ext = (IExtension<T>) Activator.CreateInstance(typeof(TExt));

            return InternalsInspector.GetInternals(baseExp, ext);
        }

        public static IExtension<T> GetInternals<T>(ApplyExpression<T> baseExp, IExtension<T> to)
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

        public static Strategy.ForProperties Current<T>(Expression<T, Strategy.ForProperties> expr)
        {
            return expr.CurrentStrategy;
        }

        public static Strategy.ForMethods Current<T>(Expression<T, Strategy.ForMethods> expr)
        {
            return expr.CurrentStrategy;
        }
    }
}
