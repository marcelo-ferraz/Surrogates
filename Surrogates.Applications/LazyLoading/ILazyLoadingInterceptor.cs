using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Surrogates.Applications.LazyLoading
{
    public interface ILazyLoadingInterceptor
    {
        IDictionary<string, LazyProperty> Properties { get; }
    }
}
