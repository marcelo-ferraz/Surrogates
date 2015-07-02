using Surrogates.Tactics;
using Surrogates.Utilities.Mixins;
using Surrogates.Utilities.SDILReader;
using System;
using System.Reflection;
using System.Runtime.Serialization;

namespace Surrogates.Expressions
{
    public abstract class InterferenceExpression<TBase, TReturn>
       : InterferenceExpression<TBase, TReturn, TReturn>
    {
        public InterferenceExpression(BaseContainer4Surrogacy container, Strategy current, Strategies strategies)
            : base(container, current, strategies) { }
    }

    public abstract class InterferenceExpression<TBase, T4Prop, T4Method> : Expression<TBase, Strategy>
    {
        public InterferenceExpression(BaseContainer4Surrogacy container, Strategy current, Strategies strategies)
            : base(container, current, strategies) { }
        
        protected virtual PropertyInfo GetProp(Func<TBase, object> propGetter)
        {
            if (propGetter.Method.ReturnType is Func<TBase, object>)
            {
                var obj = (TBase)FormatterServices
                    .GetUninitializedObject(typeof(TBase));

                return GetProp((Func<TBase, object>)propGetter(obj));
            }

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

        /// <summary>
        /// Adds a method, by its name, to the collection of expressions
        /// </summary>
        /// <param name="methodName"></param>
        /// <param name="onlyViable">Whether it will throw an exception for non-overrideable methods or just ignore them</param>
        /// <returns></returns>
        public virtual T4Method Method(string methodName, bool onlyViable = true)
        {
            return Methods(new [] { methodName }, onlyViable);
        }

        /// <summary>
        /// Adds methods, by its names, to the collection of expressions
        /// </summary>
        /// <param name="methodNames"></param>
        /// <returns></returns>
        public virtual T4Method Methods(params string[] methodNames)
        {
            return Methods(methodNames, false);
        }

        /// <summary>
        /// Adds methods, by its names, to the collection of expressions
        /// </summary>
        /// <param name="methodNames"></param>
        /// <param name="onlyViable">Whether it will throw an exception for non-overrideable methods or just ignore them</param>
        /// <returns></returns>
        public virtual T4Method Methods(string[] methodNames, bool onlyViable = true)
        {
            var strat = new
                Strategy.ForMethods(CurrentStrategy);

            CurrentStrategy = strat;

            foreach (var methodName in methodNames)
            {
                MethodInfo method;

                if ((method = strat.BaseType.GetMethod4Surrogacy(methodName)) == null)
                { throw new MethodNotFoundException(methodName); }

                strat.Add(method, onlyViable);
            }

            return (T4Method)Activator.CreateInstance(typeof(T4Method), Container, strat, Strategies);
        }

        /// <summary>
        /// Adds any method, as long as it satisfies a predicate, to the collection of expressions
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="onlyViable">Whether it will throw an exception for non-overrideable methods or just ignore them</param>
        /// <returns></returns>
        public virtual T4Method Methods(Func<MethodInfo, bool> predicate, bool onlyViable = true) 
        {
            var strat = new
                Strategy.ForMethods(CurrentStrategy);
            
            CurrentStrategy = strat;

            var methods = Strategies
                .BaseType
                .GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);                

            foreach (var method in methods)
            {
                if(predicate(method))
                { strat.Add(method, onlyViable); }
            }

            return (T4Method) Activator.CreateInstance(typeof(T4Prop), Container, strat, Strategies);
        }


        /// <summary>
        /// Adds a method, to the collection of expressions
        /// </summary>
        /// <param name="method"></param>
        /// <param name="onlyViable">Whether it will throw an exception for non-overrideable methods or just ignore them</param>
        /// <returns></returns>
        public virtual T4Method This(Func<TBase, Delegate> method, bool onlyViable = true)
        {
            return this.These(new [] { method }, onlyViable);
        }

        /// <summary>
        /// Adds methods, to the collection of expressions
        /// </summary>
        /// <param name="methods"></param>
        /// <returns></returns>
        public virtual T4Method These(params Func<TBase, Delegate>[] methods)
        {
            return These(methods, true);
        }

