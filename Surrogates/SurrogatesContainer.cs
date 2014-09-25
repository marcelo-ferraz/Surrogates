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
    public class SurrogatesContainer
    {
        private static int _assemblyNumber = 0;

        protected AssemblyBuilder AssemblyBuilder;
        protected ModuleBuilder ModuleBuilder;

        protected IDictionary<string, Type> Dictionary;

        public SurrogatesContainer()
        {
            Dictionary = 
                new Dictionary<string, Type>();
            CreateAssemblyAndModule();
        }

        protected virtual void CreateAssemblyAndModule()
        {
            Interlocked.Increment(ref _assemblyNumber);

            AssemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(
                new AssemblyName(string.Concat("Dynamic.Proxies_", _assemblyNumber)),
                AssemblyBuilderAccess.RunAndSave);

            ModuleBuilder = AssemblyBuilder.DefineDynamicModule(
                string.Concat("Dynamic.Module.Proxies_", _assemblyNumber), 
                string.Concat(AssemblyBuilder.GetName().Name, ".dll"));
        }

        /// <summary>
        /// Used to create expressions for mapping a specific type
        /// </summary>
        /// <returns></returns>
        protected virtual DefaultMapper CreateExpression()
        {
            return new DefaultMapper(new MappingState 
            { AssemblyBuilder = AssemblyBuilder, ModuleBuilder = ModuleBuilder });
        }

        /// <summary>
        /// It represents a map, from wich the container will follow to create a proxy to that class
        /// </summary>
        /// <param name="mapping"></param>
        /// <returns></returns>
        public virtual SurrogatesContainer Map(Action<IMapper> mapping)
        {
            var expression =
                CreateExpression();

            mapping.Invoke(expression);

            Type type =
                expression.Flush();

            Dictionary.Add(type.Name, type);

            return this;
        }


        public virtual T Invoke<T>(string name = null)
        {
            if (string.IsNullOrEmpty(name))
            { name = DefaultMapper.CreateName4<T>(); }

            return (T)Activator.CreateInstance(Dictionary[name]);
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
