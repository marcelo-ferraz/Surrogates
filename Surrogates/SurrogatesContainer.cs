using System;
using System.Reflection.Emit;
using Surrogates.Mappers;

namespace Surrogates
{
    /// <summary>
    /// The main container, from wich you can invoke your dependencies, and/or map them
    /// </summary>
    public class SurrogatesContainer : BaseContainer4Surrogacy<Type, DefaultMapper>
    {
        protected override void AddMap(DefaultMapper mappingExp, Type type)
        {
            Dictionary.Add(type.Name, type);
        }

        /// <summary>
        /// Adds a map of what needs to be changed in the instance into the container
        /// </summary>
        /// <param name="mapping"></param>
        /// <returns></returns>
        public SurrogatesContainer Map(Action<IMapper> mapping)
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
            { name = DefaultMapper.CreateName4(type); }

            return Activator.CreateInstance(Dictionary[name]);
        }

        public virtual SurrogatesContainer Map<T, Tin>(params string[] cmds)
        {
 
            return this;
        }
    }
}