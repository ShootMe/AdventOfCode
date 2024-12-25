using AdventOfCode.Core;
using System.Collections.Generic;
using System.ComponentModel;
namespace AdventOfCode.Y2024 {
    [Description("Code Chronicle")]
    public class Puzzle25 : ASolver {
        [Description("How many unique lock/key pairs fit together without overlapping in any column?")]
        public override string SolvePart1() {
            string[] data = Input.Split('\n');
            int width = data[0].Length;

            List<int[]> keyLocks = new();
            int total = 0;
            int[] keyLock = new int[width + 1];
            int i = 0;

            while (i < data.Length) {
                string line = data[i++];

                bool isKey = line[0] == '.';
                keyLock[width] = isKey ? 1 : 0;
                int count = 0;

                while (i < data.Length && (line = data[i++]).Length > 0) {
                    count++;
                    for (int j = 0; j < line.Length; j++) {
                        if (keyLock[j] == 0 && line[j] == (isKey ? '#' : '.')) {
                            keyLock[j] = isKey ? count : -count;
                        }
                    }
                }

                keyLocks.Add(keyLock);
                keyLock = new int[width + 1];
            }

            for (i = 0; i < keyLocks.Count; i++) {
                int[] keyLock1 = keyLocks[i];

                for (int j = i + 1; j < keyLocks.Count; j++) {
                    int[] keyLock2 = keyLocks[j];
                    if (keyLock1[width] == keyLock2[width]) { continue; }

                    int k = 0;
                    for (; k < width; k++) {
                        if (keyLock1[k] + keyLock2[k] < 0) { break; }
                    }
                    if (k == width) { total++; }
                }
            }

            return $"{total}";
        }
    }
}