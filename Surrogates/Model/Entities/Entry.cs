using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Surrogates.Model.Entities
{
    public class Entry
    {
        public Type Type { get; set; }
        public PropertyInfo StateProperty { get; set; }
    }

}
