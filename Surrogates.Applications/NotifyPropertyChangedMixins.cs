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
                .This(l => (Action<I>) l.Add)
                .Using<ChangesListListenerInterceptor<I>>("Set", typeParameters : new [] { typeof(L) })
                .And
                .AddProperty<Action<L, I, object>>("Notifier", listener);

            var meth = ext.Strategies.Builder.DefineMethod("DoMerge", MethodAttributes.Public, CallingConventions.Standard, typeof(I), new [] { typeof(I), typeof(I) });

            var sourceParam =
                meth.DefineParameter(1, ParameterAttributes.In, "source");

            var destinationParam =
                meth.DefineParameter(2, ParameterAttributes.In, "destination");

            var source = Activator.CreateInstance<I>();
            var dest = Activator.CreateInstance<I>();

            CloneHelper.CreateMerger(source, dest, meth.GetILGenerator());





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
