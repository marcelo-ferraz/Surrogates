using Surrogates.Applications.ExecutingElsewhere;
using Surrogates.Expressions;
using Surrogates.Model.Entities;
using Surrogates.Utilities;
using System;

namespace Surrogates.Applications
{
    public static class ExecuteElsewhereMixins
    {
        public static ElsewhereExpression<T> Calls<T>(this ApplyExpression<T> self, Func<T, Delegate> method)
        {
            var ext =
                new ShallowExtension<T>();

            Pass.On<T>(self, ext);
            ext.Strategies.Accesses = ~Access.Container;

            return new ElsewhereExpression<T>(ext.Factory.Replace.This(method));
        }
    }
}