using System.Collections.Generic;
using System.ComponentModel;
using AdventOfCode.Common;
using AdventOfCode.Core;
namespace AdventOfCode.Y2025 {
    [Description("Gift Shop")]
    public class Puzzle02 : ASolver {
        private List<(string, string)> ranges = new();

        public override void Setup() {
            string[] ids = Input.Split(',');
            foreach (string line in ids) {
                string[] splits = line.Split('-');
                ranges.Add((splits[0], splits[1]));
            }
        }

        [Description("What do you get if you add up all of the invalid IDs?")]
        public override string SolvePart1() {
            long total = 0;
            for (int i = 0; i < ranges.Count; i++) {
                (string start, string end) = ranges[i];
                long ending = end.ToLong();
                for (long j = start.ToLong(); j <= ending; j++) {
                    if (IsInvalid1($"{j}")) {
                        total += j;
                    }
                }
            }
            return $"{total}";
        }
        public bool IsInvalid1(string number) {
            return (number.Length & 1) == 0 && number.Substring(0, number.Length / 2) == number.Substring(number.Length / 2);
        }

        [Description("What do you get if you add up all of the invalid IDs using these new rules?")]
        public override string SolvePart2() {
            long total = 0;
            for (int i = 0; i < ranges.Count; i++) {
                (string start, string end) = ranges[i];
                long ending = end.ToLong();
                for (long j = start.ToLong(); j <= ending; j++) {
                    if (IsInvalid2($"{j}")) {
                        total += j;
                    }
                }
            }
            return $"{total}";
        }
        public bool IsInvalid2(string number) {
            for (int i = number.Length; i >= 2; i--) {
                if (number.Length % i != 0) { continue; }
                int length = number.Length / i;
                int end = number.Length - length;
                string val = number.Substring(0, length);
                bool match = true;
                for (int j = length; j <= end; j += length) {
                    if (number.Substring(j, length) != val) {
                        match = false;
                        break;
                    }
                }
                if (match) { return true; }
            }
            return false;
        }
    }
}