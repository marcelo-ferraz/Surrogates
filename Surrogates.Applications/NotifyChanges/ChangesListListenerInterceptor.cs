using Surrogates.Utilities;
using Surrogates.Utilities.WhizzoDev;
using System;
using System.Collections.Generic;
using System.Dynamic;

namespace Surrogates.Aspects.NotifyChanges
{
    public class ChangesListNotifierInterceptor<L, I> : NotifierBeforeAndAfter<L,I>
        where L : class, ICollection<I>
        where I : class
    {
        public override event Action<L, I, object> Before;

        public override event Action<L, I, object> After;
                
        public void Set(BaseContainer4Surrogacy p_Container, Delegate s_method, object[] s_args, Action<L, I, object> p_Before, Action<L, I, object> p_After, L s_instance)            
        {
            var item = 
                s_args[s_args.Length - 1];

            if (item != null)
            {
                var before = p_Before != null ? 
                    new Action<I, object>((i, v) => p_Before(s_instance, i, v)) : 
                    null;

                var after = p_After != null ? 
                    new Action<I, object>((i, v) => p_After(s_instance, i, v)) : 
                    null;
                                
                var newItem = p_Container.Invoke<I>(
                    stateBag: bag =>
                    {
                        bag.Before = before;
                        bag.After = after;
                    });

                #region Observations
                /* 
                 *  Obs.1: As the setter from the indexed properties, 'this' (set_Item), has the value argument 
                 * instead of item argument, it is best to use the s_args 
                 * 
                 *  Obs.2: The argument that holds the item to be inserted on the list it is always the latest, 
                 * for 'IList<I>.Add', 'ICollection<I>.Insert' properties, and the setter for the IList<I> indexed properties
                 */
                #endregion

                s_args[s_args.Length - 1] =
                    CloneHelper.Merge(s_args[s_args.Length - 1], newItem);

                var @int = ((IContainsNotifier4<I>)newItem)
                    .NotifierInterceptor;

                @int.Before +=
                    (it, val) =>
                        Call(this.Before, s_instance, it, val);

                @int.After +=
                    (it, val) => 
                        Call(this.After, s_instance, it, val);                
            }

            s_method.DynamicInvoke(s_args);
        }
    }
}
