using Surrogates.Model.Collections;
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

        internal Strategies(Type baseType,string name, ModuleBuilder moduleBuilder)
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

        public void Add(Strategy strategy)
        {
            _strategies.Add(strategy);
        }

        public TypeBuilder Builder
        {
            get { return _builder; }
            set { _builder = value; }
        }
        public Type BaseType { get; set; }
        public FieldList Fields { get; set; }

        public Type Apply()
        {
            foreach (var strategy in _strategies)
            {
                strategy.Apply(BaseType, ref _builder);
            }

            this.Builder
                .CreateConstructor(this.BaseType, this.Fields);

            return this.Builder.CreateType();
        }
    }
}
