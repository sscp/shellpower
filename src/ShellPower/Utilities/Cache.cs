using System;
using System.Collections.Generic;

namespace SSCP.ShellPower {
    /// <summary>
    /// Represents an abstract cache storing items of type T, by key where keys are of type K.
    /// See LruCache for a concrete implementation that implements least-recently-used eviction.
    /// </summary>
    public abstract class Cache<T, K> {
        long maxSize;
        /// <summary>
        /// Represents the max size of the cache. By default, this is simply the max size of the cache
        /// in number of elements, but you can change SizeFunc to calculate element size differently
        /// (ie, so that the size of each element is not equal to 1).
        /// </summary>
        public long MaxSize {
            get {
                return maxSize;
            }
            set {
                maxSize = value;
                while (CurrentSize > maxSize)
                    Evict();
            }
        }
        /// <summary>
        /// The current size of the cache. By default, this is simply the number of elems stored, but
        /// you can measure the size of each elem differently by setting SizeFunc.
        /// </summary>
        public long CurrentSize { get; protected set; }
        public int NumElements { get; protected set; }
        /// <summary>
        /// Used to compute the size of each element. By default, it always returns 1 (ie we're simply counting elements).
        /// </summary>
        public Func<T, long> SizeFunc { get; set; }
        /// <summary>
        /// Returns the key associated with a particular element.
        /// </summary>
        public Func<T, K> KeyFunc { get; set; }
        /// <summary>
        /// Compares two keys. By default, this is equivalent to the == operator.
        /// </summary>
        public IEqualityComparer<K> KeyComparer { get; set; }

        protected Cache() {
            SizeFunc = (t) => 1;
            KeyComparer = EqualityComparer<K>.Default;
        }
        public abstract bool ContainsKey(K key);
        public abstract T Retrieve(K key);
        public void Store(T elem) {
            NumElements++;
            CurrentSize += SizeFunc(elem);
            while (CurrentSize > MaxSize && !IsEmpty())
                Evict();
            StoreInner(elem);
        }
        protected abstract void StoreInner(T elem);
        public abstract void Evict();
        public abstract void Evict(K key);
        public abstract bool IsEmpty();
        public abstract void Clear();
        public T this[K key] {
            get {
                return Retrieve(key);
            }
        }
    }

    public class LruCache<T, K> : Cache<T, K> {
        /* we can't use the LinkedList class in this case, because we need to be able to
         * look up ListElems directly (complete with next/prev pointers) using a hashtable
         */
        class ListElem {
            public T val;
            public ListElem next, prev;
        }
        Dictionary<K, ListElem> dict = new Dictionary<K, ListElem>();
        ListElem head, tail;

        public LruCache(Func<T, K> keyFunc)
            : base() {
            KeyFunc = keyFunc;
        }

        public override bool ContainsKey(K key) {
            return dict.ContainsKey(key);
        }

        public override T Retrieve(K key) {
            ListElem elem;
            if (!dict.TryGetValue(key, out elem))
                throw new KeyNotFoundException();
            /* move to head of list, last in line for eviction */
            return elem.val;
        }

        public override void Evict() {
            if (IsEmpty())
                throw new InvalidOperationException("Cache is empty.");

            /* remove from dict */
            dict.Remove(KeyFunc(head.val));

            /* remove from list */
            if (head == tail)
                head = tail = null;
            else {
                head = head.next;
                head.prev = null;
            }
        }

        public override void Evict(K key) {
            /* remove from dict */
            if (!dict.ContainsKey(key))
                throw new KeyNotFoundException();
            var elem = dict[key];
            dict.Remove(key);

            /* remove from list */
            if (elem.next != null)
                elem.next.prev = elem.prev;
            if (elem.prev != null)
                elem.prev.next = elem.next;
            if (elem == head)
                head = head.next;
            if (elem == tail)
                tail = tail.prev;

            CurrentSize -= SizeFunc(elem.val);
        }

        public override bool IsEmpty() {
            return head == null;
        }

        public override void Clear() {
            CurrentSize = 0;
            dict.Clear();
            head = tail = null;
        }

        protected override void StoreInner(T elem) {
            K key = KeyFunc(elem);
            if (dict.ContainsKey(key))
                throw new InvalidOperationException("Key already present in cache.");

            /* append to list */
            ListElem listElem = new ListElem() {
                next = null,
                prev = tail,
                val = elem
            };
            if (tail != null)
                tail.next = listElem;
            tail = listElem;
            if (head == null)
                head = listElem;

            /* add to hashtable */
            dict.Add(key, listElem);
        }
    }
}
