using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Surrogates.Applications.Pooling
{
    public interface IContainsPool<T>
    {
        Pool<T> Pool { get; set; }
    }
}
