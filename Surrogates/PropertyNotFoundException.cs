using System;

namespace Surrogates
{   
    public class PropertyNotFoundException : Exception
    {
        public PropertyNotFoundException(string propName)
            : base(string.Format("Whilst this property '{0}' can exist inside the given type, it could not be found. Please revise its modifiers", propName))
        { }
    }  
}
