using System.Collections.Generic;
using System.Reflection.Emit;

namespace Surrogates.Model.Entities
{
    public class OverridenMethod
    {
        public OverridenMethod()
        {
            Locals = new Dictionary<string, LocalBuilder>();
        }
        
        public MethodBuilder Builder { get; set; }
        
        public LocalBuilder Return { get; set; }
        
        public ILGenerator Generator { get; set; }

        public IDictionary<string, LocalBuilder> Locals { get; set; }
    }
}
