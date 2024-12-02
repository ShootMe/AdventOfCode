using AdventOfCode.Common;
using AdventOfCode.Core;
using System;
using System.ComponentModel;
namespace AdventOfCode.Y2024 {
    [Description("Historian Hysteria")]
    public class Puzzle01 : ASolver {
        private int[] left, right, counts;

        public override void Setup() {
            string[] lines = Input.Split('\n');
            left = new int[lines.Length];
            right = new int[lines.Length];
            counts = new int[100000];
            for (int i = 0; i < lines.Length; i++) {
                string line = lines[i];
                int index = line.IndexOf(' ');
                left[i] = line[0..index].ToInt();
                int rightNum = line[index..].ToInt();
                right[i] = rightNum;
                counts[rightNum]++;
            }
            Array.Sort(left);
            Array.Sort(right);
        }

        [Description("What is the total distance between your lists?")]
        public override string SolvePart1() {
            int total = 0;
            for (int i = 0; i < left.Length; i++) {
                total += Math.Abs(left[i] - right[i]);
            }
            return $"{total}";
        }

        [Description("What is their similarity score?")]
        public override string SolvePart2() {
            int total = 0;
            for (int i = 0; i < left.Length; i++) {
                int count = counts[left[i]];
                total += left[i] * count;
            }
            return $"{total}";
        }
    }
}