using AdventOfCode.Core;
using System.Collections.Generic;
using System.ComponentModel;
namespace AdventOfCode.Y2020 {
    public class Puzzle05 : ASolver {
        private List<string> items;
        public Puzzle05(string input) : base(input) {Name = "Binary Boarding";}

        public override void Setup() {
            items = Tools.GetLines(Input);
        }

        [Description("What is the highest seat ID on a boarding pass?")]
        public override string SolvePart1() {
            int maxID = 0;
            for (int i = 0; i < items.Count; i++) {
                string item = items[i];
                int rowStart = 0;
                int rowEnd = 127;
                for (int j = 0; j < 7; j++) {
                    if (item[j] == 'F') {
                        rowEnd = (rowStart + rowEnd) >> 1;
                    } else {
                        rowStart = (rowStart + rowEnd) >> 1;
                    }
                }
                int colStart = 0;
                int colEnd = 7;
                for (int j = 7; j < 10; j++) {
                    if (item[j] == 'L') {
                        colEnd = (colStart + colEnd) >> 1;
                    } else {
                        colStart = (colStart + colEnd) >> 1;
                    }
                }

                int id = rowEnd * 8 + colEnd;
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
                string item = items[i];
                int rowStart = 0;
                int rowEnd = 127;
                for (int j = 0; j < 7; j++) {
                    if (item[j] == 'F') {
                        rowEnd = (rowStart + rowEnd) >> 1;
                    } else {
                        rowStart = (rowStart + rowEnd) >> 1;
                    }
                }
                int colStart = 0;
                int colEnd = 7;
                for (int j = 7; j < 10; j++) {
                    if (item[j] == 'L') {
                        colEnd = (colStart + colEnd) >> 1;
                    } else {
                        colStart = (colStart + colEnd) >> 1;
                    }
                }

                int id = rowEnd * 8 + colEnd;
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