using Surrogates.Applications.ExecutingElsewhere;
using Surrogates.Expressions;
using Surrogates.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
namespace Surrogates.Applications
{
    public static class ExecuteElsewhereMixins
    {
        public static ElsewhereExpression<T> Call<T>(this ApplyExpression<T> self, params Func<T, Delegate>[] methods)
        {
            var ext =
                new ShallowExtension<T>();

            Pass.On<T>(self, ext);

            return new ElsewhereExpression<T>(ext.Factory.Replace.These(methods));
        }
    }
}
