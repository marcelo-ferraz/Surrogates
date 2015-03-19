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

        public void Add(Strategy strategy)
        { 

        }

        public Type BaseType { get; set; }

        public TypeBuilder Builder { get; set; }

        public Mappers.Collections.FieldList Fields { get; set; }
    }
}
