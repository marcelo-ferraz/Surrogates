using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Surrogates
{
    public class IncohenerentException : Exception
    {
        public IncohenerentException(string msg, params object[] args)
            : base(string.Format(msg, args))
        { }
    }
}
