using Surrogates.Applications.NotifyChanges;
using Surrogates.Expressions;
using Surrogates.Utilities;
using System;
using System.Collections.Generic;

namespace Surrogates.Applications
{
    public static class NotifyChangesMixins
    {    
        public static AndExpression<L> NotifyChanges<L, I>(this ApplyExpression<L> that, Action<L, I, object> listener)
            where L : class, IList<I> 
            where I : class
        {
            var ext = new ShallowExtension<L>();
            Pass.On(that, ext);

            return ext
                .Factory
                .Replace
                .Method("set_Item")
                .Using<ChangesListListenerInterceptor<L, I>>("Set")
                .And
                .AddProperty<Action<L, I, object>>("Notifier", listener);

            //ICollection<T>.Add(T item)
            //IList<I>.Insert(int index, T item)
            //IList<I>.set_Item(T item)
        }
    }
}
