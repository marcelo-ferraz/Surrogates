using Surrogates.Applications.Contracts.Utilities;
using Surrogates.Utilities.Mixins;
using System;
using System.Text.RegularExpressions;

namespace Surrogates.Applications.Contracts
{       
    public static class PropertiesOf<T>
    {
        public class Validators
        {
            private static Func<T, P>[] ToProps<P>(params string[] names)
            {
                var props = (Func<T, P>[])
                    Array.CreateInstance(typeof(Func<T, object>), names.Length);

                for (int i = 0; i < names.Length; i++)
                {
                    props[i] = (Func<T, P>)Delegate.CreateDelegate(
                        typeof(T), typeof(T).GetProp4Surrogacy(names[i]).GetGetMethod());
                }

                return props;
            }

            public static IPropValidator Required(params string[] names)
            {
                return Required(ToProps<object>(names));
            }

            public static IPropValidator Required(params Func<T, object>[] props)
            {
                return PropertyAssertionMixins.Required(null, props);
            }

            public static IPropValidator Email(params string[] names)
            {
                return Email(ToProps<string>(names));
            }

            public static IPropValidator Email(params Func<T, string>[] props)
            {
                return Regex(Check.EmailRegexpr, props);
            }

            public static IPropValidator Url(params string[] names)
            {
                return Url(ToProps<string>(names));
            }

            public static IPropValidator Url(params Func<T, string>[] props)
            {
                return Regex(Check.UrlRegexpr, props);
            }

            public static IPropValidator Number(params string[] names)
            {
                return Number(ToProps<string>(names));
            }

            public static IPropValidator Number(params Func<T, string>[] props)
            {
                return Regex(Check.IsNumberRegexpr, props);
            }

            public static IPropValidator InBetween<P>(P min, P max, params string[] names)
                where P : struct
            {
                return InBetween<P>(min, max, ToProps<P>(names));
            }

            public static IPropValidator InBetween<P>(P min, P max, params Func<T, P>[] props)
                where P : struct
            {
                return PropertyAssertionMixins.InBetween(null, min, max, props);
            }

            public static IPropValidator BiggerThan<P>(P higher, params string[] names)
                where P : struct
            {
                return LowerThan(higher, ToProps<P>(names));
            }

            public static IPropValidator BiggerThan<P>(P higher, params Func<T, P>[] props)
                where P : struct
            {
                return PropertyAssertionMixins.BiggerThan(null, higher, props);
            }

            public static IPropValidator LowerThan<P>(P higher, params string[] names)
                where P : struct
            {
                return LowerThan(higher, ToProps<P>(names));
            }

            public static IPropValidator LowerThan<P>(P higher, params Func<T, P>[] props)
                where P : struct
            {
                return PropertyAssertionMixins.LowerThan(null, higher, props);
            }

            public static IPropValidator Regex(string expr, params string[] names)
            {
                return Regex(new Regex(expr), ToProps<string>(names));
            }

            public static IPropValidator Regex(Regex expr, params string[] names)
            {
                return Regex(expr, ToProps<string>(names));
            }

            public static IPropValidator Regex(string expr, params Func<T, string>[] props)
            {
                return Regex(new Regex(expr), props);
            }

            public static IPropValidator Regex(Regex expr, params Func<T, string>[] props)
            {
                return PropertyAssertionMixins.Regex(null, expr,  props);
            }
        }

        public static Validators Are { get; set; }

        static PropertiesOf()
        {
            Are = new PropertiesOf<T>.Validators();
        }
    }    
}
