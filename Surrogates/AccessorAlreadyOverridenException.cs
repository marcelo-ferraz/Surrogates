using Surrogates.Expressions.Properties.Accessors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Surrogates
{
    public class AccessorAlreadyOverridenException : Exception
    {
        private static string GetMsg(PropertyAccessor accessor)
        {
            return string.Format(
                "The Accessor '{0}' already was overriden.",
                Enum.GetName(typeof(PropertyAccessor), accessor));
        }

        public AccessorAlreadyOverridenException(PropertyAccessor accessor, ArgumentException argEx = null)
            : base(GetMsg(accessor), argEx) { }
    }
}
