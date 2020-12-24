using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
namespace AdventOfCode.Common {
    public class WrappingList<T> : IEnumerable<T> {
        private static readonly WrappingNode empty;
        private WrappingNode[] storage;
        private T[] values;
        private int storageIndex;
        public int Count { get; private set; }
        private WrappingNode current;
        public T Current {
            get { return Count == 0 ? default : values[current.Index]; }
            set {
                if (Count == 0) { return; }
                values[current.Index] = value;
            }
        }
        public T Next {
            get { return Count == 0 ? default : values[current.Next]; }
        }
        public T Previous {
            get { return Count == 0 ? default : values[current.Previous]; }
        }

        static WrappingList() {
            empty = new WrappingNode(true);
        }
        public WrappingList() : this(128) { }
        public WrappingList(int initialCapacity) {
            storage = new WrappingNode[initialCapacity];
            Array.Fill(storage, empty);
            values = new T[initialCapacity];
            current = empty;
            storageIndex = 0;
            Count = 0;
        }

        public T[] ToArray() {
            T[] result = new T[Count];
            if (Count != 0) {
                WrappingNode start = current;
                int index = 0;
                do {
                    result[index++] = values[start.Index];
                    start = storage[start.Next];
                } while (start != current);
            }
            return result;
        }
        public T[] ToArray(int length) {
            T[] result = new T[length];
            if (Count != 0) {
                WrappingNode start = current;
                int index = 0;
                do {
                    result[index++] = values[start.Index];
                    start = storage[start.Next];
                } while (index < length);
            }
            return result;
        }
        public void Clear() {
            Count = 0;
            storageIndex = 0;
            current = empty;
        }
        public void ReverseElements(int length) {
            if (length <= 1 || length > Count) { return; }
            length--;

            WrappingNode start = current;
            WrappingNode end = current;
            for (int i = 0; i < length; i++) {
                end = storage[end.Next];
            }

            length++;
            length >>= 1;
            for (int j = 0; j < length; j++) {
                Swap(start.Index, end.Index);
                start = storage[start.Next];
                end = storage[end.Previous];
            }
        }
        private void Swap(int left, int right) {
            if (left == right) { return; }

            T value = values[left];
            values[left] = values[right];
            values[right] = value;
        }
        public void DecreasePosition() {
            current = storage[current.Previous];
        }
        public void DecreasePosition(int count) {
            if (count <= 0) { return; }

            WrappingNode ptr = current;
            while (count-- > 0) {
                ptr = storage[ptr.Previous];
            }
            current = ptr;
        }
        public void IncreasePosition() {
            current = storage[current.Next];
        }
        public void IncreasePosition(int count) {
            if (count <= 0) { return; }

            WrappingNode ptr = current;
            while (count-- > 0) {
                ptr = storage[ptr.Next];
            }
            current = ptr;
        }
        public T this[int index] {
            get {
                WrappingNode ptr = current;
                while (index-- > 0) {
                    ptr = storage[ptr.Next];
                }
                return values[ptr.Index];
            }
            set {
                WrappingNode ptr = current;
                while (index-- > 0) {
                    ptr = storage[ptr.Next];
                }
                values[ptr.Index] = value;
            }
        }
        public void AddBefore(T value, bool setCurrentElement = false) {
            if (++Count > storage.Length || storageIndex == storage.Length) {
                Resize();
            }

            ref WrappingNode node = ref storage[storageIndex];
            if (Count == 1) {
                node.Set(storage, storageIndex, storageIndex, storageIndex);
            } else {
                node.Set(storage, storageIndex, current.Index, current.Previous);
                current = storage[current.Index];
            }

            if (setCurrentElement || current == empty) {
                current = node;
            }
            values[storageIndex++] = value;
        }
        public void AddAfter(T value, bool setCurrentElement = false) {
            if (++Count > storage.Length || storageIndex == storage.Length) {
                Resize();
            }

            ref WrappingNode node = ref storage[storageIndex];
            if (Count == 1) {
                node.Set(storage, storageIndex, storageIndex, storageIndex);
            } else {
                node.Set(storage, storageIndex, current.Next, current.Index);
                current = storage[current.Index];
            }

            if (setCurrentElement || current == empty) {
                current = node;
            }
            values[storageIndex++] = value;
        }
        public T Remove() {
            if (Count == 0) { return default; }

            storage[current.Next].Previous = current.Previous;
            storage[current.Previous].Next = current.Next;

            int valueIndex = current.Index;
            storage[current.Index] = empty;
            current = --Count != 0 ? storage[current.Next] : empty;
            return values[valueIndex];
        }
        public IEnumerable<WrappingList<T>> Permute() {
            return DoPermute(0, Count - 1);
        }
        private IEnumerable<WrappingList<T>> DoPermute(int start, int end) {
            if (start == end) {
                yield return this;
            } else {
                for (var i = start; i <= end; i++) {
                    Swap(start, i);
                    foreach (WrappingList<T> seq in DoPermute(start + 1, end)) {
                        yield return seq;
                    }
                    Swap(start, i);
                }
            }
        }
        public override string ToString() {
            if (Count == 0) { return "[]"; }

            StringBuilder list = new StringBuilder($"[{values[current.Index]}");
            WrappingNode ptr = storage[current.Next];
            while (current != ptr) {
                list.Append($", {values[ptr.Index]}");
                ptr = storage[ptr.Next];
            }
            list.Append(']');

            return list.ToString();
        }
        private void Resize() {
            if (Count < storage.Length) {
                Cleanup();
                return;
            }

            WrappingNode[] newStorage = new WrappingNode[(int)(storage.Length * 1.5)];
            T[] newValues = new T[newStorage.Length];
            storageIndex = 0;
            WrappingNode node = current;
            WrappingNode first = new WrappingNode() { Index = storageIndex };

            do {
                ref WrappingNode newNode = ref newStorage[storageIndex];
                newValues[storageIndex] = values[node.Index];

                newNode.Index = storageIndex;
                newNode.Previous = storageIndex - 1;
                newNode.Next = ++storageIndex;

                node = storage[node.Next];
            } while (node != current);

            newStorage[0].Previous = storageIndex - 1;
            newStorage[storageIndex - 1].Next = 0;
            current = newStorage[0];
            storage = newStorage;
            values = newValues;
        }
        private void Cleanup() {
            int emptyIndex = -1;
            for (int i = 0; i < storage.Length; i++) {
                if (storage[i] == empty) {
                    emptyIndex = i;
                    break;
                }
            }

            int currentIndex = current.Index;
            int nextEmptyIndex = -1;
            for (int i = emptyIndex + 1; i < storage.Length; i++) {
                if (storage[i] == empty) {
                    if (nextEmptyIndex < 0) {
                        nextEmptyIndex = i;
                    }
                } else {
                    values[emptyIndex] = values[i];
                    ref WrappingNode node = ref storage[emptyIndex];
                    node = storage[i];
                    node.Index = emptyIndex;
                    storage[node.Previous].Next = emptyIndex;
                    storage[node.Next].Previous = emptyIndex;
                    storage[i] = empty;
                    if (i == currentIndex) {
                        currentIndex = emptyIndex;
                    }
                    emptyIndex = nextEmptyIndex > 0 ? nextEmptyIndex : i;
                    nextEmptyIndex = -1;
                    i = emptyIndex;
                }
            }
            storageIndex = emptyIndex;
            current = storage[currentIndex];
        }
        public IEnumerator<T> GetEnumerator() {
            WrappingNode ptr = current;
            do {
                yield return values[ptr.Index];
                ptr = storage[ptr.Next];
            } while (ptr != current);
        }
        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }

        private struct WrappingNode {
            internal int Index;
            internal int Next;
            internal int Previous;

            internal WrappingNode(bool empty) {
                Index = -1;
                Next = -1;
                Previous = -1;
            }
            internal void Set(WrappingNode[] data, int index, int next, int previous) {
                Index = index;
                Next = next;
                Previous = previous;
                data[Next].Previous = Index;
                data[Previous].Next = Index;
            }
            public static bool operator ==(WrappingNode left, WrappingNode right) {
                return left.Index == right.Index;
            }
            public static bool operator !=(WrappingNode left, WrappingNode right) {
                return left.Index != right.Index;
            }
            public override bool Equals(object obj) {
                return obj is WrappingNode node && node.Index == Index;
            }
            public override int GetHashCode() {
                return Index;
            }
            public override string ToString() {
                return $"({Previous}, {Index}, {Next})";
            }
        }
    }
}