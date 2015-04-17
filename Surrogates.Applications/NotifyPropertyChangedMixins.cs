using Surrogates.Expressions;
using Surrogates.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Surrogates.Applications
{
    public static class NotifyChangesMixins
    {
        public class NotifyChangesInterceptor<L>
            where L:class 
        {
            L _list;

            internal L Get(BaseContainer4Surrogacy s_container, L s_instance)
            {
                if(_list == null) {
                    _list = s_container.Invoke<L>();
                }

                throw new NotImplementedException();
            }

            internal void Set(L s_value)
            {
                throw new NotImplementedException();
            }
        }

        public static AndExpression<L> NotifyChanges<L, I>(this ApplyExpression<L> that, Func<L, object> prop)
            where L : class, IList<I> 
        {
            var ext = new ShallowExtension<L>();
            Pass.On(that, ext);

            return ext
                .Factory
                .Replace
                .This(prop)
                .Accessors(a => a
                    .Getter.Using<NotifyChangesInterceptor<L>>(i => (Func<BaseContainer4Surrogacy, L, L>)i.Get)
                    .And
                    .Getter.Using<NotifyChangesInterceptor<L>>(i => (Action<L>)i.Set)
                );
        }
    }
}