        /// <summary>
        /// Adds methods, to the collection of expressions
        /// </summary>
        /// <param name="methods"></param>
        /// <param name="onlyViable">Whether it will throw an exception for non-overrideable methods or just ignore them</param>
        /// <returns></returns>
        public virtual T4Method These(Func<TBase, Delegate>[] methods, bool onlyViable = true)
        {
            var strat = new
                Strategy.ForMethods(CurrentStrategy);

            CurrentStrategy = strat;

            foreach (var methodGetter in methods)
            {
                strat.Add(
                    methodGetter(base.GetNotInit<TBase>()).Method, onlyViable);
            }

            return (T4Method)Activator.CreateInstance(typeof(T4Method), Container, strat, Strategies);
        }

        /// <summary>
        /// Adds a property, to the collection of expressions
        /// </summary>
        /// <param name="prop"></param>
        /// <param name="onlyViable">Whether it will throw an exception for non-overrideable properties or just ignore them</param>
        /// <returns></returns>
        public virtual T4Prop This(Func<TBase, object> prop, bool onlyViable = true)
        {
            return this.These(prop);
        }
        
        /// <summary>
        /// Adds properties, to the collection of expressions
        /// </summary>
        /// <param name="props"></param>
        /// <returns></returns>
        public virtual T4Prop These(params Func<TBase, object>[] props)
        {
            return These(props, true);
        }

        /// <summary>
        /// Adds properties, to the collection of expressions
        /// </summary>
        /// <param name="props"></param>
        /// <param name="onlyViable">Whether it will throw an exception for non-overrideable properties or just ignore them</param>
        /// <returns></returns>
        public virtual T4Prop These(Func<TBase, object>[] props, bool onlyViable = true)
        {
            var strat = new
                Strategy.ForProperties(CurrentStrategy);

            CurrentStrategy = strat;

            foreach (var propGetter in props)
            {
                strat.Add(GetProp(propGetter), onlyViable);
            }

            return (T4Prop) Activator.CreateInstance(typeof(T4Prop), Container, strat, Strategies);
        }

        /// <summary>
        /// Adds a property by its name, to the collection of expressions
        /// </summary>
        /// <param name="propName"></param>
        /// <param name="onlyViable">Whether it will throw an exception for non-overrideable properties or just ignore them</param>
        /// <returns></returns>
        public virtual T4Prop Property(string propName, bool onlyViable = true)
        {
            return this.Properties(propName);
        }

        /// <summary>
        /// Adds properties if they satisfy a given predicate, to the collection of expressions
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="onlyViable">Whether it will throw an exception for non-overrideable properties or just ignore them</param>
        /// <returns></returns>
        public virtual T4Prop Properties(Func<PropertyInfo, bool> predicate, bool onlyViable = true)
        {
            var strat = new
                Strategy.ForProperties(CurrentStrategy);
            
            CurrentStrategy = strat;

            var props = Strategies
                .BaseType
                .GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);                

            foreach (var prop in props)
            {
                if(predicate(prop))
                { strat.Add(prop, onlyViable); }
            }

            return (T4Prop)Activator.CreateInstance(typeof(T4Prop), Container, strat, Strategies);
        }

        /// <summary>
        /// Adds properties by their names, to the collection of expressions
        /// </summary>
        /// <param name="propNames"></param>
        /// <returns></returns>
        public virtual T4Prop Properties(params string[] propNames)
        {
            return Properties(propNames, onlyViable: true);
        }
        
        /// <summary>
        /// Adds properties by their names, to the collection of expressions
        /// </summary>
        /// <param name="propNames"></param>
        /// <param name="onlyViable">Whether it will throw an exception for non-overrideable properties or just ignore them</param>
        /// <returns></returns>
        public virtual T4Prop Properties(string[] propNames, bool onlyViable = true)
        {
            var strat = new
                Strategy.ForProperties(CurrentStrategy);
            
            CurrentStrategy = strat;

            foreach (var propName in propNames)
            {
                PropertyInfo prop;

                if ((prop = strat.BaseType.GetProp4Surrogacy(propName)) == null)
                { throw new PropertyNotFoundException(propName); }

                strat.Add(prop, onlyViable);
            }

            return (T4Prop)Activator.CreateInstance(typeof(T4Prop), Container, strat, Strategies);
        }

    }
}
