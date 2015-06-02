using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using Surrogates.Applications.Mixins;

namespace Surrogates.Applications.Contracts.Utilities
{
    public static class Check
    {
        internal static Regex EmailRegexpr;
        internal static Regex UrlRegexpr;
        internal static Regex IsNumberRegexpr;

        static Check()
        {
            EmailRegexpr = new Regex(
                @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z", RegexOptions.IgnoreCase | RegexOptions.Compiled);
            UrlRegexpr = new Regex(
                @"^(https?:\/\/)?([0-9a-zA-Z\.-]+)\.([0-9a-zA-Z\.]{2,6})(\:)?([\/\w \.-]*)*\/?", RegexOptions.Compiled);
            IsNumberRegexpr = new Regex(
                @"^([0-9]+)(.|,)?([0-9]*)$", RegexOptions.Compiled);
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
                            !string.IsNullOrEmpty((string)args[i]) && ((string)args[i]).Contains((string)expected);
                }

                if (expected.GetType().Is<char>())
                {
                    return args =>
                        !string.IsNullOrEmpty((string)args[i]) && ((string)args[i]).Contains((char)expected);
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

        /// <summary>
        /// Checks if a regex is satisfied by an argument
        /// </summary>
        /// <param name="expr">A expression</param>
        /// <param name="arg">argument</param>
        /// <returns></returns>
        public static bool Regex(string expr, string arg)
        {
            return new Regex(expr).IsMatch(arg);
        }

        /// <summary>
        /// Checks if a regex is satisfied by an argument
        /// </summary>
        /// <param name="expr">A expression</param>
        /// <param name="arg">argument</param>
        /// <returns></returns>
        public static bool Regex(Regex expr, string arg)
        {
            return expr.IsMatch(arg);
        }

        /// <summary>
        /// Checks if a given value is less than a value expected
        /// </summary>
        /// <typeparam name="P">Type of the value</typeparam>
        /// <param name="expected">The expected value</param>
        /// <param name="arg">The provided argument</param>
        /// <returns></returns>
        public static bool Less<P>(P expected, P arg)
            where P : struct
        {
            return Comparer<P>.Default.Compare(expected, arg) < 0;
        }

        /// <summary>
        /// Checks if a given value is less than, or equal to, a value expected
        /// </summary>
        /// <typeparam name="P">Type of the value</typeparam>
        /// <param name="expected">The expected value</param>
        /// <param name="arg">The provided argument</param>
        /// <returns></returns>
        public static bool LessOrEqual<P>(P expected, P arg)
            where P : struct
        {
            return Comparer<P>.Default.Compare(expected, arg) < 0 || object.Equals(expected, arg);
        }

        /// <summary>
        /// Checks if a given value is greater than, or equal to, a value expected
        /// </summary>
        /// <typeparam name="P">Type of the value</typeparam>
        /// <param name="expected">The expected value</param>
        /// <param name="arg">The provided argument</param>
        /// <returns></returns>
        public static bool GreaterOrEqual<P>(P expected, P arg)
            where P : struct
        {
            return Comparer<P>.Default.Compare(expected, arg) > 0 || object.Equals(expected, arg);
        }

        /// <summary>
        /// Checks if a given value is greater than a value expected
        /// </summary>
        /// <typeparam name="P">Type of the value</typeparam>
        /// <param name="expected">The expected value</param>
        /// <param name="arg">The provided argument</param>
        /// <returns></returns>
        public static bool Greater<P>(P expected, P arg)
            where P : struct
        {
            return Comparer<P>.Default.Compare(expected, arg) > 0;
        }

        /// <summary>
        /// Checks if a given value is in between a minimum and a maximum
        /// </summary>
        /// <typeparam name="P">Type of the value</typeparam>
        /// <param name="min">A lower bound</param>
        /// <param name="max">A higher bound</param>
        /// <param name="arg">The provided argument</param>
        /// <returns></returns>
        public static bool InBetween<P>(P min, P max, P arg)
            where P : struct
        {
            return Comparer<P>.Default.Compare(min, arg) > 0 &&
                Comparer<P>.Default.Compare(max, arg) < 0;
        }

        /// <summary>
        /// Checks if the supplied value is not null, for a reference type, or default, for a value type
        /// </summary>
        /// <param name="arg">The provided argument</param>
        /// <param name="t">The type</param>
        /// <returns></returns>
        public static bool IsNotNullOrDefault(object arg, Type t)
        {
            return // if is a reference type
                (!t.IsValueType && Nullable.GetUnderlyingType(t) != null && arg == null)
                || // if is anything else
                (arg == Activator.CreateInstance(t));
        }

    }
}