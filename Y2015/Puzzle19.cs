using AdventOfCode.Common;
using AdventOfCode.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
namespace AdventOfCode.Y2015 {
    [Description("Medicine for Rudolph")]
    public class Puzzle19 : ASolver {
        private List<KeyValuePair<string, string>> replacements;
        private string medicine;

        public override void Setup() {
            List<string> items = Input.Lines();
            replacements = new List<KeyValuePair<string, string>>();
            for (int i = items.Count - 3; i >= 0; i--) {
                string item = items[i];

                int index = item.IndexOf('=');
                replacements.Add(new KeyValuePair<string, string>(item.Substring(0, index - 1), item.Substring(index + 3)));
            }

            medicine = items[^1];
        }

        [Description("How many distinct molecules can be created?")]
        public override string SolvePart1() {
            HashSet<string> molecules = new HashSet<string>();
            for (int i = 0; i < medicine.Length; i++) {
                foreach (KeyValuePair<string, string> replacement in replacements) {
                    if (medicine.IndexOf(replacement.Key, i) == i) {
                        molecules.Add(medicine.Remove(i, replacement.Key.Length).Insert(i, replacement.Value));
                    }
                }
            }
            return $"{molecules.Count}";
        }

        [Description("What is the fewest number of steps to go from e to the medicine molecule?")]
        public override string SolvePart2() {
            HashSet<string> closed = new HashSet<string>(20000);
            Heap<Molecule> open = new Heap<Molecule>(15000);

            closed.Add(medicine);
            open.Enqueue(new Molecule() { Value = medicine, Steps = 0 });

            int bestSteps = int.MaxValue;
            while (open.Count > 0) {
                Molecule current = open.Dequeue();

                if (current.Value.Length == 1) {
                    if (current.Steps < bestSteps) {
                        bestSteps = current.Steps;
                    }
                }
                if (bestSteps < int.MaxValue && current.Value.Length > 1) {
                    continue;
                }

                string value = current.Value;
                foreach (KeyValuePair<string, string> replacement in replacements) {
                    int i = -1;
                    while ((i = value.IndexOf(replacement.Value, i + 1)) >= 0) {
                        string newValue = value.Remove(i, replacement.Value.Length).Insert(i, replacement.Key);
                        if (closed.Add(newValue)) {
                            open.Enqueue(new Molecule() { Value = newValue, Steps = current.Steps + 1 });
                        }
                    }
                }
            }
            return $"{bestSteps}";
        }

        private class Molecule : IComparable<Molecule> {
            public string Value;
            public int Steps;
            public int CompareTo(Molecule other) {
                int compare = Value.Length.CompareTo(other.Value.Length);
                if (compare != 0) { return compare; }

                return Steps.CompareTo(other.Steps);
            }
            public override string ToString() {
                return $"{Value} {Steps}";
            }
        }
    }
}