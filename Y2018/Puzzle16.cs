using AdventOfCode.Core;
using System.Collections.Generic;
using System.ComponentModel;
namespace AdventOfCode.Y2018 {
    public class Puzzle16 : ASolver {
        private List<string> items;
        private List<OpCode> opCodes;
        public Puzzle16(string input) : base(input) { Name = "Chronal Classification"; }

        public override void Setup() {
            items = Tools.GetSections(Input);
            opCodes = new List<OpCode>();
            for (int i = 0; i < 16; i++) {
                opCodes.Add(new OpCode() { ID = i });
            }
        }

        [Description("How many samples in your puzzle input behave like three or more opcodes?")]
        public override string SolvePart1() {
            int total = 0;
            for (int i = 0; i < items.Count; i++) {
                string item = items[i];
                if (string.IsNullOrEmpty(item)) { break; }

                string[] rows = item.Split('\n');
                item = rows[0];

                int index1 = item.IndexOf(',');
                int a = Tools.ParseInt(item, 9, index1 - 9);
                int index2 = item.IndexOf(',', index1 + 1);
                int b = Tools.ParseInt(item, index1 + 2, index2 - index1 - 2);
                index1 = item.IndexOf(',', index2 + 1);
                int c = Tools.ParseInt(item, index2 + 2, index1 - index2 - 2);
                index2 = item.IndexOf(',', index1 + 1);
                int d = Tools.ParseInt(item, index1 + 2, item.Length - index1 - 3);

                item = rows[1];
                index1 = item.IndexOf(' ');
                int v0 = Tools.ParseInt(item, 0, index1);
                index2 = item.IndexOf(' ', index1 + 1);
                int v1 = Tools.ParseInt(item, index1 + 1, index2 - index1 - 1);
                index1 = item.IndexOf(' ', index2 + 1);
                int v2 = Tools.ParseInt(item, index2 + 1, index1 - index2 - 1);
                int v3 = Tools.ParseInt(item, index1 + 1);

                item = rows[2];
                index1 = item.IndexOf(',');
                int a2 = Tools.ParseInt(item, 9, index1 - 9);
                index2 = item.IndexOf(',', index1 + 1);
                int b2 = Tools.ParseInt(item, index1 + 2, index2 - index1 - 2);
                index1 = item.IndexOf(',', index2 + 1);
                int c2 = Tools.ParseInt(item, index2 + 2, index1 - index2 - 2);
                int d2 = Tools.ParseInt(item, index1 + 2, item.Length - index1 - 3);

                int r1 = v1 == 0 ? a : v1 == 1 ? b : v1 == 2 ? c : d;
                int r2 = v2 == 0 ? a : v2 == 1 ? b : v2 == 2 ? c : d;
                int r3 = v3 == 0 ? a2 : v3 == 1 ? b2 : v3 == 2 ? c2 : d2;

                int count = 0;
                for (int j = 0; j < opCodes.Count; j++) {
                    OpCode code = opCodes[j];
                    if (code.Execute(r1, r2, v1, v2) == r3) {
                        count++;
                        if (!code.InvalidFor.Contains(v0) && !code.ValidFor.Contains(v0)) {
                            code.ValidFor.Add(v0);
                        }
                    } else if (!code.InvalidFor.Contains(v0)) {
                        code.ValidFor.Remove(v0);
                        code.InvalidFor.Add(v0);
                    }
                }

                if (count >= 3) {
                    total++;
                }
            }
            return $"{total}";
        }

