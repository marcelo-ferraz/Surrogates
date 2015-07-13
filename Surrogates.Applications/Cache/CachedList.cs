using Surrogates.Applications.Utilities;
using System;
using System.Collections.Generic;
using System.Threading;

namespace Surrogates.Applications.Cache
{
    public class CachedList : Locked4RW
    {
        public class Entry
        {
            public event Func<object, bool> Erase;

            public object Key { get; set; }

            public object Value { get; set; }

            public TimeSpan Timeout { get; set; }

            public Entry(object key, object value, TimeSpan timeout)
            {
                this.Key = key;
                this.Value = value;
                this.Timeout = timeout;

                new Timer(s =>
                {
                    var that =
                        (Entry)s;

                    if (that.Erase != null)
                    { that.Erase(Key); }
                },
                this,
                timeout.Milliseconds,
                timeout.Milliseconds);
            }
        }

        private static Dictionary<object, CachedList.Entry> _innerCollection;

        static CachedList()
        {
            _innerCollection = 
                new Dictionary<object, Entry>(); 
        }

        public object Add(object key, object value, TimeSpan timeout)
        {
            bool containsKey = false;

            this.Read(() =>
                containsKey = _innerCollection.ContainsKey(key));

            if (containsKey) 
            { return value; }

            var entry =
                new Entry(key, value, timeout);

            entry.Erase += 
                _innerCollection.Remove;

            this.Write(() =>
                _innerCollection.Add(key, entry));

            return value;
        }

        internal bool TryGet(object key, ref object value)
        {
            object result = null;

            this.Read(
                () =>
                {

                    if (_innerCollection.ContainsKey(key))
                    { result = _innerCollection[key].Value; }
            
                });

            return (value = result) != null;
        }
    }
}
