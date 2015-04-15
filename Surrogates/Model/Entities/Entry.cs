using System;
using System.Reflection;

namespace Surrogates.Model.Entities
{
    public class Entry
    {
        public class Prop
        {
            public PropertyInfo Info { get; set;}
            public Object Value { get; set; }
        }

        public Type Type { get; set; }
        public PropertyInfo StateProperty { get; set; }
        public PropertyInfo ContainerProperty { get; set; }
        public Prop[] Properties { get; set; }
    }

}
