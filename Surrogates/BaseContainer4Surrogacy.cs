
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading;
using Surrogates.Mappers;
using Surrogates.Mappers.Entities;

namespace Surrogates
{
    public abstract class BaseContainer4Surrogacy<T, M>
        where M: BaseMapper
    {
        private static int _assemblyNumber = 0;
        
        protected AssemblyBuilder AssemblyBuilder;
        protected ModuleBuilder ModuleBuilder;
        
        protected IDictionary<string, T> Dictionary;

        public BaseContainer4Surrogacy()
        {
            Dictionary =
                new Dictionary<string, T>();
            CreateAssemblyAndModule();
        }

        protected abstract void AddMap(M mappingExp, Type type);

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
        protected virtual M CreateExpression()
        {
            return (M) Activator.CreateInstance(typeof(M), new MappingState { AssemblyBuilder = AssemblyBuilder, ModuleBuilder = ModuleBuilder });
        }

        /// <summary>
        /// It represents a map, from wich the container will follow to create a proxy to that class
        /// </summary>
        /// <param name="mapping"></param>
        /// <returns></returns>
        protected virtual void InternalMap(Action<IMapper> mapping)
        {
            var expression =
                this.CreateExpression();

            mapping.Invoke(expression);

            Type type =
                expression.Flush();

            AddMap(expression, type);
        }

        protected virtual void InternalMap(string cmd)
        {
            var expression =
                this.CreateExpression();



            Type type =
                expression.Flush();

            AddMap(expression, type);
        }

        public virtual bool Has<T>(string key = null)
        {
            return this.Has(typeof(T), key);
        }

        public virtual bool Has(Type type = null, string key = null)
        {
            if (string.IsNullOrEmpty(key))
            { key = DefaultMapper.CreateName4(type); }

            return Dictionary.ContainsKey(key);
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
