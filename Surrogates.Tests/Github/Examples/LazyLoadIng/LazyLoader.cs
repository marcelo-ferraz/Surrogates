using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Surrogates.Tests.Github.Examples.LazyLoadIng
{
    public class IdLazyLoader
    {
        private int _value;
        private bool _isDirty = false;

        private MockedRepository _repository = new MockedRepository();

        public int Load(string propertyName)
        {
            return object.Equals(_value, default(int)) ?
                (_value = _repository.Get<int>(propertyName)) :
                _value;
        }

        public void MarkAsDirty(int value)
        {
            _isDirty = true;
            _value = value;
        }
    }

    public class LazyLoader<T>
    {
        private T _value;
        private bool _isDirty = false;

        private MockedRepository _repository = new MockedRepository();
        
        public T Load(string propertyName)
        {
            return object.Equals(_value, default(T)) ? 
                (_value = _repository.Get<T>(propertyName)) : 
                _value;
        }

        public void MarkAsDirty(T value)
        {
            _isDirty = true;
            _value = value;
        }
    }
}
