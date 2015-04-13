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
            
            this.NewProperties = 
                new List<NewProperty>();

            this.AddProperty<dynamic>("StateBag");
            this.AddProperty<SurrogatesContainer>("Container");
        }
        
        public Type BaseType { get; set; }

        public TypeBuilder Builder { get; set; }
        
        public FieldList Fields { get; set; }

        public List<NewProperty> NewProperties { get; set; }

        private void AddProperty<T>(string name)
        {
            this.NewProperties.Add(new NewProperty(Builder) { 
                Name = name,
                Type = typeof(T)
            });
        }

        private PropertyInfo GetProperty(string name, Type holder)
        {
            return holder.GetProperty(name, BindingFlags.Instance | BindingFlags.Public);
        }

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

            // this will define all the properties not previously asked before
            foreach (var property in NewProperties)
            {
                property.GetBuilder();
            }
            
            this.Builder.CreateConstructor(
                this.BaseType, this.Fields);
            
            var newType = 
                this.Builder.CreateType();

            var stateProp = 
                GetProperty("StateBag", newType);

            var containerProp =
                GetProperty("Container", newType);

            return new Entry { 
                Type = newType,
                StateProperty = stateProp,
                ContainerProperty = containerProp
            };
        } 
    }
}
