using Surrogates.Applications.Contracts;
using Surrogates.Utilities.Mixins;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Text.RegularExpressions;

namespace Surrogates.Applications
{
    public static class PropertyAssertionMixins
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

        private static IPropValidators Validate<T, P>(IPropValidators assertions, Func<T, P>[] props, Func<Func<T, P>, Action<T>> validator)
        {
            var obj = (T)
                FormatterServices.GetUninitializedObject(typeof(T));

            for (int i = 0; i < props.Length; i++)
            {
                var validate =
                    validator(props[i]);

                if (validate == null)
                { continue; }

                ((Assert_.List4.Properties)(assertions ?? (assertions = new Assert_.List4.Properties())))
                    .Validators
                    .Add(new Assert_.Entry4.Properties
                    {
                        Property = props[i],
                        Validation = validator(props[i])
                    });
            }

            return assertions;
        }

        public static IPropValidators Required<T>(this IPropValidators self, params string[] names)
        {
            return Required(self, ToProps<T, object>(names));
        }

        public static IPropValidators Required<T>(this IPropValidators self, params Func<T, object>[] props)
        {
            return Validate<T, object>(self, props, getProp =>
            {
                if (getProp.Method.ReturnType == typeof(string))
                {
                    return item =>
                        _Validate.NotNullString(getProp(item));
                }
                else
                {
                    return item =>
                        _Validate.Required(getProp(item), getProp(item).GetType());
                }
            });
        }

        public static IPropValidators Email<T>(this IPropValidators self, params string[] names)
        {
            return Email<T>(self, ToProps<T, string>(names));
        }

        public static IPropValidators Email<T>(this IPropValidators self, params Func<T, string>[] props)
        {
            return Regex<T>(self, _Validate.EmailRegexpr, props);
        }

        public static IPropValidators Url<T>(this IPropValidators self, params string[] names)
        {
            return Url<T>(self, ToProps<T, string>(names));
        }

        public static IPropValidators Url<T>(this IPropValidators self, params Func<T, string>[] props)
        {
            return Regex<T>(self, _Validate.UrlRegexpr, props);
        }

        public static IPropValidators IsNumber<T>(this IPropValidators self, params string[] names)
        {
            return IsNumber<T>(self, ToProps<T, string>(names));
        }

        public static IPropValidators IsNumber<T>(this IPropValidators self, params Func<T, string>[] props)
        {
            return Regex<T>(self, _Validate.IsNumberRegexpr, props);
        }

        public static IPropValidators InBetween<T, P>(this IPropValidators self, P min, P max, params string[] names)
            where P : struct
        {
            return InBetween<T, P>(self, min, max, ToProps<T, P>(names));
        }

        public static IPropValidators InBetween<T, P>(this IPropValidators self, P min, P max, params Func<T, P>[] props)
            where P : struct
        {
            return Validate(
                self,
                props,
                getProp =>
                    item => _Validate.InBetween<P>(min, max, getProp(item)));
        }

        public static IPropValidators BiggerThan<T, P>(this IPropValidators self, P higher, params string[] names)
            where P : struct
        {
            return LowerThan(self, higher, ToProps<T, P>(names));
        }

        public static IPropValidators BiggerThan<T, P>(this IPropValidators self, P higher, params Func<T, P>[] props)
            where P : struct
        {
            return Validate<T, P>(
                self,
                props,
                getProp =>
                    item => _Validate.GreatterThan(higher, getProp(item)));
        }

        public static IPropValidators LowerThan<T, P>(this IPropValidators self, P higher, params string[] names)
            where P : struct
        {
            return LowerThan<T, P>(self, higher, ToProps<T, P>(names));
        }

        public static IPropValidators LowerThan<T, P>(this IPropValidators self, P higher, params Func<T, P>[] props)
            where P : struct
        {
            return Validate<T, P>(
                self,
                props,
                getProp =>
                    item => _Validate.LessThan(higher, getProp(item)));
        }

        public static IPropValidators Regex<T>(this IPropValidators self, string expr, params string[] names)
        {
            return Regex(self, new Regex(expr), ToProps<T, string>(names));
        }

        public static IPropValidators Regex<T>(this IPropValidators self, Regex expr, params string[] names)
        {
            return Regex(self, expr, ToProps<T, string>(names));
        }

        public static IPropValidators Regex<T>(this IPropValidators self, string expr, params Func<T, string>[] props)
        {
            return Regex(self, new Regex(expr), props);
        }

        public static IPropValidators Regex<T>(this IPropValidators self, Regex expr, params Func<T, string>[] props)
        {
            return Validate(
                self,
                props,
                getProp =>
                    item => _Validate.Regex(expr, getProp(item)));
        }
    }
}
