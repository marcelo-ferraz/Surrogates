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
        private static AndExpression<T> AddBeforeAndAfter<T>(this AndExpression<T> expr, Delegate before, Delegate after)
        {
            if (before != null)
            {
                expr = expr
                    .And
                    .AddProperty(before.GetType(), "Before", before);
            }

            if (after != null)
            {
                expr = expr
                    .And
                    .AddProperty(after.GetType(), "After", after);
            }

            return expr;
        }

        /// <summary>
        /// Adds notifiers for the changes made inside the properties inside a collection
        /// </summary>
        /// <typeparam name="L"></typeparam>
        /// <typeparam name="I"></typeparam>
        /// <param name="that"></param>
        /// <param name="before"></param>
        /// <param name="after"></param>
        /// <returns></returns>
        public static AndExpression<L> NotifyChanges<L, I>(this ApplyExpression<L> that, Action<L, I, object> before = null, Action<L, I, object> after = null)
            where L : class, ICollection<I>
            where I : class
        {
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
                .Using<ChangesListNotifierInterceptor<L, I>>("ListNotifierInterceptor", "Set", typeParameters: new[] { typeof(L) })
                .AddBeforeAndAfter(before, after);
            
            if (isIList)
            { 
                expr = expr
                    .And
                    .Replace
                    .Property("Item")
                    .Accessors(a =>
                        a.Setter.Using<ChangesListNotifierInterceptor<L, I>>("ListNotifierInterceptor", "Set", typeParameters: new[] { typeof(L) }));
            }

            if (!ext.Container.Has<I>())
            {
                ext.Container.Map(m =>
                    m.From<I>().Apply.NotifyChanges<I>());
            }

            return expr
                .And
                .AddProperty<NotifierBeforeAndAfter<L, I>>("ListNotifierInterceptor")
                .And
                .AddInterface<IContainsNotifier4<L, I>>();
        }

        public static AndExpression<T> NotifyChanges<T>(this ApplyExpression<T> that, Action<T, object> before = null, Action<T, object> after = null)         
            where T : class
        {
            var ext = new ShallowExtension<T>();
            Pass.On(that, ext);

            var expr = ext
                .Factory                
                .Replace
                .Properties(p => true) // all props
                .Accessors(a =>
                    a.Setter.Using<ChangesNotifierInterceptor<T>>("NotifierInterceptor", "Set"))
                .AddBeforeAndAfter(before, after);
            
            return expr
                .And
                .AddInterface<IContainsNotifier4<T>>()
                .And
                .AddProperty<NotifierBeforeAndAfter<T>>("NotifierInterceptor");
        }

    }
}
