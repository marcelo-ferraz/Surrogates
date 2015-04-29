using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Surrogates.Applications.Validators
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
@"_^(?:(?:https?|ftp)://)(?:\S+(?::\S*)?@)?(?:(?!10(?:\.\d{1,3}){3})(?!127(?:\.\d{1,3}){3})(?!169\.254(?:\.\d{1,3}){2})(?!192\.168(?:\.\d{1,3}){2})(?!172\.(?:1[6-9]|2\d|3[0-1])(?:\.\d{1,3}){2})(?:[1-9]\d?|1\d\d|2[01]\d|22[0-3])(?:\.(?:1?\d{1,2}|2[0-4]\d|25[0-5])){2}(?:\.(?:[1-9]\d?|1\d\d|2[0-4]\d|25[0-4]))|(?:(?:[a-z\x{00a1}-\x{ffff}0-9]+-?)*[a-z\x{00a1}-\x{ffff}0-9]+)(?:\.(?:[a-z\x{00a1}-\x{ffff}0-9]+-?)*[a-z\x{00a1}-\x{ffff}0-9]+)*(?:\.(?:[a-z\x{00a1}-\x{ffff}]{2,})))(?::\d{2,5})?(?:/[^\s]*)?$_iuS", RegexOptions.Compiled);
            IsNumberRegexpr = new Regex(
@"^([0-9]+)(.|,)?([0-9]*)$", RegexOptions.Compiled);
        }

        public static void ValidateRegex(Regex expr, string arg)
        {
            if (!expr.IsMatch(arg))
            { throw new Exception("Not Valid"); }
        }

        public static void ValidateLowerThan<P>(P lower, P arg) 
            where P : struct
        {
            if ((Comparer<P>.Default.Compare(lower, arg) < 0))
            { throw new Exception("Not in between"); }            
        }

        public static void ValidateBiggerThan<P>(P higher, P arg) 
            where P : struct
        {
            if (Comparer<P>.Default.Compare(higher, arg) < 0)
            {
                throw new Exception("Not in between");
            }            
        }

        public static void ValidateInBetween<P>(P min, P max, P arg) 
            where P : struct
        {
            if (Comparer<P>.Default.Compare(min, arg) < 0 &&
                Comparer<P>.Default.Compare(max, arg) > 0)
            {
                throw new Exception("Not in between");
            }
        }

        public static void ValidateRequired(object val, Type t)
        {
            if (!t.IsValueType &&
                Nullable.GetUnderlyingType(t) != null &&
                val == null)
            { throw new Exception("Required!"); }
            else if (val == Activator.CreateInstance(t))
            { throw new Exception("Required!"); }
        }

        public static void ValidateRequiredString(object val)
        {
            if (string.IsNullOrEmpty((string)val))
            { throw new Exception("Required!"); }
        }

    }
}
