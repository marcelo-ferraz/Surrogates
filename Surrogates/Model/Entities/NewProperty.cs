using Surrogates.Utilities.Mixins;
using System;
using System.Reflection.Emit;
using Surrogates.Tactics;

namespace Surrogates.Model.Entities
{
    public class NewProperty
    {
        private object _defaultValue;
        private PropertyBuilder _builder;
        private Strategies _strategies;
        
        internal NewProperty(Strategies strategies)
        {
            _strategies = strategies;
        }

        public Type Type { get; set; }
        
        public string Name { get; set; }

        public PropertyBuilder GetBuilder()
        {
            return _builder ?? (_builder = _strategies.DefineNewProperty(Type, Name)); 
        }

        public object DefaultValue
        {
            get { return _defaultValue; }
            set 
            {
                if (value != null && !Type.IsAssignableFrom(value.GetType()))
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
