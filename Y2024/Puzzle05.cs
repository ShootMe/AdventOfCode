using AdventOfCode.Common;
using AdventOfCode.Core;
using System.Collections.Generic;
using System.ComponentModel;
namespace AdventOfCode.Y2024 {
    [Description("Print Queue")]
    public class Puzzle05 : ASolver {
        private Dictionary<int, HashSet<int>> rules = new();
        private List<List<int>> updates = new();

        public override void Setup() {
            string[] lines = Input.Split('\n');
            foreach (string line in lines) {
                if (string.IsNullOrEmpty(line)) { continue; }

                int index = line.IndexOf('|');
                if (index < 0) {
                    string[] nums = line.Split(',');
                    List<int> updatePages = new();
                    for (int i = 0; i < nums.Length; i++) {
                        updatePages.Add(nums[i].ToInt());
                    }
                    updates.Add(updatePages);
                } else {
                    int page1 = line[0..index].ToInt();
                    int page2 = line[index..].ToInt();
                    HashSet<int> before;
                    if (!rules.TryGetValue(page1, out before)) {
                        before = new();
                        rules[page1] = before;
                    }
                    before.Add(page2);
                }
            }
        }

        [Description("What do you get if you add up the middle page number from those correctly-ordered updates?")]
        public override string SolvePart1() {
            return $"{OrderUpdates(true)}";
        }

        [Description("What do you get if you add up the middle page numbers after correctly ordering just those updates?")]
        public override string SolvePart2() {
            return $"{OrderUpdates(false)}";
        }
        private int OrderUpdates(bool onlyGood) {
            int total = 0;
            for (int i = 0; i < updates.Count; i++) {
                List<int> updateOrig = updates[i];
                List<int> update = new(updateOrig);

                update.Sort(delegate (int left, int right) {
                    HashSet<int> beforeLeft, beforeRight;
                    if (rules.TryGetValue(left, out beforeLeft)) {
                        if (beforeLeft.Contains(right)) {
                            return -1;
                        }
                    } else if (!rules.TryGetValue(right, out beforeRight)) {
                        return 0;
                    }
                    return 1;
                });

                bool isGood = true;
                for (int j = 0; j < update.Count; j++) {
                    if (update[j] != updateOrig[j]) {
                        isGood = false;
                        break;
                    }
                }

                if (isGood == onlyGood) {
                    if (onlyGood) {
                        total += updateOrig[update.Count / 2];
                    } else {
                        total += update[update.Count / 2];
                    }
                }
            }
            return total;
        }
    }
}