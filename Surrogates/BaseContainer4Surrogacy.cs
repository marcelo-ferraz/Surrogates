
using Surrogates.Expressions;
using Surrogates.Model.Entities;
using Surrogates.Model.Parsers;
using Surrogates.Tactics;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading;

namespace Surrogates
{
    public abstract class BaseContainer4Surrogacy
    {
        private static int _assemblyNumber = 0;
        
        protected AssemblyBuilder AssemblyBuilder;
        protected ModuleBuilder ModuleBuilder;

        protected IDictionary<string, Entry> Dictionary;

        public BaseContainer4Surrogacy()
        {
            Dictionary =
                new Dictionary<string, Entry>();
            CreateAssemblyAndModule();
        }

        /// <summary>
        /// Invokes the asked surrogated type, either by name, or by the base type
        /// </summary>
        /// <typeparam name="T">The base type</typeparam>
        /// <param name="name">The choosen name</param>
        /// <returns></returns>
        public abstract T Invoke<T>(string name = null, dynamic stateBag = null, params object[] args);

        /// <summary>
        /// Invokes the asked surrogated type, either by name, or by the base type
        /// </summary>
        /// <param name="type">The base type</typeparam>
        /// <param name="name">The choosen name</param>
        /// <returns></returns>
        public abstract object Invoke(Type type, string name = null, dynamic stateBag = null, params object[] args);

        /// <summary>
        /// Adds a map of what needs to be changed in the instance into the container
        /// </summary>
        /// <param name="mapping"></param>
        /// <returns></returns>
        public abstract BaseContainer4Surrogacy Map(Action<NewExpression> mapping);
     
        public abstract BaseContainer4Surrogacy Map<T>(string cmd);

        public abstract BaseContainer4Surrogacy Map<T, I>(string cmd);

        public abstract BaseContainer4Surrogacy Map<T, I1, I2>(string cmd);

        protected virtual void CreateAssemblyAndModule()
        {
            Interlocked.Increment(ref _assemblyNumber);

            try
            {
                if (!Monitor.TryEnter(AppDomain.CurrentDomain))
                { throw new ExecutionEngineException("It was not possible qo aquire the lock for the domain."); }

                AssemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(
                    new AssemblyName(string.Concat("Dynamic.Proxies_", _assemblyNumber)),
                    AssemblyBuilderAccess.RunAndSave);
            }
            finally { Monitor.Exit(AppDomain.CurrentDomain); }

            ModuleBuilder = AssemblyBuilder.DefineDynamicModule(
                string.Concat("Dynamic.Module.Proxies_", _assemblyNumber),
                string.Concat(AssemblyBuilder.GetName().Name, ".dll"));
        }
        
        /// <summary>
        /// It represents a map, from wich the container will follow to create a proxy to that class
        /// </summary>
        /// <param name="mapping"></param>
        /// <returns></returns>
        protected virtual void InternalMap(Action<NewExpression> mapping)
        {
            var expression =
                new NewExpression(ModuleBuilder, this);

            mapping(expression);

            var entry =
                expression.Strategies.Apply();
                        
            Dictionary.Add(entry.Type.Name, entry);
        }

        protected virtual void InternalMap<T>(string cmd, params Type[] interceptors)
        {
            string[] aliases = (string[]) 
                Array.CreateInstance(typeof(string), 0);
            
            var strategies = 
                ParseStrCmd(cmd, typeof(T), interceptors, ref aliases);

            var entry =
                strategies.Apply();

            Dictionary.Add(aliases[0], entry);
        }

        protected virtual Strategies ParseStrCmd(string cmd, Type baseType, Type[] interceptors, ref string[] aliases)
        {
            var parser = 
                new SCLParser();

            if (!parser.TryGetAliases(cmd, ref aliases))
            { throw new FormatException("Could not extract 'Aliases' from the command written"); }

            var strategies =
                new Strategies(baseType, aliases[0], ModuleBuilder);

            if (!parser.TryGetOperations(cmd, aliases, interceptors, ref strategies))
            { throw new FormatException("Could extract an expression from the command written"); }

            return strategies;
        }

        public virtual bool Has<T>(string key = null)
        {
            return this.Has(typeof(T), key);
        }

        public virtual bool Has(Type type = null, string key = null)
        {
            if (string.IsNullOrEmpty(key))
            { key = string.Concat(type.Name, "Proxy"); }

            return Dictionary.ContainsKey(key);
        }

        /// <summary>
        /// Saves the current proxies in a file, so you don't have to do map it again, if you want to
        /// </summary>
        public virtual void Save()
        {
            AssemblyBuilder.Save(string.Concat(AssemblyBuilder.GetName().Name, ".dll"));
            //TODO: save the dictionary, with all the types, surrogates and names serialized in a file
        }
    }
}
