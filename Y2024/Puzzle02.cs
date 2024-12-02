using AdventOfCode.Common;
using AdventOfCode.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
namespace AdventOfCode.Y2024 {
    [Description("Red-Nosed Reports")]
    public class Puzzle02 : ASolver {
        private List<Report> reports = new();

        public override void Setup() {
            string[] lines = Input.Split('\n');
            foreach (string line in lines) {
                reports.Add(new Report(line));
            }
        }

        [Description("How many reports are safe?")]
        public override string SolvePart1() {
            int total = 0;
            for (int i = 0; i < reports.Count; i++) {
                total += reports[i].IsSafe ? 1 : 0;
            }
            return $"{total}";
        }

        [Description("How many reports are now safe?")]
        public override string SolvePart2() {
            int total = 0;
            for (int i = 0; i < reports.Count; i++) {
                total += reports[i].IsTolerate ? 1 : 0;
            }
            return $"{total}";
        }
        private class Report {
            public int[] Levels;
            public bool IsSafe, IsTolerate;
            public Report(string line) {
                string[] splits = line.Split(' ');
                Levels = new int[splits.Length];
                for (int i = 0; i < splits.Length; i++) {
                    Levels[i] = splits[i].ToInt();
                }
                IsSafe = GetSafe(Levels);
                SetTolerate();
            }
            private bool GetSafe(int[] levels) {
                int way = levels[0] - levels[1];
                bool isSafe = Math.Abs(way) <= 3 && Math.Abs(way) > 0;
                for (int i = 2; i < levels.Length; i++) {
                    int next = levels[i - 1] - levels[i];
                    if (Math.Sign(next) != Math.Sign(way) || (Math.Abs(next) > 3 && Math.Abs(next) > 0)) {
                        return false;
                    }
                }
                return isSafe;
            }
            private void SetTolerate() {
                IsTolerate = IsSafe;
                if (IsSafe) { return; }

                for (int i = 0; i < Levels.Length; i++) {
                    if (HelpSetTolerate(i)) {
                        IsTolerate = true;
                        break;
                    }
                }
            }
            private bool HelpSetTolerate(int ignore) {
                int[] levels = new int[Levels.Length - 1];
                int index = 0;
                for (int i = 0; i < Levels.Length; i++) {
                    if (i == ignore) { continue; }

                    levels[index++] = Levels[i];
                }
                return GetSafe(levels);
            }
        }
    }
}