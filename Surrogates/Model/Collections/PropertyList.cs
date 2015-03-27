using Surrogates.Expressions.Accessors;
using Surrogates.Tactics;
using System.Collections.Generic;
using System.Reflection;

namespace Surrogates.Model.Collections
{
    public class PropertyList : List<Property>
    {
        private Strategy _owner;

        internal PropertyList(Strategy strategy)
        {
            this._owner = strategy;
            Accessors = PropertyAccessor.None;
        }

        internal PropertyAccessor Accessors { get; set; }

        public void Add(PropertyInfo property)
        {
            var prop =
                new Property(_owner) { Original = property };

            var index =
                this.BinarySearch(prop);

            this.Insert(index < 0 ? ~index : index, prop);
        }

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
}