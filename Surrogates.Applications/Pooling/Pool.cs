﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace Surrogates.Applications.Pooling
{
    public sealed class Pool<T> : IDisposable
    {
        private interface IItemStore
        {
            T Fetch();
            void Store(T item);
            int Count { get; }
        }

        private class QueueStore : Queue<T>, IItemStore
        {
            public QueueStore(int capacity)
                : base(capacity) { }

            public T Fetch()
            {
                return Dequeue();
            }

            public void Store(T item)
            {
                Enqueue(item);
            }
        }

        private class StackStore : Stack<T>, IItemStore
        {
            public StackStore(int capacity)
                : base(capacity) { }

            public T Fetch()
            {
                return Pop();
            }

            public void Store(T item)
            {
                Push(item);
            }
        }

        private class CircularStore : IItemStore
        {
            private List<Slot> _slots;
            private int _freeSlotCount;
            private int _position = -1;

            public CircularStore(int capacity)
            {
                _slots = new List<Slot>(capacity);
            }

            public T Fetch()
            {
                if (Count == 0)
                { throw new InvalidOperationException("The buffer is empty."); }

                int startPosition = _position;
                do
                {
                    Advance();
                    Slot slot = _slots[_position];
                    if (!slot.IsInUse)
                    {
                        slot.IsInUse = true;
                        --_freeSlotCount;
                        return slot.Item;
                    }
                } while (startPosition != _position);
                throw new InvalidOperationException("No free slots.");
            }

            public void Store(T item)
            {
                Slot slot = _slots.Find(s => object.Equals(s.Item, item));
                if (slot == null)
                {
                    slot = new Slot(item);
                    _slots.Add(slot);
                }
                slot.IsInUse = false;
                ++_freeSlotCount;
            }

            public int Count
            {
                get { return _freeSlotCount; }
            }

            private void Advance()
            {
                _position = (_position + 1) % _slots.Count;
            }

            class Slot
            {
                public Slot(T item)
                {
                    this.Item = item;
                }

                public T Item { get; private set; }
                public bool IsInUse { get; set; }
            }
        }

        private bool _isDisposed;
        private LoadingMode _loadingMode;
        private IItemStore _itemStore;
        private int _size;
        private int _count;
        private Semaphore _sync;

        private Func<Pool<T>, dynamic> _factory;

        public Pool(int size, Func<Pool<T>, dynamic> factory)
            : this(size, factory, LoadingMode.Lazy, AccessMode.FIFO) { }

        public Pool(int size, Func<Pool<T>, dynamic> factory, LoadingMode loadingMode, AccessMode accessMode)
        {
            if (size <= 0)
            { 
                throw new ArgumentOutOfRangeException(
                    "size", size, "Argument 'size' must be greater than zero."); 
            }
            
            if (factory == null)
            { throw new ArgumentNullException("factory"); }

            this._size = size;
            this._factory = factory;
            
            this._sync = 
                new Semaphore(size, size);
            
            this._loadingMode = 
                loadingMode;
            
            this._itemStore = 
                CreateItemStore(accessMode, size);

            if (loadingMode == LoadingMode.Eager)
            {
                PreloadItems();
            }
        }

        private T AcquireEager()
        {
            lock (_itemStore)
            {
                return _itemStore.Fetch();
            }
        }

        private T AcquireLazy()
        {
            lock (_itemStore)
            {
                if (_itemStore.Count > 0)
                {
                    return _itemStore.Fetch();
                }
            }
            Interlocked.Increment(ref _count);
            return _factory(this);
        }

        private T AcquireLazyExpanding()
        {
            bool shouldExpand = false;
            if (_count < _size)
            {
                int newCount = Interlocked.Increment(ref _count);
                if (newCount <= _size)
                {
                    shouldExpand = true;
                }
                else
                {
                    // Another thread took the last spot - use the store instead
                    Interlocked.Decrement(ref _count);
                }
            }
            if (shouldExpand)
            {
                return _factory(this);
            }
            else
            {
                lock (_itemStore)
                {
                    return _itemStore.Fetch();
                }
            }
        }

        private void PreloadItems()
        {
            for (int i = 0; i < _size; i++)
            {
                T item = _factory(this);
                _itemStore.Store(item);
            }
            _count = _size;
        }

        private IItemStore CreateItemStore(AccessMode mode, int capacity)
        {
            switch (mode)
            {
                case AccessMode.FIFO:
                    return new QueueStore(capacity);
                case AccessMode.LIFO:
                    return new StackStore(capacity);
                default:
                    Debug.Assert(mode == AccessMode.Circular,
                        "Invalid AccessMode in CreateItemStore");
                    return new CircularStore(capacity);
            }
        }

        public T Acquire()
        {
            _sync.WaitOne();
            switch (_loadingMode)
            {
                case LoadingMode.Eager:
                    return AcquireEager();
                case LoadingMode.Lazy:
                    return AcquireLazy();
                default:
                    Debug.Assert(_loadingMode == LoadingMode.LazyExpanding,
                        "Unknown LoadingMode encountered in Acquire method.");
                    return AcquireLazyExpanding();
            }
        }

        public void Release(T item)
        {
            lock (_itemStore)
            {
                _itemStore.Store(item);
            }
            _sync.Release();
        }

        public void Dispose()
        {
            if (_isDisposed) { return; }

            _isDisposed = true;


            if (typeof(IDisposable).IsAssignableFrom(typeof(T)))
            {
                lock (_itemStore)
                {
                    while (_itemStore.Count > 0)
                    {
                        IDisposable disposable = 
                            (IDisposable)_itemStore.Fetch();
                        disposable.Dispose();
                    }
                }
            }
            _sync.Close();
        }

        public bool IsDisposed
        {
            get { return _isDisposed; }
        }
    }
}
