using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Surrogates.Utilities.Mixins;

namespace Surrogates.Applications.Validators
{
    public class PropertiesValidator<T> : BaseValidator
    {        
        private Func<T, P>[] ToProps<P>(params string[] names)
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

        public Func<T, bool> Required(params string[] names)
        {
            return Required(ToProps<object>(names));
        }

        public Func<T, bool> Required(params Func<T, object>[] props)
        {
            Func<T, bool> result = null;

            foreach (var getProp in props)
            {
                Func<T, bool> validate;

                if (getProp.Method.ReturnType == typeof(string))
                {
                    validate =
                        item =>
                        {
                            var val = getProp(item);
                            var t = val.GetType();

                            return (t == typeof(string) && string.IsNullOrEmpty((string)val)) ?
                                false :
                                val == Activator.CreateInstance(t);
                        };
                }
                else
                {
                    validate =
                        item =>
                        {
                            var val = getProp(item);
                            var t = val.GetType();

                            return (!t.IsValueType || Nullable.GetUnderlyingType(t) != null) ?
                                val == null :
                                val == Activator.CreateInstance(t);
                        };
                }

                result = (Func<T, bool>)(result != null ?
                    Delegate.Combine(result, validate) :
                    validate);
            }

            return result;
        }

        public Func<T, bool> Email(params string[] names)
        {
            return Email(ToProps<string>(names));
        }

        public Func<T, bool> Email(params Func<T, string>[] props)
        {
            return Regex(EmailRegexpr, props);
        }

        public Func<T, bool> Url(params string[] names)
        {
            return Url(ToProps<string>(names));
        }

        public Func<T, bool> Url(params Func<T, string>[] props)
        {
            return Regex(UrlRegexpr, props);
        }

        public Func<T, bool> IsNumber(params string[] names)
        {
            return IsNumber(ToProps<string>(names));
        }

        public Func<T, bool> IsNumber(params Func<T, string>[] props)
        {
            return Regex(IsNumberRegexpr, props);
        }

        public Func<T, bool> InBetween<P>(P min, P max, params string[] names)
            where P : struct
        {
            return InBetween<P>(min, max, ToProps<P>(names));
        }

        public Func<T, bool> InBetween<P>(P min, P max, params Func<T, P>[] props)
            where P : struct
        {
            Func<T, bool> result = null;

            var comparer = Comparer<P>.Default;

            foreach (var getProp in props)
            {
                Func<T, bool> validate =
                    item =>
                        (comparer.Compare(min, getProp(item)) > 0) &&
                        (comparer.Compare(max, getProp(item)) < 0);

                result = (Func<T, bool>)(result != null ?
                    Delegate.Combine(result, validate) :
                    validate);
            }

            return result;
        }

        public Func<T, bool> BiggerThan<P>(P lower, params string[] names)
            where P : struct
        {
            return LowerThan(lower, ToProps<P>(names));
        }

        public static Func<T, bool> BiggerThan<P>(P lower, params Func<T, P>[] props)
            where P : struct
        {
            Func<T, bool> result = null;

            var comparer = Comparer<P>.Default;

            foreach (var getProp in props)
            {
                Func<T, bool> validate =
                    item =>
                        (comparer.Compare(lower, getProp(item)) > 0);

                result = (Func<T, bool>)(result != null ?
                    Delegate.Combine(result, validate) :
                    validate);
            }

            return result;
        }

        public Func<T, bool> LowerThan<P>(P higher, params string[] names)
            where P : struct
        {
            return LowerThan(higher, ToProps<P>(names));
        }

        public Func<T, bool> LowerThan<P>(P higher, params Func<T, P>[] props)
            where P : struct
        {
            Func<T, bool> result = null;

            var comparer = Comparer<P>.Default;

            foreach (var getProp in props)
            {
                Func<T, bool> validate =
                    item =>
                        (comparer.Compare(higher, getProp(item)) < 0);

                result = (Func<T, bool>)(result != null ?
                    Delegate.Combine(result, validate) :
                    validate);
            }

            return result;
        }

        public Func<T, bool> Regex(string expr, params string[] names)
        {
            return Regex(new Regex(expr), ToProps<string>(names));
        }

        public Func<T, bool> Regex(Regex expr, params string[] names)
        {
            return Regex(expr, ToProps<string>(names));
        }

        public Func<T, bool> Regex(string expr, params Func<T, string>[] props)
        {
            return Regex(new Regex(expr), props);
        }

        public Func<T, bool> Regex(Regex expr, params Func<T, string>[] props)
        {
            Func<T, bool> result = null;

            foreach (var getProp in props)
            {
                Func<T, bool> validate =
                    item =>
                        expr.IsMatch(getProp(item));

                result = (Func<T, bool>)(result != null ?
                    Delegate.Combine(result, validate) :
                    validate);
            }

            return result;
        }
    }

}
