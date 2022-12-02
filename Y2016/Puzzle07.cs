using AdventOfCode.Common;
using AdventOfCode.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
namespace AdventOfCode.Y2016 {
    [Description("Internet Protocol Version 7")]
    public class Puzzle07 : ASolver {
        private IPv7[] ips;

        public override void Setup() {
            List<string> items = Input.Lines();
            ips = new IPv7[items.Count];
            for (int i = 0; i < ips.Length; i++) {
                ips[i] = items[i];
            }
        }

        [Description("How many IPs in your puzzle input support TLS?")]
        public override string SolvePart1() {
            int count = 0;
            for (int i = 0; i < ips.Length; i++) {
                if (ips[i].SupportsTLS()) {
                    count++;
                }
            }
            return $"{count}";
        }

        [Description("How many IPs in your puzzle input support SSL?")]
        public override string SolvePart2() {
            int count = 0;
            for (int i = 0; i < ips.Length; i++) {
                if (ips[i].SupportsSSL()) {
                    count++;
                }
            }
            return $"{count}";
        }

        private class IPv7 {
            public List<string> Supernet = new List<string>();
            public List<string> Hypernet = new List<string>();

            public bool SupportsTLS() {
                bool validTLS = false;

                for (int i = 0; i < Supernet.Count; i++) {
                    validTLS = validTLS || PartTLS(Supernet[i]);
                }
                for (int i = 0; i < Hypernet.Count; i++) {
                    validTLS = validTLS && !PartTLS(Hypernet[i]);
                }

                return validTLS;
            }
            private bool PartTLS(string part) {
                if (part.Length < 4) { return false; }

                char last1 = part[0];
                char last2 = part[1];
                for (int i = 2; i + 1 < part.Length; i++) {
                    if (last1 != last2 && part[i] == last2 && part[i + 1] == last1) {
                        return true;
                    }
                    last1 = last2;
                    last2 = part[i];
                }

                return false;
            }
            public bool SupportsSSL() {
                HashSet<Tuple<char, char>> sequences = new HashSet<Tuple<char, char>>(CharComparer.Comparer);
                for (int i = 0; i < Supernet.Count; i++) {
                    PartSSL(sequences, Supernet[i]);
                }
                for (int i = 0; i < Hypernet.Count; i++) {
                    if (PartSSL(sequences, Hypernet[i], true)) {
                        return true;
                    }
                }

                return false;
            }
            private bool PartSSL(HashSet<Tuple<char, char>> sequences, string part, bool checkSequence = false) {
                if (part.Length < 3) { return false; }

                char last1 = part[0];
                char last2 = part[1];
                for (int i = 2; i < part.Length; i++) {
                    if (last1 != last2 && part[i] == last1) {
                        if (checkSequence) {
                            if (sequences.Contains(new Tuple<char, char>(last2, last1))) {
                                return true;
                            }
                        } else {
                            sequences.Add(new Tuple<char, char>(last1, last2));
                        }
                    }
                    last1 = last2;
                    last2 = part[i];
                }

                return false;
            }

            public static implicit operator IPv7(string value) {
                int index1;
                int index2 = -1;
                int lastIndex = 0;
                IPv7 ip = new IPv7();
                do {
                    index1 = value.IndexOf('[', index2 + 1);
                    index2 = value.IndexOf(']', index1 + 1);
                    if (index1 <= lastIndex) {
                        index1 = value.Length;
                    }
                    ip.Supernet.Add(value.Substring(lastIndex, index1 - lastIndex));

                    if (index2 > index1) {
                        ip.Hypernet.Add(value.Substring(index1 + 1, index2 - index1 - 1));
                    }
                    lastIndex = index2 + 1;
                } while (index1 < value.Length);

                return ip;
            }
            public override string ToString() {
                StringBuilder ip = new StringBuilder();
                for (int i = 0; i < Supernet.Count; i++) {
                    ip.Append(Supernet[i]).Append(" , ");
                }
                ip.Length -= 2;
                ip.Append("[ ");
                for (int i = 0; i < Hypernet.Count; i++) {
                    ip.Append(Hypernet[i]).Append(" , ");
                }
                ip.Length -= 2;
                ip.Append("]");
                return ip.ToString();
            }

            private class CharComparer : IEqualityComparer<Tuple<char, char>> {
                public static CharComparer Comparer { get; } = new CharComparer();
                public bool Equals(Tuple<char, char> x, Tuple<char, char> y) {
                    return x.Item1 == y.Item1 && x.Item2 == y.Item2;
                }
                public int GetHashCode(Tuple<char, char> obj) {
                    return obj.Item1 | (obj.Item2 << 8);
                }
            }
        }
    }
}