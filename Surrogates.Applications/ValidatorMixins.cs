using Surrogates.Applications.Validators;
using Surrogates.Expressions;
using Surrogates.Utilities;
using Surrogates.Utilities.Mixins;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Surrogates.Applications
{
    public static class ValidatorMixins
    {
        public class ValidatorInterceptor
        { 

        }

        public static AndExpression<T> Validators<T>(this ApplyExpression<T> that, params Func<Validators<T>, IDictionary<string, Func<object[], bool>>>[] validators)
        {
            var ext = 
                new ShallowExtension<T>();

            Pass.On(that, to: ext);

            ext.Factory.Replace
                .This

            throw new Exception();
        }
    }
}
