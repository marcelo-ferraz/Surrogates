using Surrogates.Mappers;
using Surrogates.Mappers.Entities;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading;

namespace Surrogates 
{
    /// <summary>
    /// The main container, from wich you can invoke your dependencies, and/or map them
    /// </summary>
    public class SurrogatesContainer : BaseContainer4Surrogacy
    {
        public new SurrogatesContainer Map(Action<IMapper> mapping)
        {
            return (SurrogatesContainer)base.Map(mapping);
        }

        public virtual T Invoke<T>(string name = null)
        {
            return (T)Invoke(typeof(T), name);
        }
        
        public virtual object Invoke(Type type, string name = null)
        {
            if (string.IsNullOrEmpty(name))
            { name = DefaultMapper.CreateName4(type); }

            return Activator.CreateInstance(Dictionary[name]);            
        }

       /// <summary>
       /// Saves the current proxies in a file, so you don't have to do map it again, if you want to
       /// </summary>
        public virtual void Save()
        {
            AssemblyBuilder.Save(string.Concat(AssemblyBuilder.GetName().Name, ".dll"));
            //save the dictionary, with all the types, surrogates and names
        }
    }
}
