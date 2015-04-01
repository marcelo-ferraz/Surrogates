using Surrogates.Tactics;
using Surrogates.Utilities.Mixins;
using Surrogates.Utilities.SDILReader;
using System;
using System.Reflection;

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

        public T4Method Method(string methodName)
        {
            return Methods(methodName);
        }

        public virtual T4Method Methods(params string[] methodNames)
        {
            var strat = new
                Strategy.ForMethods(CurrentStrategy);

            CurrentStrategy = strat;



            foreach (var methodName in methodNames)
            {
                MethodInfo method;

                if ((method = strat.BaseType.GetMethod4Surrogacy(methodName)) == null)
                { throw new MethodNotFoundException(methodName); }

                strat.Methods.Add(method);
            }

            return (T4Method)Activator.CreateInstance(typeof(T4Method), strat, Strategies);
        }

        public T4Method This(Func<TBase, Delegate> method)
        {
            return this.These(method);
        }

        public virtual T4Method These(params Func<TBase, Delegate>[] methods)
        {
            var strat = new
                Strategy.ForMethods(CurrentStrategy);

            CurrentStrategy = strat;

            foreach (var methodGetter in methods)
            {
                strat.Methods.Add(
                    methodGetter(base.GetNotInit<TBase>()).Method);
            }

            return (T4Method)Activator.CreateInstance(typeof(T4Method), strat, Strategies);
        }

        public T4Prop This(Func<TBase, object> prop)
        {
            return this.These(prop);
        }

        public virtual T4Prop These(params Func<TBase, object>[] props)
        {
            var strat = new
                Strategy.ForProperties(CurrentStrategy);

            CurrentStrategy = strat;

            foreach (var propGetter in props)
            {
                strat.Properties.Add(
                    GetProp(propGetter));
            }

            return (T4Prop)Activator.CreateInstance(typeof(T4Prop), strat, Strategies);
        }

        public T4Prop Property(string propName)
        {
            return this.Properties(propName);
        }

        public virtual T4Prop Properties(params string[] propNames)
        {
            var strat = new
                Strategy.ForProperties(CurrentStrategy);
            
            CurrentStrategy = strat;

            foreach (var propName in propNames)
            {
                PropertyInfo prop;

                if ((prop = strat.BaseType.GetProp4Surrogacy(propName)) == null)
                { throw new PropertyNotFoundException(propName); }

                strat.Properties.Add(prop);
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
            { throw new IncoherentBody4PropertyException(); }

            var prop =
                typeof(TBase).GetProp4Surrogacy(propName);
            if (prop == null)
            { throw new PropertyNotFoundException(propName); }

            return prop;
        }
    }
}
