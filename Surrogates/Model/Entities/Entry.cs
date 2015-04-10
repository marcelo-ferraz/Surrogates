using System;
using System.Reflection;

namespace Surrogates.Model.Entities
{
    public class Entry
    {
        public Type Type { get; set; }
        public PropertyInfo StateProperty { get; set; }
        public PropertyInfo ContainerProperty { get; set; }
    }

}
