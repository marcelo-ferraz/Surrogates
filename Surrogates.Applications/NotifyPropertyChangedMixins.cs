using Surrogates.Applications.Model;
using Surrogates.Applications.NotifyChanges;
using Surrogates.Expressions;
using Surrogates.Tactics;
using Surrogates.Utilities;
using Surrogates.Utilities.WhizzoDev;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Surrogates.Applications
{
    public static class NotifyChangesMixins
    {
        public static AndExpression<L> NotifyChanges<L, I>(this ApplyExpression<L> that, Action<L, I, object> before = null, Action<L, I, object> after = null)
            where L : class, ICollection<I>
            where I : class
        {
            if (before == null && after == null)
            {
                throw new ArgumentNullException(
                    "You have to provide at least one of the notifiers, either before or after!");
            }

            var ext = new ShallowExtension<L>();
            Pass.On(that, ext);

            var isIList = 
                typeof(IList<I>).IsAssignableFrom(typeof(L));
            
            //ICollection<T>.Add(T item)
            //IList<I>.Insert(int index, T item)
            //IList<I>.set_Item(T item)

            var methodsNames = 
                isIList ?
                new[] { "Add", "Insert" } :
                new[] { "Add" };

            var expr = ext
                .Factory
                .Replace
                .Methods(methodsNames)
                .Using<ChangesListListenerInterceptor<I>>("Set", typeParameters : new [] { typeof(L) });

            if(before != null)
            {
                expr = expr
                    .And
                    .AddProperty<Action<L, I, object>>("Before", before);
            }

            if(after != null)
            {
                expr = expr
                    .And
                    .AddProperty<Action<L, I, object>>("After", after); 
            }
            

            if (isIList)
            { 
                expr = expr
                    .And
                    .Replace
                    .Property("Item")
                    .Accessors(a =>
                        a.Setter.Using<ChangesListListenerInterceptor<I>>("Set", typeParameters: new[] { typeof(L) }));
            }

            ext.Container.Map(m =>
                m.From<I>()
                .Replace
                .Properties(p => true) // all props
                .Accessors(a =>
                    a.Setter.Using<ChangesNotifierInterceptor<I>>("Set")));
                        
            return expr;
        }
    }
}
