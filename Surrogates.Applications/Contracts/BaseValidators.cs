using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using Surrogates.Applications.Mixins;

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

        public static bool Less<P>(P less, P arg)
            where P : struct
        {
            return Comparer<P>.Default.Compare(less, arg) < 0;
        }

        public static bool LessOrEqual<P>(P less, P arg)
            where P : struct
        {
            return Comparer<P>.Default.Compare(less, arg) < 0 || object.Equals(less, arg);
        }

        public static bool GreaterOrEqual<P>(P higher, P arg)
            where P : struct
        {
            return Comparer<P>.Default.Compare(higher, arg) > 0 || object.Equals(higher, arg);
        }

        public static bool Greater<P>(P higher, P arg)
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

        public static bool IsNotNullOrDefault(object val, Type t)
        {
            return // if is a reference type
                (!t.IsValueType && Nullable.GetUnderlyingType(t) != null && val == null)
                || // if is anything else
                (val == Activator.CreateInstance(t));
        }

        internal static Func<object[], bool> Contains(int i, ParameterInfo[] @params, object expected)
        {
            var pType =
                @params[i].ParameterType;

            if (pType.Is<IList>())
            {
                return args =>
                    ((IList)args[i]).Contains(expected);
            }

            if (pType.Is<IDictionary>())
            {
                return args =>
                    ((IDictionary)args[i]).Contains(expected);
            }

            if (pType.Is(typeof(ICollection<>)) ||
                pType.Is(typeof(IDictionary<,>)))
            {
                var contains =
                    new Func<object, object[], object>(pType.GetMethod("Contains").Invoke);

                return args =>
                    (bool)contains(args[i], new object[] { expected });
            }

            if (pType.Is<string>())
            {
                if (expected.GetType().Is<string>())
                {
                    return
                        args =>
                            !string.IsNullOrEmpty((string)args[i]) && ((string)args[i]).Contains((char)expected);
                }

                if (expected.GetType().Is<char>())
                {
                    return args =>
                        !string.IsNullOrEmpty((string)args[i]) && ((string)args[i]).Contains((string)expected);
                }
            }

            //Tries with Linq's extension
            if (pType.Is(typeof(IEnumerable<>)))
            {
                var genParams =
                    pType.GetGenericArguments();

                var containsType = typeof(Func<,,>)
                    .MakeGenericType(genParams[0], pType, typeof(bool));

                var contains = (Delegate)Activator.CreateInstance(
                    containsType,
                    typeof(Enumerable).GetMethod("Contains", new Type[] { pType, genParams[0] }));

                return args => (bool)contains.DynamicInvoke(args[i], expected);
            }

            throw new NotSupportedException(
                string.Format("The parameter given, '{0}' is not a supported collection or array.", @params[i].Name));
        }
    }
}