using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Surrogates.Model.Entities
{
    public class NewAttribute
    {
        public Type Type { get; set; }
        public string MemberName { get; set; }
        public AttributeTargets Targets { get; set; }
        public object[] Arguments { get; set; }
    }
}
