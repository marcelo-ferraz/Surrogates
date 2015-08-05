using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Surrogates.Aspects.Tests.Model
{
    public class SimpletonList : IList<Simpleton>
    {
        private List<Simpleton> _simpleton = new List<Simpleton>();

        public int IndexOf(Simpleton item)
        {
            return _simpleton.IndexOf(item);
        }

        public virtual void Insert(int index, Simpleton item)
        {
            _simpleton.Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            _simpleton.RemoveAt(index);
        }

        public virtual Simpleton this[int index]
        {
            get { return _simpleton[index]; }
            set { _simpleton[index] = value; }
        }

        public virtual void Add(Simpleton item)
        {
            _simpleton.Add(item);
        }

        public void Clear()
        {
            _simpleton.Clear();
        }

        public bool Contains(Simpleton item)
        {
            return _simpleton.Contains(item);
        }

        public void CopyTo(Simpleton[] array, int arrayIndex)
        {
            _simpleton.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get { return _simpleton.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(Simpleton item)
        {
            return _simpleton.Remove(item);
        }

        public IEnumerator<Simpleton> GetEnumerator()
        {
            return _simpleton.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _simpleton.GetEnumerator();
        }
    }
}