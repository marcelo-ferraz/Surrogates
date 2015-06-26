using Surrogates.Utilities.SDILReader;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace Surrogates.Utilities.WhizzoDev
{
    /// <summary>
    /// CloningAttribute for specifying the cloneproperties of a field.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class CloneAttribute : Attribute
    {
        private CloneType _clonetype;
        public CloneAttribute()
        {

        }

        public CloneType CloneType
        {
            get { return _clonetype; }
            set { _clonetype = value; }
        }
    }
}