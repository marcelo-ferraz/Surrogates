using System;

namespace Surrogates
{
    public class ProxyAlreadyMadeException : Exception
    {
        private static string GetMsg(Type type, string name)
        {
            return string.Format(
                "The Type '{0}' already was mapped to a proxy. If you wish to create more than one proxy for '{0}', please create it with a name different from '{1}'.",
                type, 
                name);
        }

        public ProxyAlreadyMadeException(Type type, string name, ArgumentException argEx)
            : base(GetMsg(type, name), argEx) { }
    }
}
