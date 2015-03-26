using System;
using System.Reflection.Emit;
using Surrogates.Expressions;
using Surrogates.Model;

namespace Surrogates
{
    /// <summary>
    /// The main container, from wich you can invoke your dependencies, and/or map them
    /// </summary>
    public class SurrogatesContainer : BaseContainer4Surrogacy
    {
        private SurrogatesContainer Map<T>(string[] cmds, params Type[] interceptors)
        {
            foreach (var cmd in cmds)
            { InternalMap<T>(cmd, interceptors); }

            return this;
        }

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
        public virtual T Invoke<T>(string name = null)
        {
            return (T)Invoke(typeof(T), name);
        }

        /// <summary>
        /// Invokes the asked surrogated type, either by name, or by the base type
        /// </summary>
        /// <param name="type">The base type</typeparam>
        /// <param name="name">The choosen name</param>
        /// <returns></returns>
        public virtual object Invoke(Type type, string name = null)
        {
            if (string.IsNullOrEmpty(name))
            { name = string.Concat(type.Name, "Proxy"); }

            return Activator.CreateInstance(Dictionary[name]);
        }
        
        public virtual SurrogatesContainer Map<T, I>(params string[] cmds)
        {
            return Map<T>(cmds, typeof(I));
        }

        public virtual SurrogatesContainer Map<T, I1, I2>(params string[] cmds)
        {
            return Map<T>(cmds, typeof(I1), typeof(I2));
        }

        public virtual SurrogatesContainer Map<T, I1, I2, I3>(params string[] cmds)
        {
            return Map<T>(cmds, typeof(I1), typeof(I2), typeof(I3));
        }

        public virtual SurrogatesContainer Map<T, I1, I2, I3, I4>(params string[] cmds)
        {
            return Map<T>(cmds, typeof(I1), typeof(I2), typeof(I3), typeof(I4));
        }

        public virtual SurrogatesContainer Map<T, I1, I2, I3, I4, I5>(params string[] cmds)
        {
            return Map<T>(cmds, typeof(I1), typeof(I2), typeof(I3), typeof(I4), typeof(I5));
        }

        public virtual SurrogatesContainer Map<T, I1, I2, I3, I4, I5, I6>(params string[] cmds)
        {
            return Map<T>(cmds, typeof(I1), typeof(I2), typeof(I3), typeof(I4), typeof(I5), typeof(I6));
        }

        public virtual SurrogatesContainer Map<T, I1, I2, I3, I4, I5, I6, I7>(params string[] cmds)
        {
            return Map<T>(cmds, typeof(I1), typeof(I2), typeof(I3), typeof(I4), typeof(I5), typeof(I6), typeof(I7));
        }

        public virtual SurrogatesContainer Map<T, I1, I2, I3, I4, I5, I6, I7, I8>(params string[] cmds)
        {
            return Map<T>(cmds, typeof(I1), typeof(I2), typeof(I3), typeof(I4), typeof(I5), typeof(I6), typeof(I7), typeof(I8));
        }

        public virtual SurrogatesContainer Map<T, I1, I2, I3, I4, I5, I6, I7, I8, I9>(params string[] cmds)
        {
            return Map<T>(cmds, typeof(I1), typeof(I2), typeof(I3), typeof(I4), typeof(I5), typeof(I6), typeof(I7), typeof(I8), typeof(I9));
        }
    }
}