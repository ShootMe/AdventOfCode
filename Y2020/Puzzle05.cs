using AdventOfCode.Common;
using AdventOfCode.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
namespace AdventOfCode.Y2020 {
    [Description("Binary Boarding")]
    public class Puzzle05 : ASolver {
        private List<string> items;

        public override void Setup() {
            items = Input.Lines();
        }

        [Description("What is the highest seat ID on a boarding pass?")]
        public override string SolvePart1() {
            int maxID = 0;
            for (int i = 0; i < items.Count; i++) {
                string item = items[i].Replace('F', '0').Replace('B', '1').Replace('R', '1').Replace('L', '0');
                int id = Convert.ToInt32(item, 2);
                if (id > maxID) {
                    maxID = id;
                }
            }
            return $"{maxID}";
        }

        [Description("What is the ID of your seat?")]
        public override string SolvePart2() {
            HashSet<int> ids = new HashSet<int>();
            for (int i = 0; i < items.Count; i++) {
                string item = items[i].Replace('F', '0').Replace('B', '1').Replace('R', '1').Replace('L', '0');
                int id = Convert.ToInt32(item, 2);
                ids.Add(id);
            }

            for (int i = 8; i < 1016; i++) {
                if (ids.Contains(i - 1) && ids.Contains(i + 1) && !ids.Contains(i)) {
                    return $"{i}";
                }
            }
            return string.Empty;
        }
    }
}