using Surrogates.Tactics;
using Surrogates.Utilities.Mixins;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;

namespace Surrogates.Model.Entities
{
    public class NewProperty
    {
        private object _defaultValue;
        private PropertyBuilder _builder;
        private TypeBuilder _typeBuilder;

        internal NewProperty(TypeBuilder typeBuilder)
        {
            _typeBuilder = typeBuilder;
        }

        public Type Type { get; set; }
        
        public string Name { get; set; }

        public PropertyBuilder GetBuilder()
        {
            return _builder ?? (_builder = _typeBuilder.DefineNewProperty(Type, Name, DefaultValue)); 
        }

        public object DefaultValue
        {
            get { return _defaultValue; }
            set 
            {
                if (!Type.IsAssignableFrom(value.GetType()))
                {
                    throw new IncohenerentNewProperty(
                        "The default value for the newly informed property '{0}' is of type incorrect. Informed Type: {1}; Default's value: {2};", 
                        Name, 
                        Type, 
                        DefaultValue.GetType());
                }

                _defaultValue = value; 
            }
        }
    }
}
