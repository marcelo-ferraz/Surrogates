using Surrogates.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Surrogates
{
    /// <summary>
    /// The main container, from wich you can invoke your dependencies, and/or map them
    /// </summary>
    public class SurrogatesContainer : BaseContainer4Surrogacy
    {
        /// <summary>
        /// Adds a map of what needs to be changed in the instance into the container
        /// </summary>
        /// <param name="mapping"></param>
        /// <returns></returns>
        public SurrogatesContainer Map(Action<NewExpression> mapping)
        {
            base.InternalMap(mapping);
            return this;
        }

        /// <summary>
        /// Invokes the asked surrogated type, either by name, or by the base type
        /// </summary>
        /// <typeparam name="T">The base type</typeparam>
        /// <param name="name">The choosen name</param>
        /// <returns></returns>
        public virtual T Invoke<T>(string name = null, dynamic stateBag = null)
        {
            return (T)Invoke(typeof(T), name, stateBag);
        }

        /// <summary>
        /// Invokes the asked surrogated type, either by name, or by the base type
        /// </summary>
        /// <param name="type">The base type</typeparam>
        /// <param name="name">The choosen name</param>
        /// <returns></returns>
        public virtual object Invoke(Type type, string name = null, dynamic stateBag = null)
        {
            if (string.IsNullOrEmpty(name))
            { name = string.Concat(type.Name, "Proxy"); }

            var entry = Dictionary[name];

            var obj = Activator
                .CreateInstance(entry.Type);

            if (stateBag != null)
            { entry.StateProperty.SetValue(obj, stateBag, null); }

            return obj;
        }

        public virtual SurrogatesContainer Map<T>(string cmd)
        {
            InternalMap<T>(cmd);

            return this;
        }

        public virtual SurrogatesContainer Map<T, I>(string cmd)
        {
            InternalMap<T>(cmd, typeof(I));

            return this;
        }

        public virtual SurrogatesContainer Map<T, I1, I2>(string cmd)
        {
            InternalMap<T>(cmd, typeof(I1), typeof(I2));
            
            return this;
        }
    }
}