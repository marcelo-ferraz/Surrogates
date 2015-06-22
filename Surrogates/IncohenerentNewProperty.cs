using System;

namespace Surrogates
{
    public class IncohenerentNewProperty : Exception
    {
        public IncohenerentNewProperty(string msg, params object[] args)
            : base(string.Format(msg, args))
        { }
    }
}
