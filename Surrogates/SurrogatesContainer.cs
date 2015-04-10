using Surrogates.Expressions;
using System;

namespace Surrogates
{
    /// <summary>
    /// The main container, from which you can Map And Invoke your surrogated types
    /// </summary>
    public class SurrogatesContainer : BaseContainer4Surrogacy
    {
        /// <summary>
        /// Invokes the asked surrogated type, either by name, or by the base type
        /// </summary>
        /// <typeparam name="T">The base type</typeparam>
        /// <param name="name">The choosen name</param>
        /// <returns></returns>
        public override T Invoke<T>(string name = null, dynamic stateBag = null, params object[] args)
        {
            return (T)Invoke(typeof(T), name, stateBag, args);
        }

        /// <summary>
        /// Invokes the asked surrogated type, either by name, or by the base type
        /// </summary>
        /// <param name="type">The base type</typeparam>
        /// <param name="name">The choosen name</param>
        /// <returns></returns>
        public override object Invoke(Type type, string name = null, dynamic stateBag = null, params object[] args)
        {
            if (string.IsNullOrEmpty(name))
            { name = string.Concat(type.Name, "Proxy"); }

            var entry = Dictionary[name];

            var obj = Activator
                .CreateInstance(entry.Type, args);

            if (stateBag != null)
            { entry.StateProperty.SetValue(obj, stateBag, null); }

            entry.ContainerProperty.SetValue(obj, this, null);

            return obj;
        }

        /// <summary>
        /// Adds a map of what needs to be changed in the instance into the container
        /// </summary>
        /// <param name="mapping"></param>
        /// <returns></returns>
        public override BaseContainer4Surrogacy Map(Action<NewExpression> mapping)
        {
            base.InternalMap(mapping);
            return this;
        }

        public override BaseContainer4Surrogacy Map<T>(string cmd)
        {
            InternalMap<T>(cmd);

            return this;
        }

        public override BaseContainer4Surrogacy Map<T, I>(string cmd)
        {
            InternalMap<T>(cmd, typeof(I));

            return this;
        }

        public override BaseContainer4Surrogacy Map<T, I1, I2>(string cmd)
        {
            InternalMap<T>(cmd, typeof(I1), typeof(I2));
            
            return this;
        }
    }
}