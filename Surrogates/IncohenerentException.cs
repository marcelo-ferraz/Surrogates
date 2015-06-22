using System;

namespace Surrogates
{
    public class IncoherenceException : Exception
    {
        public IncoherenceException(string msg, params object[] args)
            : base(string.Format(msg, args))
        { }
    }
}
