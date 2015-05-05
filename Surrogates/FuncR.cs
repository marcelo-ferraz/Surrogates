using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Surrogates
{

    public delegate R FuncR<T, R>(ref T arg);
}
