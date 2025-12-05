using System.Collections.Generic;
using System.ComponentModel;
using AdventOfCode.Common;
using AdventOfCode.Core;
namespace AdventOfCode.Y2025 {
    [Description("Cafeteria")]
    public class Puzzle05 : ASolver {
        private List<(long, long)> freshIDRanges = new();
        private List<long> availableIDs = new();

        public override void Setup() {
            string[] lines = Input.Split('\n');
            int i = 0;
            while (i < lines.Length) {
                string line = lines[i++];
                if (string.IsNullOrEmpty(line)) { break; }

                string[] splits = line.Split('-');
                long s = splits[0].ToLong();
                long e = splits[1].ToLong();
                bool added = false;
                for (int j = 0; j < freshIDRanges.Count; j++) {
                    (long s1, long e1) = freshIDRanges[j];
                    if (s <= e1 && e >= s1) {
                        s = s < s1 ? s : s1;
                        e = e > e1 ? e : e1;
                        freshIDRanges.RemoveAt(j--);
                    } else if (e < s1) {
                        freshIDRanges.Insert(j, (s, e));
                        added = true;
                        break;
                    }
                }
                if (!added) { freshIDRanges.Add((s, e)); }
            }
            while (i < lines.Length) {
                string line = lines[i++];
                availableIDs.Add(line.ToLong());
            }
        }

        [Description("How many of the available ingredient IDs are fresh?")]
        public override string SolvePart1() {
            int total = 0;
            for (int i = 0; i < availableIDs.Count; i++) {
                long id = availableIDs[i];
                for (int j = 0; j < freshIDRanges.Count; j++) {
                    (long s, long e) = freshIDRanges[j];
                    if (id >= s && id <= e) {
                        total++;
                        break;
                    }
                }
            }
            return $"{total}";
        }

        [Description("How many ingredient IDs are considered to be fresh according to the fresh ingredient ID ranges?")]
        public override string SolvePart2() {
            long total = 0;
            for (int j = 0; j < freshIDRanges.Count; j++) {
                (long s, long e) = freshIDRanges[j];
                total += e - s + 1;
            }
            return $"{total}";
        }
    }
}