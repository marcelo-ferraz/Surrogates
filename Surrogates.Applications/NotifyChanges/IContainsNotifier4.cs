using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Surrogates.Aspects.NotifyChanges
{
    public interface IContainsNotifier4<L, I>
        where L : class, ICollection<I>
        where I : class
    {
        NotifierBeforeAndAfter<L, I> ListNotifierInterceptor { get; }
    }
    
    public interface IContainsNotifier4<T>
        where T: class
    {
        NotifierBeforeAndAfter<T> NotifierInterceptor { get; }
    }
}
