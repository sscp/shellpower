using System.Collections.Generic;
using System.Linq;
using System;
using OpenTK;

namespace SSCP.ShellPower {

    /// <summary>
    /// A 3-dimensional KD tree. This allows fast querying by 
    /// location and proximity. All elements stored in this collection
    /// must have a 3D bounding box. 
    /// </summary>
    public class KDTree<T> : ICollection<T> where T : IBoundingBox {
        private class KDBucket : IEnumerable<T> {
            const int MAX_BUCKET_SIZE = 50;

            public KDBucket first, second;
            public List<T> elements;
            public int depth;
            public double boundary;

            public void Add(T elem) {
                if (elements != null && elements.Count >= MAX_BUCKET_SIZE)
                    Split();
                if (elements == null) {
                    float min, max;
                    MinMax(elem, out min, out max);
                    if (min < boundary) first.Add(elem);
                    if (max >= boundary) second.Add(elem);
                } else {
                    elements.Add(elem);
                }
            }
            public bool Remove(T elem) {
                if (elements == null) {
                    float min, max;
                    MinMax(elem, out min, out max);
                    bool found = false;
                    if (min < boundary) found = found || first.Remove(elem);
                    if (max >= boundary) found = found || second.Remove(elem);
                    if (first.elements != null && first.elements.Count == 0) {
                        elements = second.elements;
                        first = second = null;
                        return true;
                    }
                    if (second.elements != null && second.elements.Count == 0) {
                        elements = first.elements;
                        first = second = null;
                        return true;
                    }
                    return found;
                } else {
                    return elements.Remove(elem);
                }
            }
            public bool Contains(T elem) {
                if (elements == null) {
                    float min, max;
                    MinMax(elem.BoundingBox, out min, out max);
                    if (min <= boundary) return first.Contains(elem);
                    if (max >= boundary) return second.Contains(elem);
                    return false;
                } else {
                    return elements.Contains(elem);
                }
            }
            public void Split() {
                /* find the boundary heuristically--just take the median of our contents so far */
                /* yes i am this lazy */
                var coords = elements.Select((t) => {
                    float min, max;
                    MinMax(t.BoundingBox, out min, out max);
                    return (min + max) / 2f;
                }).ToList();
                coords.Sort();
                boundary = coords[coords.Count / 2];

                first = new KDBucket();
                first.elements = new List<T>(MAX_BUCKET_SIZE);
                first.depth = depth + 1;
                second = new KDBucket();
                second.elements = new List<T>(MAX_BUCKET_SIZE);
                second.depth = depth + 1;
                foreach (var t in elements) {
                    float min, max;
                    MinMax(t.BoundingBox, out min, out max);
                    if (min < boundary) {
                        first.Add(t);
                    }
                    if (max >= boundary) {
                        second.Add(t);
                    }
                }

                elements = null;
            }
            public void MinMax(T elem, out float min, out float max) {
                MinMax(elem.BoundingBox, out min, out max);
            }
            public void MinMax(Quad3 box, out float min, out float max) {
                switch (depth % 3) {
                    case 0:
                        min = box.Min.X;
                        max = box.Max.X;
                        break;
                    case 1:
                        min = box.Min.Y;
                        max = box.Max.Y;
                        break;
                    case 2:
                        min = box.Min.Z;
                        max = box.Max.Z;
                        break;
                    default:
                        min = max = 0;
                        break;
                }
            }

            public IEnumerator<T> GetEnumerator() {
                return new KDEnumerator(this, Quad3.Infinite);
            }
            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
                return new KDEnumerator(this, Quad3.Infinite);
            }
        }

        private class KDEnumerator : IEnumerator<T>, System.Collections.IEnumerator {
            KDBucket bucket;
            KDEnumerator inner;
            Quad3 boundingBox;
            int ix = -1;

            public KDEnumerator(KDBucket bucket, Quad3 boundingBox) {
                this.bucket = bucket;
                this.boundingBox = boundingBox;
            }
            public T Current {
                get {
                    if (bucket.elements == null)
                        return inner.Current;
                    return bucket.elements[ix];
                }
            }
            public void Dispose() {
            }
            object System.Collections.IEnumerator.Current {
                get {
                    return Current;
                }
            }
            public bool MoveNext() {
                if (bucket.elements == null) {
                    if (ix == -1) {
                        float min, max;
                        bucket.MinMax(boundingBox, out min, out max);
                        if (min <= bucket.boundary) {
                            inner = new KDEnumerator(bucket.first, boundingBox);
                            ix = 0;
                        } else {
                            inner = new KDEnumerator(bucket.second, boundingBox);
                            ix = 1;
                        }
                    }
                    if (ix == 0) {
                        if (inner.MoveNext()) {
                            return true;
                        }
                        float min, max;
                        bucket.MinMax(boundingBox, out min, out max);
                        if (max < bucket.boundary)
                            return false;
                        ix = 1;
                        inner = new KDEnumerator(bucket.second, boundingBox);
                    }

                    //at this point, ix == 1
                    return inner.MoveNext();
                } else {
                    do {
                        ix++;
                    } while (ix < bucket.elements.Count && !Current.BoundingBox.Overlaps(boundingBox));
                    return (ix < bucket.elements.Count);
                }
            }
            public void Reset() {
                ix = -1;
            }
        }

        private class KDEnumerable : IEnumerable<T> {
            KDBucket bucket;
            Quad3 boundingBox;
            public KDEnumerable(KDBucket bucket, Quad3 boundingBox) {
                this.boundingBox = boundingBox;
                this.bucket = bucket;
            }
            public IEnumerator<T> GetEnumerator() {
                return new KDEnumerator(bucket, boundingBox);
            }
            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
                return new KDEnumerator(bucket, boundingBox);
            }
        }

        private KDBucket root;

        public KDTree() {
            Clear();
        }

        public IEnumerable<T> GetElementsInVolume(Quad3 boundingBox) {
            return new KDEnumerable(root, boundingBox);
        }

        public void Add(T item) {
            root.Add(item);
            Count++;
        }

        public void Clear() {
            root = new KDBucket();
            root.elements = new List<T>();
        }

        public bool Contains(T item) {
            return root.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex) {
            T[] ret = new T[Count];

        }

        public int Count { get; private set; }

        public bool IsReadOnly {
            get { return false; }
        }

        public bool Remove(T item) {
            bool found = root.Remove(item);
            if (found)
                Count--;
            return found;
        }

        public IEnumerator<T> GetEnumerator() {
            return root.GetEnumerator();
        }
        
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
            return this.GetEnumerator();
        }

        private void Shuffle(int[] p) {
            var rand = new Random();
            for (int i = 0; i < p.Length; i++) {
                int ix = i + rand.Next(p.Length - i);
                int tmp = p[i];
                p[i] = p[ix];
                p[ix] = tmp;
            }
        }

        /// <summary>
        /// Adds a list of objects to the tree. Adds them in random order
        /// to try to keep the tree balanced.
        /// </summary>
        public void AddAll(List<T> boxes) {
            int n = boxes.Count;
            int[] perm = new int[n];
            for (int i = 0; i < n; i++) {
                perm[i] = i;
            }
            Shuffle(perm);
            for (int i = 0; i < n; i++) {
                int ix = perm[i];
                Add(boxes[ix]);
            }
        }
    }
}
