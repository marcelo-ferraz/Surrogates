using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using Surrogates.Mappers.Collections;
using Surrogates.Utilities.Mixins;

namespace Surrogates.Tactics
{
    public class Strategies 
    {
        private TypeBuilder _builder;
        private IList<Strategy> _strategies;

        internal Strategies(Type baseType,string name, ModuleBuilder moduleBuilder)
        {
            this.BaseType = baseType;
            
            if (string.IsNullOrEmpty(name))
            { name = string.Concat(baseType, "Proxy"); }
            
            try
            {
                Builder = moduleBuilder
                    .DefineType(name, TypeAttributes.Public, baseType);
            }
            catch (ArgumentException argEx)
            { throw new ProxyAlreadyMadeException(baseType, name, argEx); }
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

            this.Builder
                .CreateConstructor(this.BaseType, this.Fields);

            foreach (var strategy in _strategies)
            {
                strategy.Apply(BaseType, ref _builder);
            }

            return this.Builder.CreateType();
        }
    }
}
