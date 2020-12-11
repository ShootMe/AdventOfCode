using System.Collections;
using System.Collections.Generic;
using System.Text;
namespace AdventOfCode.Common {
    public unsafe class WrappingList<T> : IEnumerable<T>, IEnumerator<T> {
        private static readonly WrappingNode* empty;
        private WrappingNode[] storage;
        private T[] values;
        private int storageIndex;
        public int Count { get; private set; }
        private WrappingNode* current, iterator, head;
        public T CurrentElement {
            get { return Count == 0 ? default : values[current - head]; }
            set {
                if (Count == 0) { return; }
                values[current - head] = value;
            }
        }
        T IEnumerator<T>.Current => Count == 0 || iterator == null ? default : values[iterator - head];
        object IEnumerator.Current => Count == 0 || iterator == null ? null : values[iterator - head];

        static WrappingList() {
            WrappingNode node = new WrappingNode(true);
            empty = &node;
        }
        public WrappingList() : this(128) { }
        public WrappingList(int initialCapacity) {
            storage = new WrappingNode[initialCapacity];
            fixed (WrappingNode* ptr = &storage[0]) {
                head = ptr;
            }
            values = new T[initialCapacity];
            current = empty;
            storageIndex = 0;
            Count = 0;
        }

        public T[] ToArray() {
            T[] result = new T[Count];
            if (Count != 0) {
                WrappingNode* start = current;
                int index = 0;
                do {
                    result[index++] = values[start - head];
                    start = start->Next;
                } while (start != current);
            }
            return result;
        }
        public T[] ToArray(int length) {
            T[] result = new T[length];
            if (Count != 0) {
                WrappingNode* start = current;
                int index = 0;
                do {
                    result[index++] = values[start - head];
                    start = start->Next;
                } while (index < length);
            }
            return result;
        }
        public void ReverseElements(int length) {
            if (length <= 1 || length > Count) { return; }
            length--;

            for (int i = 0; i < length; i++) {
                WrappingNode* start = current;
                current = start->Next;
                for (int j = i; j < length; j++) {
                    SwapAhead(start);
                }
            }
        }
        private void SwapAhead(WrappingNode* node) {
            WrappingNode* ptrNext = node->Next;
            if (ptrNext->Next == node) { return; }

            WrappingNode* ptrPrevious = node->Previous;
            node->Next = ptrNext->Next;
            node->Previous = ptrNext;

            ptrNext->Next = node;
            ptrNext->Previous = ptrPrevious;

            ptrNext->Previous->Next = ptrNext;
            node->Next->Previous = node;
        }
        public void MoveElementForward() {
            SwapAhead(current);
        }
        public void MoveElementForward(int count) {
            while (count-- > 0) {
                SwapAhead(current);
            }
        }
        public void MoveElementBack() {
            SwapAhead(current->Previous);
        }
        public void MoveElementBack(int count) {
            while (count-- > 0) {
                SwapAhead(current->Previous);
            }
        }
        public void DecreasePosition() {
            current = current->Previous;
        }
        public void DecreasePosition(int count) {
            if (count <= 0) { return; }

            WrappingNode* ptr = current;
            while (count-- > 0) {
                ptr = ptr->Previous;
            }
            current = ptr;
        }
        public void IncreasePosition() {
            current = current->Next;
        }
        public void IncreasePosition(int count) {
            if (count <= 0) { return; }

            WrappingNode* ptr = current;
            while (count-- > 0) {
                ptr = ptr->Next;
            }
            current = ptr;
        }
        public T ElementAt(int index) {
            if (index < 0) { return default; }

            WrappingNode* ptr = current;
            while (index-- > 0) {
                ptr = ptr->Next;
            }
            return values[ptr - head];
        }
        public void AddBefore(T value, bool setCurrentElement = false) {
            if (++Count > storage.Length || storageIndex == storage.Length) {
                Resize();
            }

            WrappingNode* node = head + storageIndex;
            if (Count == 1) {
                node->Set(node, node);
            } else {
                node->Set(current, current->Previous);
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

            WrappingNode* node = head + storageIndex;
            values[storageIndex] = value;
            if (Count == 1) {
                node->Set(node, node);
            } else {
                node->Set(current->Next, current);
            }

            if (setCurrentElement || current == empty) {
                current = node;
            }
            values[storageIndex++] = value;
        }
        public T Remove() {
            if (Count == 0) { return default; }

            current->Next->Previous = current->Previous;
            current->Previous->Next = current->Next;

            long valueIndex = current - head;
            current->Previous = null;
            current = --Count != 0 ? current->Next : empty;

            return values[valueIndex];
        }
        public override string ToString() {
            if (Count == 0) { return "[]"; }

            StringBuilder list = new StringBuilder($"[{values[current - head]}");
            WrappingNode* ptr = current->Next;
            while (current != ptr) {
                list.Append($", {values[ptr - head]}");
                ptr = ptr->Next;
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
            WrappingNode* node = current;
            WrappingNode* last = null;
            WrappingNode* first;
            fixed (WrappingNode* ptr = &newStorage[0]) {
                first = ptr;
            }

            do {
                WrappingNode* newNode = first + storageIndex;
                newNode->Previous = last;
                newValues[storageIndex++] = values[node - head];
                last = newNode;
                node = node->Next;
            } while (node != current);

            first->Previous = last;
            current = first;
            head = current;

            do {
                last->Next = first;
                first = last;
                last = last->Previous;
            } while (last != current);

            current->Next = first;

            storage = newStorage;
            values = newValues;
        }
        private void Cleanup() {
            storageIndex = 0;
            WrappingNode* ptr = current;
            WrappingNode* itr = head;
            do {
                int offset = (int)(ptr - head);
                if (offset != storageIndex) {
                    T value = values[storageIndex];
                    values[storageIndex] = values[offset];
                    values[offset] = value;
                }

                storageIndex++;
                ptr = ptr->Next;
                itr++;
            } while (ptr != current);

            current = head;
            ptr = current;
            itr = ptr + storageIndex - 1;
            for (int i = storageIndex - 1; i > 0; i--) {
                ptr->Next = ptr + 1;
                ptr->Previous = itr;
                itr = ptr++;
            }
            ptr->Next = current;
            ptr->Previous = itr;
        }
        public IEnumerator<T> GetEnumerator() {
            return this;
        }
        IEnumerator IEnumerable.GetEnumerator() {
            return this;
        }
        public bool MoveNext() {
            if (iterator == null) {
                iterator = current;
                return true;
            }
            iterator = iterator->Next;
            return iterator != current;
        }
        public void Reset() {
            iterator = null;
        }
        public void Dispose() {
            iterator = null;
        }

        internal struct WrappingNode {
            internal WrappingNode* Next;
            internal WrappingNode* Previous;

            internal WrappingNode(bool empty) {
                fixed (WrappingNode* ptr = &this) {
                    Next = ptr;
                    Previous = ptr;
                }
            }
            internal void Set(WrappingNode* next, WrappingNode* previous) {
                fixed (WrappingNode* ptr = &this) {
                    Next = next;
                    Previous = previous;
                    Next->Previous = ptr;
                    Previous->Next = ptr;
                }
            }
        }
    }
}