using System;
using System.Reflection;
using System.Reflection.Emit;
using Surrogates.Utilities.Mixins;
using Surrogates.Tactics;

namespace Surrogates.Model
{
    public class Property : IComparable
    {
        private FieldBuilder _field;
        private TypeBuilder _typeBuilder;
        private PropertyBuilder _propBuilder;
        private Strategy _owner;

        internal Property(Strategy strategy)
        {
            _owner = strategy;
            _typeBuilder = strategy.TypeBuilder;
        }

        public PropertyInfo Original { get; set; }

        public PropertyBuilder Builder
        {
            get
            {
                return _propBuilder ??
                    (_propBuilder = _typeBuilder.DefineProperty(
                    Original.Name,
                    Original.Attributes | PropertyAttributes.HasDefault,
                    Original.PropertyType,
                    Type.EmptyTypes));
            }
            set { _propBuilder = value; }
        }

        public FieldBuilder Field
        {
            get
            {
                return _field ??
                    (_field = _typeBuilder.DefineFieldFromProperty(Original));
            }
            set { _field = value; }
        }


        public int CompareTo(object obj)
        {
            if (obj is Property)
            { return this.CompareTo(((Property)obj).Original); }

            if (!(obj is PropertyInfo))
            { throw new NotSupportedException(); }

            var prop =
                obj as PropertyInfo;

            return this.Original.Name.CompareTo(prop.Name);
        }
    }
}