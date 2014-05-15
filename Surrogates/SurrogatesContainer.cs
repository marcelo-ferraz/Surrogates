using Surrogates.Mappers;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading;

namespace Surrogates 
{
    /// <summary>
    /// 
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
        /// Creates the name of the surrogate type, in case no type is provided.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        protected virtual string CreateTypeName<T>()
        {
            return DefaultMapper.CreateName4<T>();
        }

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
            { name = CreateTypeName<T>(); }

            return (T)Activator.CreateInstance(Dictionary[name]);
        }

        public virtual void Save()
        {
            AssemblyBuilder.Save(string.Concat(AssemblyBuilder.GetName().Name, ".dll"));
            //save the dictionary, with all the types, surrogates and names
        }
    }
}
