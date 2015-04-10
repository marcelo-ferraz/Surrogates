using Surrogates.Model.Collections;
using Surrogates.Model.Entities;
using Surrogates.Utilities.Mixins;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace Surrogates.Tactics
{
    public class Strategies 
    {
        private TypeBuilder _builder;
        private IList<Strategy> _strategies;

        internal Strategies(Type baseType, string name, ModuleBuilder moduleBuilder)
        {            
            if (string.IsNullOrEmpty(name))
            { name = string.Concat(baseType, "Proxy"); }
            
            try
            {
                Builder = moduleBuilder
                    .DefineType(name, TypeAttributes.Public, baseType);
            }
            catch (ArgumentException argEx)
            { throw new ProxyAlreadyMadeException(baseType, name, argEx); }

            this.BaseType = baseType;
            this._strategies = new List<Strategy>();
            this.Fields = new FieldList(this);
        }

        public Type BaseType { get; set; }

        public TypeBuilder Builder { get; set; }
        
        public FieldList Fields { get; set; }
               
        public void Add(Strategy strategy)
        {
            _strategies.Add(strategy);
        }

        public Entry Apply()
        {
            foreach (var strategy in _strategies)
            {
                strategy.Apply(
                    this.BaseType, ref this._builder);
            }

            this.Builder
                .DefinePropertyStateBag();

            this.Builder.CreateConstructor(
                this.BaseType, this.Fields);
            
            var type = 
                this.Builder.CreateType();

            var stateProp = 
                type.GetProperty("StateBag", BindingFlags.Instance | BindingFlags.Public);

            return new Entry { 
                Type = type,
                StateProperty = stateProp
            };
        } 
    }
}
