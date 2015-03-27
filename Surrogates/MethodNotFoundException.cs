using System;

namespace Surrogates
{
    public class MethodNotFoundException:Exception
    {
        public MethodNotFoundException(string methodName)
            : base(string.Format("Whilst this method '{0}' can exist inside the given type, it could not be found. Please revise its modifiers", methodName))
        { }
    }
}
