using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Surrogates
{
    public class IncoherenceException : Exception
    {
        public IncoherenceException(string msg, params object[] args)
            : base(string.Format(msg, args))
        { }
    }
}
