using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Surrogates.Applications.LazyLoading
{
    public class LazyProperty
    {
        /// <summary>
        /// The name of the properties
        /// </summary>
        public string Name { get; internal set; }

        /// <summary>
        /// The value, of that properties
        /// </summary>
        public object Value { get; set; }

        /// <summary>
        /// If it was modified, by other than the lazy loading feature
        /// </summary>
        public bool IsDirty { get; internal set; }
    }
}
