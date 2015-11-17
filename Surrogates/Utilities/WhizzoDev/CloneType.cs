using Surrogates.Utilities.SDILReader;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace Surrogates.Utilities.WhizzoDev
{
    /// <summary>
    /// Enumeration that defines the type of cloning of a srcField.
    /// Used in combination with the CloneAttribute
    /// </summary>
    public enum CloneType
    {     
        Shallow,
        Deep
    }
}