using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Surrogates.Applications.Cache
{
    public class CachedList
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

        private Dictionary<object, CachedList.Entry> _innerCollection;

        public CachedList()
        { _innerCollection = new Dictionary<object, Entry>(); }

        public object Add(object key, object value, TimeSpan timeout)
        {
            if (_innerCollection.ContainsKey(key))
            { return value; }

            var entry =
                new Entry(key, value, timeout);

            entry.Erase += this._innerCollection.Remove;

            _innerCollection.Add(key, entry);

            return value;
        }

        internal bool TryGet(object key, ref object result)
        {
            if (_innerCollection.ContainsKey(key))
            {
                result = _innerCollection[key];
                return true;
            }
            return false;
        }
    }
}
