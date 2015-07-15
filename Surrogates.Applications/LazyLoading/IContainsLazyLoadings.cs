using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Surrogates.Applications.LazyLoading
{
    public interface IContainsLazyLoadings
    {
        /// <summary>
        /// The Lazy loading Interceptor
        /// </summary>
        ILazyLoadingInterceptor LazyLoadingInterceptor { get; set; }
    }
}
