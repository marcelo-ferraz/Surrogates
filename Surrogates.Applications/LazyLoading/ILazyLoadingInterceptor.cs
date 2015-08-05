using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Surrogates.Aspects.LazyLoading
{
    public interface ILazyLoadingInterceptor
    {
        /// <summary>
        /// A list of the watched properties by this interceptor
        /// </summary>
        IDictionary<string, LazyProperty> Properties { get; }
    }
}
