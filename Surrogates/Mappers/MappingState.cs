using Surrogates.Expressions.Properties.Accessors;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace Surrogates.Mappers
{
    public class MappingState
    {
        public class PropertyList : List<PropertyInfo>
        {
            internal PropertyList()
            {
                Accessors = PropertyAccessor.None;
            }

            internal PropertyAccessor Accessors { get; set; }

            public void Add(PropertyAccessor accessor)
            {
                if ((Accessors & accessor) == accessor)
                {
                    throw new AccessorAlreadyOverridenException(accessor);
                }

                Accessors |= accessor;
            }

            public new void Clear()
            {
                base.Clear();
                Accessors = PropertyAccessor.None;
            }
        }

        public MappingState()
        {
            Methods = new List<MethodInfo>();
            Fields = new List<FieldInfo>();
            Properties = new PropertyList();
        }

        internal AssemblyBuilder AssemblyBuilder { get; set; }
        internal ModuleBuilder ModuleBuilder { get; set; }
        internal TypeBuilder TypeBuilder { get; set; } 
        internal IList<MethodInfo> Methods { get;set; }
        internal IList<FieldInfo> Fields { get; set; }
        internal PropertyList Properties { get; set; }
    }
}
