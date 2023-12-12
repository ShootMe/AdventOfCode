using AdventOfCode.Common;
using AdventOfCode.Core;
using System.Collections.Generic;
using System.ComponentModel;
namespace AdventOfCode.Y2023 {
    [Description("Hot Springs")]
    public class Puzzle12 : ASolver {
        private List<Info> springInfos1 = new();
        private List<Info> springInfos2 = new();

        public override void Setup() {
            string[] lines = Input.Split('\n');
            foreach (string line in lines) {
                Info info = new Info(line);
                springInfos1.Add(info);
                springInfos2.Add(new Info($"{info.Springs}?{info.Springs}?{info.Springs}?{info.Springs}?{info.Springs} {info.GroupCounts},{info.GroupCounts},{info.GroupCounts},{info.GroupCounts},{info.GroupCounts}"));
            }
        }

        [Description("What is the sum of those counts?")]
        public override string SolvePart1() {
            long total = 0;
            for (int i = 0; i < springInfos1.Count; i++) {
                total += springInfos1[i].Arrangements();
            }
            return $"{total}";
        }

        [Description("What is the new sum of possible arrangement counts?")]
        public override string SolvePart2() {
            long total = 0;
            for (int i = 0; i < springInfos2.Count; i++) {
                total += springInfos2[i].Arrangements();
            }
            return $"{total}";
        }

        private class Info {
            public string Springs, GroupCounts;
            public int TotalCount;
            public int[] Groups;
            public Info(string data) {
                int index = data.IndexOf(' ');
                Springs = data.Substring(0, index);
                GroupCounts = data.Substring(index + 1);
                Groups = GroupCounts.ToInts(',');
                TotalCount = -1;
                for (int i = 0; i < Groups.Length; i++) {
                    TotalCount += Groups[i] + 1;
                }
            }
            public long Arrangements() {
                return Arrangements(0, 0, TotalCount, new Dictionary<(int, int), long>());
            }
            private long Arrangements(int index, int groupIndex, int remainingCount, Dictionary<(int, int), long> memory) {
                if (memory.TryGetValue((index, groupIndex), out long value)) { return value; }

                int groupCount = Groups[groupIndex];
                int extraCount = groupCount == remainingCount ? 0 : 1;

                long count = 0;
                int start = index;
                while (index + remainingCount <= Springs.Length) {
                    if (IsValid(index, groupCount, extraCount)) {
                        if (extraCount > 0) {
                            count += Arrangements(index + groupCount + 1, groupIndex + 1, remainingCount - groupCount - 1, memory);
                        } else {
                            count++;
                        }
                    }

                    if (Springs[index] == '#') { break; }
                    index++;
                }

                memory.Add((start, groupIndex), count);
                return count;
            }
            private bool IsValid(int index, int size, int pad) {
                if (index + size + pad > Springs.Length) { return false; }

                int end = index + size;
                for (int i = index; i < end; i++) {
                    char current = Springs[i];
                    if (current == '.') { return false; }
                }

                int maxCheck = pad == 0 ? Springs.Length - 1 : end;
                for (int i = end; i <= maxCheck; i++) {
                    if (Springs[i] == '#') { return false; }
                }
                return true;
            }
        }
    }
}