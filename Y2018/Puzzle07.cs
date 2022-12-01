using AdventOfCode.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
namespace AdventOfCode.Y2018 {
    [Description("The Sum of Its Parts")]
    public class Puzzle07 : ASolver {
        private HashSet<AssemblyStep> steps;

        private void SetupSteps() {
            List<string> items = Tools.GetLines(Input);
            steps = new HashSet<AssemblyStep>();

            for (int i = 0; i < items.Count; i++) {
                string item = items[i];
                char stepDepends = item[5];
                char step = item[36];

                AssemblyStep info = new AssemblyStep() { Name = step };

                AssemblyStep temp;
                if (!steps.TryGetValue(info, out temp)) {
                    steps.Add(info);
                } else {
                    info = temp;
                }

                info.DependsOn.Add(stepDepends);

                info = new AssemblyStep() { Name = stepDepends };

                if (!steps.TryGetValue(info, out temp)) {
                    steps.Add(info);
                }
            }
        }

        [Description("In what order should the steps in your instructions be completed?")]
        public override string SolvePart1() {
            SetupSteps();

            List<AssemblyStep> topLevel = new List<AssemblyStep>();
            List<AssemblyStep> leftToExecute = new List<AssemblyStep>();
            foreach (AssemblyStep step in steps) {
                if (step.DependsOn.Count == 0) {
                    topLevel.Add(step);
                } else {
                    leftToExecute.Add(step);
                }
            }

            topLevel.Sort(delegate (AssemblyStep left, AssemblyStep right) {
                return right.Name.CompareTo(left.Name);
            });

            StringBuilder order = new StringBuilder(steps.Count);
            while (topLevel.Count > 0) {
                AssemblyStep step = topLevel[topLevel.Count - 1];
                topLevel.RemoveAt(topLevel.Count - 1);

                order.Append(step.Name);

                for (int j = leftToExecute.Count - 1; j >= 0; j--) {
                    AssemblyStep left = leftToExecute[j];

                    if (left.DependsOn.Remove(step.Name) && left.DependsOn.Count == 0) {
                        topLevel.Add(left);
                    }
                }

                topLevel.Sort(delegate (AssemblyStep left, AssemblyStep right) {
                    return right.Name.CompareTo(left.Name);
                });
            }

            return order.ToString();
        }

        [Description("How long will it take to complete all of the steps?")]
        public override string SolvePart2() {
            SetupSteps();

            List<AssemblyStep> topLevel = new List<AssemblyStep>();
            List<AssemblyStep> leftToExecute = new List<AssemblyStep>();
            foreach (AssemblyStep step in steps) {
                if (step.DependsOn.Count == 0) {
                    topLevel.Add(step);
                } else {
                    leftToExecute.Add(step);
                }
            }

            topLevel.Sort(delegate (AssemblyStep left, AssemblyStep right) {
                return right.Name.CompareTo(left.Name);
            });

            int timeSpent = 0;
            int workers = 5;
            List<AssemblyStep> working = new List<AssemblyStep>();
            while (topLevel.Count > 0 || working.Count > 0) {
                while (topLevel.Count > 0 && working.Count < workers) {
                    AssemblyStep step = topLevel[topLevel.Count - 1];
                    step.TimeLeft = 61 + (byte)step.Name - 0x41;
                    topLevel.RemoveAt(topLevel.Count - 1);
                    working.Add(step);
                }

                timeSpent++;

                for (int i = working.Count - 1; i >= 0; i--) {
                    AssemblyStep step = working[i];
                    step.TimeLeft--;

                    if (step.TimeLeft == 0) {
                        working.RemoveAt(i);

                        for (int j = leftToExecute.Count - 1; j >= 0; j--) {
                            AssemblyStep left = leftToExecute[j];

                            if (left.DependsOn.Remove(step.Name) && left.DependsOn.Count == 0) {
                                topLevel.Add(left);
                            }
                        }

                        topLevel.Sort(delegate (AssemblyStep left, AssemblyStep right) {
                            return right.Name.CompareTo(left.Name);
                        });
                    }
                }
            }

            return $"{timeSpent}";
        }

        private class AssemblyStep : IEquatable<AssemblyStep> {
            public List<char> DependsOn = new List<char>();
            public char Name;
            public int TimeLeft;

            public override string ToString() {
                return $"{Name} DependsOn({DependsOn.Count})";
            }
            public bool Equals(AssemblyStep other) {
                return Name == other.Name;
            }
            public override bool Equals(object obj) {
                return obj is AssemblyStep step && step.Name == Name;
            }
            public override int GetHashCode() {
                return Name;
            }
        }
    }
}