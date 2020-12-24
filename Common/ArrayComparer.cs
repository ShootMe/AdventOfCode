using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace AdventOfCode.Common {
    public class ArrayComparer<T> : IEqualityComparer<T[]>, IEqualityComparer<List<T>> where T : IEquatable<T> {
        public static ArrayComparer<T> Comparer { get; } = new ArrayComparer<T>();
        public bool Equals(T[] left, T[] right) {
            ReadOnlySpan<T> leftSpan = new ReadOnlySpan<T>(left);
            ReadOnlySpan<T> rightSpan = new ReadOnlySpan<T>(right);
            return leftSpan.SequenceEqual(rightSpan);
        }
        public int GetHashCode(T[] obj) {
            int hash = 17;
            if (obj != null) {
                for (int i = obj.Length - 1; i >= 0; i--) {
                    hash = (hash * 31) + obj[i].GetHashCode();
                }
            }
            return hash;
        }
        public string ToString(T[] obj) {
            StringBuilder sb = new StringBuilder();
            if (obj != null) {
                for (int i = 0; i < obj.Length; i++) {
                    sb.Append(obj[i]).Append(", ");
                }
                if (obj.Length > 0) {
                    sb.Length -= 2;
                }
            }
            return sb.ToString();
        }

        public bool Equals(List<T> left, List<T> right) {
            if (left.Count != right.Count) { return false; }
            for (int i = left.Count - 1; i >= 0; i--) {
                if (!left[i].Equals(right[i])) { return false; }
            }
            return true;
        }
        public int GetHashCode(List<T> obj) {
            int hash = 17;
            if (obj != null) {
                for (int i = obj.Count - 1; i >= 0; i--) {
                    hash = (hash * 31) + obj[i].GetHashCode();
                }
            }
            return hash;
        }
        public string ToString(List<T> obj) {
            StringBuilder sb = new StringBuilder();
            if (obj != null) {
                for (int i = 0; i < obj.Count; i++) {
                    sb.Append(obj[i]).Append(", ");
                }
                if (obj.Count > 0) {
                    sb.Length -= 2;
                }
            }
            return sb.ToString();
        }
    }
}