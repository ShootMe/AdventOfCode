using AdventOfCode.Common;
using AdventOfCode.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
namespace AdventOfCode.Y2024 {
    [Description("Historian Hysteria")]
    public class Puzzle01 : ASolver {
        private List<int> left = new();
        private List<int> right = new();

        public override void Setup() {
            string[] lines = Input.Split('\n');
            foreach (string line in lines) {
                string[] nums = line.Split(" ", StringSplitOptions.RemoveEmptyEntries);
                left.Add(nums[0].ToInt());
                right.Add(nums[1].ToInt());
            }
            left.Sort();
            right.Sort();
        }

        [Description("Your actual left and right lists contain many location IDs. What is the total distance between your lists?")]
        public override string SolvePart1() {
            int total = 0;
            for (int i = 0; i < left.Count; i++) {
                total += Math.Abs(left[i] - right[i]);
            }
            return $"{total}";
        }

        [Description("Once again consider your left and right lists. What is their similarity score?")]
        public override string SolvePart2() {
            int total = 0;
            Dictionary<int, int> rightCount = new();
            for (int i = 0; i < left.Count; i++) {
                int count = 0;
                if (rightCount.TryGetValue(right[i], out count)) {
                    rightCount[right[i]]++;
                } else {
                    rightCount[right[i]] = 1;
                }
            }
            for (int i = 0; i < left.Count; i++) {
                int count = 0;
                if (rightCount.TryGetValue(left[i], out count)) {
                    total += left[i] * count;
                }
            }
            return $"{total}";
        }
    }
}