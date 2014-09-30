using Surrogates.Expressions.Properties.Accessors;
using Surrogates.Mappers.Collections;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace Surrogates.Mappers.Entities
{
    public class MappingState
    {
        public MappingState()
        {
            Methods = new List<MethodInfo>();
            Fields = new FieldList(owner: this);
        }

        private TypeBuilder _typeBuilder;

        public AssemblyBuilder AssemblyBuilder { get; set; }
        public ModuleBuilder ModuleBuilder { get; set; }

        public TypeBuilder TypeBuilder
        {
            get { return _typeBuilder; }
            set
            {
                _typeBuilder = value;
                Properties = new PropertyList(this);
            }
        }

        internal IList<MethodInfo> Methods { get; set; }
        internal FieldList Fields { get; set; }
        internal PropertyList Properties { get; set; }
    }
}