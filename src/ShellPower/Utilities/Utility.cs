using System;
using System.Collections.Generic;
using System.Linq;

namespace SSCP.ShellPower {
    public static class Extensions {
        public static T ArgMin<T, V>(this IEnumerable<T> list, Func<T, V> eval) {
            Comparer<V> comparer = Comparer<V>.Default;
            T argMin = list.First();
            V min = eval(list.First());
            foreach (var t in list.Skip(1)) {
                var v = eval(t);
                if (comparer.Compare(v, min) < 0) {
                    min = v;
                    argMin = t;
                }
            }

            return argMin;
        }
    }
    public class Pair<T> {
        public Pair(T first, T second)// : this()
        {
            this.First = first;
            this.Second = second;
        }
        public override bool Equals(object obj) {
            return
                obj is Pair<T>
                && ((Pair<T>)obj).First.Equals(First)
                && ((Pair<T>)obj).Second.Equals(Second);
        }
        public override int GetHashCode() {
            return First.GetHashCode() ^ Second.GetHashCode();
        }
        public T First { get; set; }
        public T Second { get; set; }
    }
}
