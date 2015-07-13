using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Surrogates.Applications.LazyLoading
{
    public class LazyProperty
    {
        /// <summary>
        /// The name of the property
        /// </summary>
        public string Name { get; internal set; }

        /// <summary>
        /// The value, of that property
        /// </summary>
        public object Value { get; set; }

        /// <summary>
        /// If it was modified, by other than the lazy loading feature
        /// </summary>
        public bool IsDirty { get; internal set; }
    }
}
