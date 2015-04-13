using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Surrogates
{
    public class IncohenerentNewProperty : Exception
    {
        public IncohenerentNewProperty(string msg, params object[] args)
            : base(string.Format(msg, args))
        { }
    }
}
