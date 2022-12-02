using AdventOfCode.Common;
using AdventOfCode.Core;
using System.Collections.Generic;
using System.ComponentModel;
namespace AdventOfCode.Y2020 {
    [Description("Password Philosophy")]
    public class Puzzle02 : ASolver {
        private PasswordEntry[] entries;

        public override void Setup() {
            List<string> items = Input.Lines();
            entries = new PasswordEntry[items.Count];
            for (int i = 0; i < entries.Length; i++) {
                entries[i] = items[i];
            }
        }

        [Description("How many passwords are valid according to their policies?")]
        public override string SolvePart1() {
            int count = 0;
            for (int i = 0; i < entries.Length; i++) {
                if (entries[i].IsValid1()) {
                    count++;
                }
            }

            return $"{count}";
        }

        [Description("How many passwords are valid according to the new policies?")]
        public override string SolvePart2() {
            int count = 0;
            for (int i = 0; i < entries.Length; i++) {
                if (entries[i].IsValid2()) {
                    count++;
                }
            }

            return $"{count}";
        }

        private class PasswordEntry {
            public int Low;
            public int High;
            public char Char;
            public string Entry;

            public static implicit operator PasswordEntry(string line) {
                int index1 = line.IndexOf('-');
                int index2 = line.IndexOf(' ');
                int index3 = line.IndexOf(':');
                PasswordEntry entry = new PasswordEntry();
                entry.Low = Tools.ParseInt(line, 0, index1);
                entry.High = Tools.ParseInt(line, index1 + 1, index2 - index1 - 1);
                entry.Char = line[index2 + 1];
                entry.Entry = line.Substring(index3 + 2);
                return entry;
            }
            public bool IsValid1() {
                int count = 0;
                for (int i = 0; i < Entry.Length; i++) {
                    if (Entry[i] == Char) {
                        count++;
                    }
                }
                return count >= Low && count <= High;
            }
            public bool IsValid2() {
                return (Entry.Length >= Low && Entry[Low - 1] == Char) ^ (Entry.Length >= High && Entry[High - 1] == Char);
            }
        }
    }
}