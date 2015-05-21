using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Surrogates.Applications.Contracts
{
    internal static class BaseValidators
    {
        public static Regex EmailRegexpr;
        public static Regex UrlRegexpr;
        public static Regex IsNumberRegexpr;

        static BaseValidators()
        {
            EmailRegexpr = new Regex(
                @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z", RegexOptions.IgnoreCase | RegexOptions.Compiled);
            UrlRegexpr = new Regex(
                @"^(https?:\/\/)?([0-9a-zA-Z\.-]+)\.([0-9a-zA-Z\.]{2,6})(\:)?([\/\w \.-]*)*\/?", RegexOptions.Compiled);
            IsNumberRegexpr = new Regex(
                @"^([0-9]+)(.|,)?([0-9]*)$", RegexOptions.Compiled);
        }

        public static bool ValidateRegex(Regex expr, string arg)
        {
            return expr.IsMatch(arg);
        }

        public static bool ValidateLowerThan<P>(P lower, P arg) 
            where P : struct
        {
            return Comparer<P>.Default.Compare(lower, arg) < 0;   
        }

        public static bool ValidateBiggerThan<P>(P higher, P arg) 
            where P : struct
        {
            return Comparer<P>.Default.Compare(higher, arg) > 0;
        }

        public static bool ValidateInBetween<P>(P min, P max, P arg) 
            where P : struct
        {
            return Comparer<P>.Default.Compare(min, arg) > 0 &&
                Comparer<P>.Default.Compare(max, arg) < 0;
        }

        public static bool ValidateRequired(object val, Type t)
        {
            return // if is a reference type
                (!t.IsValueType && Nullable.GetUnderlyingType(t) != null && val == null)
                || // if is anything else
                (val == Activator.CreateInstance(t));
        }

        public static bool ValidateRequiredString(object val)
        {
            return !string.IsNullOrEmpty((string)val);
        }
    }
}
