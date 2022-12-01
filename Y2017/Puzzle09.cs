using AdventOfCode.Core;
using System.Collections.Generic;
using System.ComponentModel;
namespace AdventOfCode.Y2017 {
    [Description("Stream Processing")]
    public class Puzzle09 : ASolver {
        private Group root;

        public override void Setup() {
            root = new Group(Input);
        }

        [Description("What is the total score for all groups?")]
        public override string SolvePart1() {
            return $"{root.GetScore()}";
        }

        [Description("How many non-canceled characters are within the garbage?")]
        public override string SolvePart2() {
            return $"{root.GetTotalGarbage()}";
        }

        private class Group {
            public List<Group> Children = new List<Group>();
            public List<string> Garbage = new List<string>();
            public Group(string value) {
                int start = 0;
                Initialize(value, ref start);
            }
            private Group(string value, ref int start) {
                Initialize(value, ref start);
            }
            public int GetScore(int level = 1) {
                int total = level;
                for (int i = 0; i < Children.Count; i++) {
                    total += Children[i].GetScore(level + 1);
                }
                return total;
            }
            public int GetTotalGarbage() {
                int total = 0;
                for (int i = 0; i < Garbage.Count; i++) {
                    string garbage = Garbage[i];
                    for (int j = 0; j < garbage.Length; j++) {
                        char g = garbage[j];
                        if (g == '!') {
                            j++;
                        } else {
                            total++;
                        }
                    }
                }
                for (int i = 0; i < Children.Count; i++) {
                    total += Children[i].GetTotalGarbage();
                }
                return total;
            }
            private void Initialize(string value, ref int start) {
                start++;
                while (start < value.Length) {
                    char current = value[start];
                    if (current == '<') {
                        Garbage.Add(GetGarbage(value, ref start));
                    } else if (current == '}') {
                        break;
                    } else if (current == '{') {
                        Children.Add(new Group(value, ref start));
                    }
                    start++;
                }
            }
            private string GetGarbage(string value, ref int start) {
                start++;
                int index = start;
                while (start < value.Length) {
                    char current = value[start];
                    if (current == '!') {
                        start++;
                    } else if (current == '>') {
                        break;
                    }
                    start++;
                }

                return value.Substring(index, start - index);
            }
            public override string ToString() {
                return $"{Children.Count} {Garbage.Count}";
            }
        }
    }
}