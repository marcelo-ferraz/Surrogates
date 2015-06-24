using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;

namespace Surrogates.Model.Entities
{
    public class OverridenMethod
    {
        public MethodBuilder Builder { get; set; }
        public LocalBuilder Return { get; set; }
        public ILGenerator Generator { get; set; }
    }
}
