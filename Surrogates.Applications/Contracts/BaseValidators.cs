using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Surrogates.Applications.Contracts
{
    public static class _Validate
    {
        internal static Regex EmailRegexpr;
        internal static Regex UrlRegexpr;
        internal static Regex IsNumberRegexpr;

        static _Validate()
        {
            EmailRegexpr = new Regex(
                @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z", RegexOptions.IgnoreCase | RegexOptions.Compiled);
            UrlRegexpr = new Regex(
                @"^(https?:\/\/)?([0-9a-zA-Z\.-]+)\.([0-9a-zA-Z\.]{2,6})(\:)?([\/\w \.-]*)*\/?", RegexOptions.Compiled);
            IsNumberRegexpr = new Regex(
                @"^([0-9]+)(.|,)?([0-9]*)$", RegexOptions.Compiled);
        }

        public static bool Regex(Regex expr, string arg)
        {
            return expr.IsMatch(arg);
        }

        public static bool LessThan<P>(P lower, P arg) 
            where P : struct
        {
            return Comparer<P>.Default.Compare(lower, arg) < 0;   
        }

        public static bool GreatterThan<P>(P higher, P arg) 
            where P : struct
        {
            return Comparer<P>.Default.Compare(higher, arg) > 0;
        }

        public static bool InBetween<P>(P min, P max, P arg) 
            where P : struct
        {
            return Comparer<P>.Default.Compare(min, arg) > 0 &&
                Comparer<P>.Default.Compare(max, arg) < 0;
        }

        public static bool Required(object val, Type t)
        {
            return // if is a reference type
                (!t.IsValueType && Nullable.GetUnderlyingType(t) != null && val == null)
                || // if is anything else
                (val == Activator.CreateInstance(t));
        }

        public static bool NotNullString(object val)
        {
            return !string.IsNullOrEmpty((string)val);
        }
    }
}
