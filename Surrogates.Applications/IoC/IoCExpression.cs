using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Surrogates.Expressions;
using Surrogates.Expressions.Accessors;
using Surrogates.Tactics;
using Surrogates.Utilities;
using Surrogates.Applications.Infrastructure;

namespace Surrogates.Applications.IoC
{
    public class IoCExpression<T>
    {
        private InterferenceExpression<T> _expression;
        
        internal IoCExpression(InterferenceExpression<T> expr)
        {
            _expression = expr;
        }

        private AndExpression<T> AddArgs(AndExpression<T> expr, object[] args)
        {
            if (args == null || args.Length < 1)
            { return expr; }

            var strat =
                Pass.Current(_expression);

            var newValues =
                strat.Properties.ToDictionary(p => p.Original.Name, p => args);

            var paramsProp = strat.NewProperties
                .FirstOrDefault(p => p.Name == "Params");

            var @params =
                paramsProp != null ?
                ((Dictionary<string, object[]>)paramsProp.DefaultValue).MergeLeft(newValues) :
                newValues;

            return expr
                .And
                .AddProperty<Dictionary<string, object[]>>("Params", @params);
        }

        public AndExpression<T> Implying(Type injType, params object[] args)
        {
            var interceptorType =
                typeof(IoCInterceptor4<>)
                .MakeGenericType(injType);

            var expr = _expression.Accessors(
                m => m
                    .Getter.Using(interceptorType, "Get")
                    .And
                    .Setter.Using(interceptorType, "Set"));

            return AddArgs(expr, args);
        }

        public AndExpression<T> Implying<TInj>(params object[] args)
        {
            var expr = _expression.Accessors(
                m => m
                    .Getter.Using<IoCInterceptor4<TInj>>(ioc => (Func<string, Dictionary<string, object[]>, TInj>)ioc.Get)
                    .And
                    .Setter.Using<IoCInterceptor4<TInj>>(ioc => (Action<TInj>)ioc.Set));
            
            return AddArgs(expr, args);
        }
    }
}
