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
            where L : class, ICollection<I>
            where I : class
        {
            var ext = new ShallowExtension<L>();
            Pass.On(that, ext);

            var methodsNames = 
                typeof(IList<I>).IsAssignableFrom(typeof(L)) ?
                new[] { "Add", "Insert", "set_Item" } :
                new[] { "Add" };

            var expr = ext
                .Factory
                .Replace
                .Methods(methodsNames, false)
                .Using<ChangesListListenerInterceptor<I>>("Set")
                .And
                .AddProperty<Action<L, I, object>>("Notifier", listener);
            
            ext.Container.Map(m =>
                m.From<I>()
                .Replace
                .Properties(p => true) // all props
                .Accessors(a =>
                    a.Setter.Using<ChangesNotifierInterceptor<I>>("Set")));
                        
            return expr;
            //ICollection<T>.Add(T item)
            //IList<I>.Insert(int index, T item)
            //IList<I>.set_Item(T item)
        }
    }
}
