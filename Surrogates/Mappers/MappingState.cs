using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace Surrogates.Mappers
{
    public class MappingState
    {
        public MappingState()
        {
            Methods = new List<MethodInfo>();
            Fields = new List<FieldInfo>();
        }

        internal AssemblyBuilder AssemblyBuilder { get; set; }
        internal ModuleBuilder ModuleBuilder { get; set; }
        internal TypeBuilder TypeBuilder { get; set; } 
        internal IList<MethodInfo> Methods { get;set; }
        internal IList<FieldInfo> Fields { get; set; }

        //internal MethodInfo PushMethod(int i)
        //{
        //    var method = Methods[i];
        //    Methods.RemoveAt(i);
        //    return method;
        //}

        //internal MethodInfo Push(MethodInfo method)
        //{
        //    Methods.Remove(method);
        //    return method;
        //}

        public List<PropertyInfo> Properties { get; set; }
    }
}
