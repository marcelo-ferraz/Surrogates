using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Surrogates
{   
    public class PropertyNotFoundException : Exception
    {
        public PropertyNotFoundException(string propName)
            : base(string.Format("Whilst this property '{0}' can exist inside the given method, it could not be found. Please revise its modifiers", propName))
        { }
    }  
}
