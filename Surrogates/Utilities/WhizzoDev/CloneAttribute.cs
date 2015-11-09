using Surrogates.Utilities.SDILReader;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace Surrogates.Utilities.WhizzoDev
{
    /// <summary>
    /// CloningAttribute for specifying the cloneproperties of a property or class.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Class, AllowMultiple = false)]
    public class CloneAttribute : Attribute
    {
        private CloneType _clonetype;
        
        public CloneAttribute() { }

        public CloneAttribute(CloneType cloneType, params string[] aliases)
        {
            CloneType = cloneType;
            Aliases = aliases;
        }

        public CloneType CloneType { get; set; }

        public string[] Aliases { get; set; }
    }
}