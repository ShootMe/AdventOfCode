using AdventOfCode.Core;
using System.Collections.Generic;
using System.ComponentModel;
namespace AdventOfCode.Y2017 {
    [Description("Packet Scanners")]
    public class Puzzle13 : ASolver {
        private List<Scanner> scanners;

        public override void Setup() {
            List<string> items = Tools.GetLines(Input);
            scanners = new List<Scanner>(items.Count + 1);
            HashSet<int> seen = new HashSet<int>();
            for (int i = 0; i < items.Count; i++) {
                string item = items[i];
                int index = item.IndexOf(": ");
                int depth = Tools.ParseInt(item, 0, index);
                int range = Tools.ParseInt(item, index + 2);
                seen.Add(depth);
                scanners.Add(new Scanner() { Depth = depth, Range = range, Length = range == 1 ? 1 : (range - 1) * 2 });
            }
            int max = scanners[^1].Depth;
            for (int i = 0; i < max; i++) {
                if (!seen.Contains(i)) {
                    scanners.Add(new Scanner() { Depth = i, Range = 0 });
                }
            }
            scanners.Sort(delegate (Scanner left, Scanner right) {
                return left.Depth.CompareTo(right.Depth);
            });
        }

        [Description("What is the severity of your whole trip?")]
        public override string SolvePart1() {
            int time = 0;
            int current = 0;
            int score = 0;
            while (current < scanners.Count) {
                if (scanners[current].Range != 0 && time % scanners[current].Length == 0) {
                    score += scanners[current].Range * scanners[current].Depth;
                }

                current++;
                time++;
            }
            return $"{score}";
        }

        [Description("What is the fewest picoseconds that you need to delay to pass through the firewall safely?")]
        public override string SolvePart2() {
            int time = 1;
            while (true) {
                int i = 0;
                for (; i < scanners.Count; i++) {
                    if (scanners[i].Range == 0) { continue; }

                    if ((time + i) % scanners[i].Length == 0) {
                        time++;
                        break;
                    }
                }
                if (i == scanners.Count) {
                    return $"{time}";
                }
            }
        }

        private class Scanner {
            public int Depth;
            public int Range;
            public int Length;

            public override string ToString() {
                return $"{Depth} {Range}";
            }
        }
    }
}