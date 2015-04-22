using Surrogates.Applications.Validators;
using Surrogates.Expressions;
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
        public static AndExpression<T> Validators<T>(this ApplyExpression<T> that, params Func<Validators<T>, Func<T, bool>>[] validators )
        {
            throw new NotImplementedException("");

            Validators(null,
                v => v.Parameters. )
        }
    }
}
