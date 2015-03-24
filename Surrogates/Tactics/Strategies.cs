using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;

namespace Surrogates.Tactics
{
    public class Strategies 
    {
        private TypeBuilder _builder;

        private Type _baseType;

        private IList<Strategy> _strategies;

        public void Add(Strategy strategy)
        {
            _strategies.Add(strategy);
        }

        public Type BaseType { get; set; }

        public TypeBuilder Builder { get; set; }

        public Mappers.Collections.FieldList Fields { get; set; }
        
        public Type Apply()
        {
            //create constructor
            foreach (var strategy in _strategies)
            {
                strategy.Apply(_baseType, ref _builder);
            }

            return _builder.DeclaringType;
        }
    }
}
