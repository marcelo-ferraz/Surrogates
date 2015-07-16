using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;

namespace Surrogates.Model.Entities
{
    [Serializable]
    public class DynamicObj : DynamicObject, IDictionary<string, object>
    {
        // The inner _dictionary.
        Dictionary<string, object> _dictionary
            = new Dictionary<string, object>();

        // This property returns the number of elements 
        // in the inner _dictionary. 
        public int Count
        {
            get
            {
                return _dictionary.Count;
            }
        }

        public override bool TryGetMember(
            GetMemberBinder binder, out object result)
        {
            _dictionary.TryGetValue(binder.Name, out result);
            return true;
        }

        public override bool TrySetMember(
            SetMemberBinder binder, object value)
        {
            _dictionary[binder.Name] = value;
            return true;
        }


        public void Add(string key, object value)
        {
            _dictionary.Add(key, value);
        }

        public bool ContainsKey(string key)
        {
            return _dictionary.ContainsKey(key);
        }

        public ICollection<string> Keys
        {
            get { return _dictionary.Keys; }
        }

        public bool Remove(string key)
        {
            return _dictionary.Remove(key);
        }

        public bool TryGetValue(string key, out object value)
        {
            return _dictionary.TryGetValue(key, out value);
        }

        public ICollection<object> Values
        {
            get { return _dictionary.Values; }
        }

        public object this[string key]
        {
            get { return _dictionary[key]; }
            set { _dictionary[key] = value; }
        }

        public void Add(KeyValuePair<string, object> item)
        {
            _dictionary.Add(item.Key, item.Value);
        }

        public void Clear()
        {
            _dictionary.Clear();
        }

        public bool Contains(KeyValuePair<string, object> item)
        {
            return _dictionary.Contains(item);
        }

        public void CopyTo(KeyValuePair<string, object>[] array, int arrayIndex)
        {
            ((ICollection<KeyValuePair<string, object>>)_dictionary).CopyTo(array, arrayIndex);
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(KeyValuePair<string, object> item)
        {
            return _dictionary.Remove(item.Key);
        }

        public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
        {
            return _dictionary.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _dictionary.GetEnumerator();
        }
    }
}