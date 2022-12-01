using AdventOfCode.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
namespace AdventOfCode.Y2017 {
    [Description("Recursive Circus")]
    public class Puzzle07 : ASolver {
        private HashSet<ProgramInfo> programs;

        public override void Setup() {
            List<string> items = Tools.GetLines(Input);
            programs = new HashSet<ProgramInfo>();

            for (int i = 0; i < items.Count; i++) {
                string item = items[i];
                int index1 = item.IndexOf("->");
                string[] carrying;
                if (index1 > 0) {
                    carrying = item.Substring(index1 + 3).Split(", ");
                } else {
                    carrying = new string[0];
                }
                index1 = item.IndexOf('(');
                int index2 = item.IndexOf(')');

                ProgramInfo info = new ProgramInfo();
                info.Name = item.Substring(0, index1 - 1);
                info.Weight = Tools.ParseInt(item.Substring(index1 + 1, index2 - index1 - 1));

                ProgramInfo temp;
                if (!programs.TryGetValue(info, out temp)) {
                    programs.Add(info);
                } else {
                    temp.Weight = info.Weight;
                    info = temp;
                }

                for (int j = 0; j < carrying.Length; j++) {
                    ProgramInfo infoCarrying = new ProgramInfo() { Name = carrying[j] };
                    if (!programs.TryGetValue(infoCarrying, out temp)) {
                        infoCarrying.Parent = info;
                        programs.Add(infoCarrying);
                    } else {
                        temp.Parent = info;
                        infoCarrying = temp;
                    }

                    info.Carrying.Add(infoCarrying);
                }
            }
        }

        [Description("What is the name of the bottom program?")]
        public override string SolvePart1() {
            ProgramInfo info = GetBaseProgram();
            return info.Name;
        }

        [Description("What would its weight need to be to balance the entire tower?")]
        public override string SolvePart2() {
            ProgramInfo info = GetBaseProgram();
            return $"{info.FixUnbalance().Weight}";
        }

        private ProgramInfo GetBaseProgram() {
            using (var enumerator = programs.GetEnumerator()) {
                enumerator.MoveNext();
                ProgramInfo info = enumerator.Current;

                while (info.Parent != null) {
                    info = info.Parent;
                }

                return info;
            }
        }

        private class ProgramInfo : IEquatable<ProgramInfo> {
            public string Name;
            public int Weight;
            public ProgramInfo Parent;
            public List<ProgramInfo> Carrying = new List<ProgramInfo>();

            public ProgramInfo FixUnbalance(int otherWeight = 0) {
                if (Carrying.Count == 0) {
                    int total = GetTotalWeight();
                    Weight -= total - otherWeight;
                    return this;
                }

                int balance = -1;
                List<ProgramInfo> good = new List<ProgramInfo>();
                List<ProgramInfo> bad = new List<ProgramInfo>();
                for (int i = 0; i < Carrying.Count; i++) {
                    ProgramInfo info = Carrying[i];
                    int total = info.GetTotalWeight();
                    if (balance < 0 || total == balance) {
                        balance = total;
                        good.Add(info);
                    } else {
                        bad.Add(info);
                    }
                }

                if (good.Count == 0 || bad.Count == 0) {
                    int total = GetTotalWeight();
                    Weight -= total - otherWeight;
                    return this;
                }

                if (good.Count < bad.Count) {
                    return good[0].FixUnbalance(bad[0].GetTotalWeight());
                } else {
                    return bad[0].FixUnbalance(good[0].GetTotalWeight());
                }
            }
            public int GetTotalWeight() {
                int total = Weight;
                for (int i = 0; i < Carrying.Count; i++) {
                    total += Carrying[i].GetTotalWeight();
                }
                return total;
            }
            public override string ToString() {
                return $"{Parent?.Name} -> {Name} ({GetTotalWeight()}) -> ({Carrying.Count})";
            }
            public bool Equals(ProgramInfo other) {
                return Name.Equals(other.Name, StringComparison.OrdinalIgnoreCase);
            }
            public override bool Equals(object obj) {
                return obj is ProgramInfo info && info.Name.Equals(Name, StringComparison.OrdinalIgnoreCase);
            }
            public override int GetHashCode() {
                return Name.GetHashCode();
            }
        }
    }
}