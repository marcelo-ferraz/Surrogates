using System;

namespace Surrogates.OldExpressions.Properties.Accessors
{
    [Flags]
    public enum PropertyAccessor : byte
    {
        None = 0,
        Set = 1,
        Get = 2
    }
}
