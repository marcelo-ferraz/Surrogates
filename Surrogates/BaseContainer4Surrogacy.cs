
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading;
using Surrogates.Expressions;
using Surrogates.Model;
using Surrogates.Model.Parsers;
using Surrogates.Tactics;

namespace Surrogates
{
    public abstract class BaseContainer4Surrogacy
    {
        private static int _assemblyNumber = 0;
        
        protected AssemblyBuilder AssemblyBuilder;
        protected ModuleBuilder ModuleBuilder;
        
        protected IDictionary<string, Type> Dictionary;

        public BaseContainer4Surrogacy()
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
        /// It represents a map, from wich the container will follow to create a proxy to that class
        /// </summary>
        /// <param name="mapping"></param>
        /// <returns></returns>
        protected virtual void InternalMap(Action<NewExpression> mapping)
        {
            var expression =
                new NewExpression(ModuleBuilder);

            mapping(expression);

            Type type =
                expression.Strategies.Apply();
            var name = expression.Name;
            
            Dictionary.Add(type.Name, type);
        }

        protected virtual void InternalMap<T>(string cmd, params Type[] interceptors)
        {
            var parser = new SCLParser();
            
            string[] aliases = null;

            if (!parser.TryGetAliases(cmd, ref aliases))
            { throw new FormatException("Could not extract 'Aliases' from the command written"); }

            var strategies = 
                new Strategies(typeof(T), aliases[0], ModuleBuilder);
            
            if (!parser.TryGetOperations(cmd, aliases, interceptors, ref strategies))
            { throw new FormatException("Could extract an expression from the command written"); }

            Type type =
                strategies.Apply();

            Dictionary.Add(aliases[0], type);
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
            //save the dictionary, with all the types, surrogates and names
        }
    }
}
