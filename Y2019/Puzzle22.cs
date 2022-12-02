using AdventOfCode.Common;
using AdventOfCode.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;
namespace AdventOfCode.Y2019 {
    [Description("Slam Shuffle")]
    public class Puzzle22 : ASolver {
        private List<Operation> operations;

        public override void Setup() {
            List<string> items = Input.Lines();
            operations = new List<Operation>();
            for (int i = 0; i < items.Count; i++) {
                operations.Add(new Operation(items[i]));
            }
        }

        [Description("What is the position of card 2019?")]
        public override string SolvePart1() {
            LinkedList<int> cards = new LinkedList<int>();
            for (int i = 0; i < 10007; i++) {
                cards.AddLast(i);
            }

            for (int i = 0; i < operations.Count; i++) {
                operations[i].Apply(cards);
            }

            LinkedListNode<int> node = cards.First;
            for (int i = 0; i < 10007; i++) {
                if (node.Value == 2019) {
                    return $"{i}";
                }
                node = node.Next;
            }

            return string.Empty;
        }

        [Description("What number is on the card that ends up in position 2020?")]
        public override string SolvePart2() {
            BigInteger deckSize = 119315717514047;
            BigInteger repeats = 101741582076661;
            BigInteger offset = 0;
            BigInteger inc = 1;

            for (int i = 0; i < operations.Count; i++) {
                Operation op = operations[i];
                switch (op.Type) {
                    case OpType.Cut:
                        offset = (offset + op.Amount * inc + deckSize) % deckSize;
                        break;
                    case OpType.Deal:
                        inc = (-inc + deckSize) % deckSize;
                        offset = (offset + inc + deckSize) % deckSize;
                        break;
                    case OpType.Increment:
                        inc = (inc * BigInteger.ModPow(op.Amount, deckSize - 2, deckSize) + deckSize) % deckSize;
                        break;
                }
            }

            BigInteger increment = BigInteger.ModPow(inc, repeats, deckSize);
            offset = (offset * (1 - increment) * BigInteger.ModPow(1 - inc, deckSize - 2, deckSize)) % deckSize;
            return $"{(offset + 2020 * increment) % deckSize}";
        }

        private class Operation {
            public OpType Type;
            public int Amount;

            public Operation(string item) {
                int index;
                if ((index = item.IndexOf("increment", StringComparison.OrdinalIgnoreCase)) > 0) {
                    Type = OpType.Increment;
                    Amount = Tools.ParseInt(item, index + 10);
                } else if ((index = item.IndexOf("stack", StringComparison.OrdinalIgnoreCase)) > 0) {
                    Type = OpType.Deal;
                    Amount = 0;
                } else {
                    Type = OpType.Cut;
                    Amount = Tools.ParseInt(item, 4);
                }
            }

            public void Apply(LinkedList<int> cards) {
                switch (Type) {
                    case OpType.Cut:
                        int amount = Amount;
                        while (amount > 0) {
                            LinkedListNode<int> first = cards.First;
                            cards.RemoveFirst();
                            cards.AddLast(first);
                            amount--;
                        }

                        while (amount < 0) {
                            LinkedListNode<int> last = cards.Last;
                            cards.RemoveLast();
                            cards.AddFirst(last);
                            amount++;
                        }
                        break;
                    case OpType.Deal:
                        LinkedList<int> copy = new LinkedList<int>(cards);
                        cards.Clear();
                        int count = copy.Count;
                        for (int i = 0; i < count; i++) {
                            LinkedListNode<int> last = copy.Last;
                            copy.RemoveLast();
                            cards.AddLast(last);
                        }
                        break;
                    case OpType.Increment:
                        LinkedList<int> inc = new LinkedList<int>(cards);
                        int total = cards.Count;
                        LinkedListNode<int> current = cards.First;
                        LinkedListNode<int> node = inc.First;
                        for (int i = 0; i < total; i++) {
                            current.Value = node.Value;

                            node = node.Next;
                            for (int j = 0; j < Amount; j++) {
                                current = current.Next;
                                if (current == null) {
                                    current = cards.First;
                                }
                            }
                        }
                        break;
                }
            }

            public override string ToString() {
                return $"{Type} {Amount}";
            }
        }
        private enum OpType {
            Cut,
            Deal,
            Increment
        }
    }
}