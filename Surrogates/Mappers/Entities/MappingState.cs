using Surrogates.Expressions.Properties.Accessors;
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
            Fields = new List<FieldInfo>();
        }

        private TypeBuilder _typeBuilder;

        internal AssemblyBuilder AssemblyBuilder { get; set; }
        internal ModuleBuilder ModuleBuilder { get; set; }
        
        internal TypeBuilder TypeBuilder 
        {
            get { return _typeBuilder; }
            set 
            {
                _typeBuilder = value;
                Properties = new PropertyList(this);
            } 
        }
 
        internal IList<MethodInfo> Methods { get;set; }
        internal IList<FieldInfo> Fields { get; set; }
        internal PropertyList Properties { get; set; }
    }
}
