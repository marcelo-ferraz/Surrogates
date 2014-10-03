using System.Collections.Generic;
using System.Reflection;
using Surrogates.Expressions.Properties.Accessors;
using Surrogates.Mappers.Entities;

namespace Surrogates.Mappers.Collections
{
    public class PropertyList : List<Property>
    {
        private MappingState _owner;

        internal PropertyList(MappingState state)
        {
            this._owner = state;
            Accessors = PropertyAccessor.None;
        }

        internal PropertyAccessor Accessors { get; set; }

        public void Add(PropertyInfo property)
        {
            var prop =
                new Property(_owner, _owner.TypeBuilder) { Original = property };

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