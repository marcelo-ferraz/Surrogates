using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Surrogates.Utilities.Mixins;
using System.Reflection;
using System.Runtime.Serialization;

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

            public static IPropValidators Required(params string[] names)
            {
                return Required(ToProps<object>(names));
            }

            public static IPropValidators Required(params Func<T, object>[] props)
            {
                return PropertyAssertionMixins.Required(null, props);
            }

            public static IPropValidators Email(params string[] names)
            {
                return Email(ToProps<string>(names));
            }

            public static IPropValidators Email(params Func<T, string>[] props)
            {
                return Regex(Check.EmailRegexpr, props);
            }

            public static IPropValidators Url(params string[] names)
            {
                return Url(ToProps<string>(names));
            }

            public static IPropValidators Url(params Func<T, string>[] props)
            {
                return Regex(Check.UrlRegexpr, props);
            }

            public static IPropValidators Number(params string[] names)
            {
                return Number(ToProps<string>(names));
            }

            public static IPropValidators Number(params Func<T, string>[] props)
            {
                return Regex(Check.IsNumberRegexpr, props);
            }

            public static IPropValidators InBetween<P>(P min, P max, params string[] names)
                where P : struct
            {
                return InBetween<P>(min, max, ToProps<P>(names));
            }

            public static IPropValidators InBetween<P>(P min, P max, params Func<T, P>[] props)
                where P : struct
            {
                return PropertyAssertionMixins.InBetween(null, min, max, props);
            }

            public static IPropValidators BiggerThan<P>(P higher, params string[] names)
                where P : struct
            {
                return LowerThan(higher, ToProps<P>(names));
            }

            public static IPropValidators BiggerThan<P>(P higher, params Func<T, P>[] props)
                where P : struct
            {
                return PropertyAssertionMixins.BiggerThan(null, higher, props);
            }

            public static IPropValidators LowerThan<P>(P higher, params string[] names)
                where P : struct
            {
                return LowerThan(higher, ToProps<P>(names));
            }

            public static IPropValidators LowerThan<P>(P higher, params Func<T, P>[] props)
                where P : struct
            {
                return PropertyAssertionMixins.LowerThan(null, higher, props);
            }

            public static IPropValidators Regex(string expr, params string[] names)
            {
                return Regex(new Regex(expr), ToProps<string>(names));
            }

            public static IPropValidators Regex(Regex expr, params string[] names)
            {
                return Regex(expr, ToProps<string>(names));
            }

            public static IPropValidators Regex(string expr, params Func<T, string>[] props)
            {
                return Regex(new Regex(expr), props);
            }

            public static IPropValidators Regex(Regex expr, params Func<T, string>[] props)
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
