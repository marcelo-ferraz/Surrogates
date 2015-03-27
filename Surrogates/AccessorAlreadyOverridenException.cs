using Surrogates.Expressions.Accessors;
using System;

namespace Surrogates
{
    public class AccessorAlreadyOverridenException : Exception
    {
        private string p;
        private Expressions.Accessors.PropertyAccessor accessor;

        private static string GetMsg(PropertyAccessor accessor)
        {
            return string.Format(
                "The Accessor '{0}' already was overriden.",
                Enum.GetName(typeof(PropertyAccessor), accessor));
        }

        public AccessorAlreadyOverridenException(PropertyAccessor accessor, ArgumentException argEx = null)
            : base(GetMsg(accessor), argEx) { }

        public AccessorAlreadyOverridenException(string msg)
            : base(msg) { }
    }
}
