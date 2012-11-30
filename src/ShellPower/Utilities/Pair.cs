using System;
using System.Collections.Generic;
using System.Linq;

namespace SSCP.ShellPower {
    public struct Pair<T> {
        public Pair(T first, T second) : this()
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
            var hash = First.GetHashCode() * 65539 + Second.GetHashCode();
            return hash;
        }
        public T First { get; set; }
        public T Second { get; set; }
    }
}
