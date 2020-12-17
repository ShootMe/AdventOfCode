using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace AdventOfCode.Common {
    public class ArrayComparer<T> : IEqualityComparer<T[]> where T : IEquatable<T> {
        public static ArrayComparer<T> Comparer { get; } = new ArrayComparer<T>();
        public bool Equals(T[] x, T[] y) {
            ReadOnlySpan<T> left = new ReadOnlySpan<T>(x);
            ReadOnlySpan<T> right = new ReadOnlySpan<T>(y);
            return left.SequenceEqual(right);
        }
        public int GetHashCode(T[] obj) {
            int hash = 17;
            for (int i = obj.Length - 1; i >= 0; i--) {
                hash = (hash * 31) + obj[i].GetHashCode();
            }
            return hash;
        }
        public string ToString(T[] obj) {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < obj.Length; i++) {
                sb.Append(obj[i]).Append(", ");
            }
            if (obj.Length > 0) {
                sb.Length -= 2;
            }
            return sb.ToString();
        }
    }
}