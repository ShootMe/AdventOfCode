using AdventOfCode.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
namespace AdventOfCode.Y2019 {
    [Description("Universal Orbit Map")]
    public class Puzzle06 : ASolver {
        private HashSet<Orbit> orbits;

        public override void Setup() {
            List<string> items = Tools.GetLines(Input);
            orbits = new HashSet<Orbit>();
            for (int i = 0; i < items.Count; i++) {
                string[] names = items[i].Split(')');

                Orbit orbit1 = new Orbit() { Name = names[0] };
                if (!orbits.TryGetValue(orbit1, out orbit1)) {
                    orbit1 = new Orbit() { Name = names[0] };
                    orbits.Add(orbit1);
                }

                Orbit orbit2 = new Orbit() { Name = names[1] };
                if (!orbits.TryGetValue(orbit2, out orbit2)) {
                    orbit2 = new Orbit() { Name = names[1] };
                    orbits.Add(orbit2);
                }

                orbit1.Satellites.Add(orbit2);
                orbit2.Parent = orbit1;
            }
        }

        [Description("What is the total number of direct and indirect orbits?")]
        public override string SolvePart1() {
            Orbit com = new Orbit() { Name = "COM" };
            orbits.TryGetValue(com, out com);

            return $"{com.Count()}";
        }

        [Description("What is the minimum number of orbital transfers to move from YOU to SAN?")]
        public override string SolvePart2() {
            Orbit san = new Orbit() { Name = "SAN" };
            orbits.TryGetValue(san, out san);
            Orbit you = new Orbit() { Name = "YOU" };
            orbits.TryGetValue(you, out you);

            Dictionary<Orbit, int> counts = new Dictionary<Orbit, int>();
            int levelYou = -1;
            int levelSan = -1;

            int count;
            while (true) {
                you = you?.Parent;
                if (you != null) {
                    levelYou++;
                    if (counts.TryGetValue(you, out count)) {
                        return $"{count + levelYou}";
                    }
                    counts.Add(you, levelYou);
                }

                san = san?.Parent;
                if (san != null) {
                    levelSan++;
                    if (counts.TryGetValue(san, out count)) {
                        return $"{count + levelSan}";
                    }
                    counts.Add(san, levelSan);
                }
            }
        }

        private class Orbit : IEquatable<Orbit> {
            public Orbit Parent;
            public List<Orbit> Satellites = new List<Orbit>();
            public string Name;

            public int Count(int level = 0) {
                int count = 0;
                for (int i = 0; i < Satellites.Count; i++) {
                    count += Satellites[i].Count(level + 1);
                }
                return count + Satellites.Count + (level > 0 ? level - 1 : 0);
            }
            public bool Equals(Orbit other) {
                return Name.Equals(other.Name, StringComparison.OrdinalIgnoreCase);
            }
            public override bool Equals(object obj) {
                return obj is Orbit orbit && orbit.Name.Equals(Name, StringComparison.OrdinalIgnoreCase);
            }
            public override int GetHashCode() {
                return Name.GetHashCode();
            }
            public override string ToString() {
                return $"{Parent?.Name} ) {Name} ) {Satellites.Count}";
            }
        }
    }
}