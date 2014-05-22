using Surrogates.Expressions.Classes;
using Surrogates.Mappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;

namespace Surrogates.Expressions.Properties.Accessors
{
    [Flags]
    public enum PropertyAccessor : byte
    {
        None = 0,
        Set = 1,
        Get = 2
    }
}
