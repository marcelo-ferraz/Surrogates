using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Surrogates.Aspects.Pooling
{
    public interface IContainsPool<T>
    {
        Pool<T> Pool { get; set; }
    }
}
