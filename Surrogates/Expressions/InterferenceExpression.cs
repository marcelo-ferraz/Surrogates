using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Surrogates.Tactics;
using Surrogates.Utilities.Mixins;
using Surrogates.Utilities.SDILReader;

namespace Surrogates.Expressions
{
    public abstract class InterferenceExpression<TBase, TReturn>
       : InterferenceExpression<TBase, TReturn, TReturn>
    {
        public InterferenceExpression(Strategy current, Strategies strategies)
            : base(current, strategies) { }
    }
    public abstract class InterferenceExpression<TBase, T4Prop, T4Method> : Expression<TBase, Strategy>
    {
        public InterferenceExpression(Strategy current, Strategies strategies)
            : base(current, strategies) { }

        public T4Method This(params Func<TBase, Delegate>[] method)
        {
            return (T4Method)Activator.CreateInstance(typeof(T4Method), CurrentStrategy, Strategies);
        }

        public T4Prop This(params Func<TBase, object>[] props)
        {
            var strat = new
                Strategy.ForProperties(CurrentStrategy);

            foreach (var propGetter in props)
            {
                strat.Properties.Add(
                    GetProp(propGetter));
            }

            return (T4Prop)Activator.CreateInstance(typeof(T4Prop), strat, Strategies);
        }

        protected virtual PropertyInfo GetProp(Func<TBase, object> propGetter)
        {
            var reader =
                new MethodBodyReader(propGetter.Method);

            string propName = null;

            for (int i = 0; i < reader.Instructions.Count; i++)
            {
                var code =
                    reader.Instructions[i].Code.Name;

                if (code != "callvirt" && code != "call")
                { continue; }

                if (!(reader.Instructions[1].Operand is MethodInfo))
                { continue; }

                propName = ((MethodInfo)reader.Instructions[i].Operand).Name;

                if (!propName.Contains("get_") && !propName.Contains("set_"))
                { throw new ArgumentException("What was provided is not an property"); }

                propName = propName
                    .Replace("get_", string.Empty)
                    .Replace("set_", string.Empty);

                break;
            }

            if (string.IsNullOrEmpty(propName))
            { throw new IncoherentPropertyOnBodyException(); }

            var prop =
                typeof(TBase).GetProp4Surrogacy(propName);
            if (prop == null)
            { throw new PropertyNotFoundException(propName); }

            return prop;
        }
    }
}
