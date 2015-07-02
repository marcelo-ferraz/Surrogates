using Surrogates.Utilities;
using Surrogates.Utilities.WhizzoDev;
using System;
using System.Collections.Generic;

namespace Surrogates.Applications.NotifyChanges
{
    public class ChangesListListenerInterceptor<I>        
        where I : class
    {
        private static Action<I, object> _emptyAction = new Action<I, object>((i, v) => {});

        public void Set<L>(BaseContainer4Surrogacy s_Container, Delegate s_method, object[] s_args, Action<L, I, object> s_Before, Action<L, I, object> s_After, L s_instance)
            where L : class, ICollection<I>
        {
            var item = 
                s_args[s_args.Length - 1];

            if (item != null)
            {
                var before = s_Before != null ? 
                    new Action<I, object>((i, v) => s_Before(s_instance, i, v)) : 
                    _emptyAction;

                var after = s_After != null ? 
                    new Action<I, object>((i, v) => s_After(s_instance, i, v)) : 
                    _emptyAction;
                                
                var newItem = s_Container.Invoke<I>(
                    stateBag: new 
                    { 
                        Before = before,
                        After =  after
                    });

                #region Observations
                /* 
             * Obs.1: As the setter from the indexed property, 'this' (set_Item), has the value argument 
             * instead of item argument, it is best to use the s_args 
             * 
             * Obs.2: The argument that holds the item to be inserted on the list it is always the latest, 
             * for 'IList<I>.Add', 'ICollection<I>.Insert' methods, and the setter for the IList<I> indexed property
             */
                #endregion

                s_args[s_args.Length - 1] =
                    CloneHelper.Merge(s_args[s_args.Length - 1], newItem);
            }

            s_method.DynamicInvoke(s_args);
        }
    }
}