        [Description("What value is contained in register 0 after executing the test program?")]
        public override string SolvePart2() {
            for (int j = 0; j < opCodes.Count; j++) {
                for (int i = 0; i < opCodes.Count; i++) {
                    OpCode code = opCodes[i];
                    if (code.ActualID >= 0) { continue; }

                    if (code.ValidFor.Count == 1) {
                        int actualID = 0;
                        foreach (int id in code.ValidFor) {
                            actualID = id;
                            break;
                        }
                        code.ActualID = actualID;

                        for (int k = 0; k < opCodes.Count; k++) {
                            if (k == i) { continue; }

                            OpCode other = opCodes[k];
                            other.ValidFor.Remove(actualID);
                            other.InvalidFor.Add(actualID);
                        }
                    }
                }
            }

            opCodes.Sort((code1, code2) => code1.ActualID.CompareTo(code2.ActualID));

            int index = 0;
            while (index < items.Count) {
                string item = items[index++];
                if (string.IsNullOrEmpty(item)) { break; }
            }

            int a = 0;
            int b = 0;
            int c = 0;
            int d = 0;
            string[] rows = items[index].Split('\n');
            for (int i = 0; i < rows.Length; i++) {
                string item = rows[i];

                int index1 = item.IndexOf(' ');
                int v0 = Tools.ParseInt(item, 0, index1);
                int index2 = item.IndexOf(' ', index1 + 1);
                int v1 = Tools.ParseInt(item, index1 + 1, index2 - index1 - 1);
                index1 = item.IndexOf(' ', index2 + 1);
                int v2 = Tools.ParseInt(item, index2 + 1, index1 - index2 - 1);
                int v3 = Tools.ParseInt(item, index1 + 1);

                int r1 = v1 == 0 ? a : v1 == 1 ? b : v1 == 2 ? c : d;
                int r2 = v2 == 0 ? a : v2 == 1 ? b : v2 == 2 ? c : d;
                OpCode code = opCodes[v0];
                int result = code.Execute(r1, r2, v1, v2);
                if (v3 == 0) {
                    a = result;
                } else if (v3 == 1) {
                    b = result;
                } else if (v3 == 2) {
                    c = result;
                } else {
                    d = result;
                }
            }

            return $"{a}";
        }

        private class OpCode {
            public int ID;
            public int ActualID;
            public HashSet<int> ValidFor;
            public HashSet<int> InvalidFor;

            public OpCode() {
                ValidFor = new HashSet<int>();
                InvalidFor = new HashSet<int>();
                ActualID = -1;
            }

            public int Execute(int reg1, int reg2, int value1, int value2) {
                switch (ID) {
                    case 0: return reg1 + reg2;
                    case 1: return reg1 + value2;
                    case 2: return reg1 * reg2;
                    case 3: return reg1 * value2;
                    case 4: return reg1 & reg2;
                    case 5: return reg1 & value2;
                    case 6: return reg1 | reg2;
                    case 7: return reg1 | value2;
                    case 8: return reg1;
                    case 9: return value1;
                    case 10: return value1 > reg2 ? 1 : 0;
                    case 11: return reg1 > value2 ? 1 : 0;
                    case 12: return reg1 > reg2 ? 1 : 0;
                    case 13: return value1 == reg2 ? 1 : 0;
                    case 14: return reg1 == value2 ? 1 : 0;
                    case 15: return reg1 == reg2 ? 1 : 0;
                }
                return 0;
            }

            public override string ToString() {
                switch (ID) {
                    case 0: return $"{ID} {ActualID} addr";
                    case 1: return $"{ID} {ActualID} addi";
                    case 2: return $"{ID} {ActualID} mulr";
                    case 3: return $"{ID} {ActualID} muli";
                    case 4: return $"{ID} {ActualID} banr";
                    case 5: return $"{ID} {ActualID} bani";
                    case 6: return $"{ID} {ActualID} borr";
                    case 7: return $"{ID} {ActualID} bori";
                    case 8: return $"{ID} {ActualID} setr";
                    case 9: return $"{ID} {ActualID} seti";
                    case 10: return $"{ID} {ActualID} gtir";
                    case 11: return $"{ID} {ActualID} gtri";
                    case 12: return $"{ID} {ActualID} gtrr";
                    case 13: return $"{ID} {ActualID} eqir";
                    case 14: return $"{ID} {ActualID} eqri";
                    case 15: return $"{ID} {ActualID} eqrr";
                }
                return string.Empty;
            }
        }
    }
}