using Surrogates.Applications.Contracts;
using Surrogates.Applications.Contracts.Collections;
using Surrogates.Applications.Contracts.Model;
using Surrogates.Applications.Contracts.Utilities;
using Surrogates.Utilities.Mixins;
using System;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;

namespace Surrogates.Applications
{
    [Obsolete("This is still not done")]
    internal static class PropertyAssertionMixins
    {
        private static Func<T, P>[] ToProps<T, P>(params string[] names)
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

        private static IPropValidator Validate<T, P>(IPropValidator assertions, Func<T, P>[] props, Func<Func<T, P>, Action<T>> validator)
        {
            var obj = (T)
                FormatterServices.GetUninitializedObject(typeof(T));

            var ass = (AssertionList4Properties)(assertions ?? (assertions = new AssertionList4Properties()));

            for (int i = 0; i < props.Length; i++)
            {
                var validate =
                    validator(props[i]);

                if (validate == null)
                { continue; }

                ass.Validators
                    .Add(new AssertionEntry4Properties
                    {
                        Property = props[i],
                        Validation = validator(props[i])
                    });
            }

            return assertions;
        }

        public static IPropValidator Required<T>(this IPropValidator self, params string[] names)
        {
            return Required(self, ToProps<T, object>(names));
        }

        public static IPropValidator Required<T>(this IPropValidator self, params Func<T, object>[] props)
        {
            return Validate<T, object>(self, props, 
                getProp =>
                {
                    if (getProp.Method.ReturnType == typeof(string))
                    {
                        return item => { };
                            //{ return !string.IsNullOrEmpty((string)getProp(item)); };
                    }
                    else
                    {
                        return item =>
                            Check.IsNotNullOrDefault(getProp(item), getProp(item).GetType());
                    }
                });
        }

        public static IPropValidator Email<T>(this IPropValidator self, params string[] names)
        {
            return Email<T>(self, ToProps<T, string>(names));
        }

        public static IPropValidator Email<T>(this IPropValidator self, params Func<T, string>[] props)
        {
            return Regex<T>(self, Check.EmailRegexpr, props);
        }

        public static IPropValidator Url<T>(this IPropValidator self, params string[] names)
        {
            return Url<T>(self, ToProps<T, string>(names));
        }

        public static IPropValidator Url<T>(this IPropValidator self, params Func<T, string>[] props)
        {
            return Regex<T>(self, Check.UrlRegexpr, props);
        }

        public static IPropValidator IsNumber<T>(this IPropValidator self, params string[] names)
        {
            return IsNumber<T>(self, ToProps<T, string>(names));
        }

        public static IPropValidator IsNumber<T>(this IPropValidator self, params Func<T, string>[] props)
        {
            return Regex<T>(self, Check.IsNumberRegexpr, props);
        }

        public static IPropValidator InBetween<T, P>(this IPropValidator self, P min, P max, params string[] names)
            where P : struct
        {
            return InBetween<T, P>(self, min, max, ToProps<T, P>(names));
        }

        public static IPropValidator InBetween<T, P>(this IPropValidator self, P min, P max, params Func<T, P>[] props)
            where P : struct
        {
            return Validate(
                self,
                props,
                getProp =>
                    item => Check.InBetween<P>(min, max, getProp(item)));
        }

        public static IPropValidator BiggerThan<T, P>(this IPropValidator self, P higher, params string[] names)
            where P : struct
        {
            return LowerThan(self, higher, ToProps<T, P>(names));
        }

        public static IPropValidator BiggerThan<T, P>(this IPropValidator self, P higher, params Func<T, P>[] props)
            where P : struct
        {
            return Validate<T, P>(
                self,
                props,
                getProp =>
                    item => Check.Greater(higher, getProp(item)));
        }

        public static IPropValidator LowerThan<T, P>(this IPropValidator self, P higher, params string[] names)
            where P : struct
        {
            return LowerThan<T, P>(self, higher, ToProps<T, P>(names));
        }

        public static IPropValidator LowerThan<T, P>(this IPropValidator self, P higher, params Func<T, P>[] props)
            where P : struct
        {
            return Validate<T, P>(
                self,
                props,
                getProp =>
                    item => Check.Less(higher, getProp(item)));
        }

        public static IPropValidator Regex<T>(this IPropValidator self, string expr, params string[] names)
        {
            return Regex(self, new Regex(expr), ToProps<T, string>(names));
        }

        public static IPropValidator Regex<T>(this IPropValidator self, Regex expr, params string[] names)
        {
            return Regex(self, expr, ToProps<T, string>(names));
        }

        public static IPropValidator Regex<T>(this IPropValidator self, string expr, params Func<T, string>[] props)
        {
            return Regex(self, new Regex(expr), props);
        }

        public static IPropValidator Regex<T>(this IPropValidator self, Regex expr, params Func<T, string>[] props)
        {
            return Validate(
                self,
                props,
                getProp =>
                    item => Check.Regex(expr, getProp(item)));
        }
    }
}
