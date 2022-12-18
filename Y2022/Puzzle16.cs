using AdventOfCode.Common;
using AdventOfCode.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
namespace AdventOfCode.Y2022 {
    [Description("Proboscidea Volcanium")]
    public class Puzzle16 : ASolver {
        private Valve[] valves;
        private int AAID, FlowRateMaxID;
        public override void Setup() {
            Dictionary<string, int> ids = new();
            string[] lines = Input.Split('\n');
            valves = new Valve[lines.Length];
            for (int i = 0; i < lines.Length; i++) {
                string line = lines[i];
                string[] splits = line.SplitOn("Valve ", " has flow rate=", "; tunnel", "valve", " ");
                Valve valve = new Valve() {
                    Name = splits[1],
                    FlowRate = splits[2].ToInt(),
                    Connections = new()
                };

                valves[i] = valve;
            }

            Array.Sort(valves);
            for (int i = 0; i < valves.Length; i++) {
                valves[i].ID = i;
                ids.Add(valves[i].Name, i);
                if (valves[i].FlowRate > 0) { FlowRateMaxID = i; }
            }

            for (int i = 0; i < lines.Length; i++) {
                string line = lines[i];
                string[] splits = line.SplitOn("Valve ", " has flow rate=", "; tunnel", "valve", " ");
                Valve valve = valves[ids[splits[1]]];
                splits = splits[5].Split(", ");
                for (int j = 0; j < splits.Length; j++) {
                    valve.Connections.Add(ids[splits[j]]);
                }
            }
            AAID = ids["AA"];
        }

        [Description("What is the most pressure you can release?")]
        public override string SolvePart1() {
            Queue<(int, int, int, int)> open = new();
            Dictionary<(int, int), int> seen = new();
            open.Enqueue((AAID, 0, 0, 0));
            seen.Add((AAID, 0), -1);
            int maxRelease = 0;

            while (open.Count > 0) {
                (int ID, int Release, int Visited, int Minute) = open.Dequeue();

                if (Release > maxRelease) {
                    maxRelease = Release;
                }

                Valve valve = valves[ID];
                if (valve.FlowRate > 0 && (Visited & (1 << ID)) == 0) {
                    int toRelease = Release + valve.FlowRate * (30 - Minute - 1);
                    int newVisited = Visited | (1 << ID);
                    if (!seen.TryGetValue((ID, newVisited), out int bestRelease) || bestRelease < toRelease) {
                        seen[(ID, newVisited)] = toRelease;
                        open.Enqueue((ID, toRelease, newVisited, Minute + 1));
                    }
                }

                if (Minute >= 28) { continue; }

                for (int j = 0; j < valve.Connections.Count; j++) {
                    Valve next = valves[valve.Connections[j]];
                    if (!seen.TryGetValue((next.ID, Visited), out int bestRelease) || bestRelease < Release) {
                        seen[(next.ID, Visited)] = Release;
                        open.Enqueue((next.ID, Release, Visited, Minute + 1));
                    }
                }
            }
            return $"{maxRelease}";
        }

        [Description("With you and an elephant working together for 26 minutes, what is the most pressure you could release?")]
        public override string SolvePart2() {
            return string.Empty;
        }

        private class Valve : IEquatable<Valve>, IComparable<Valve> {
            public int ID;
            public string Name;
            public int FlowRate;
            public List<int> Connections;

            public int CompareTo(Valve other) {
                return other.FlowRate.CompareTo(FlowRate);
            }
            public bool Equals(Valve other) {
                return ID == other.ID;
            }
            public override string ToString() {
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < Connections.Count; i++) {
                    sb.Append(Connections[i]).Append(',');
                }
                sb.Length--;
                return $"{Name}({FlowRate}) {sb}";
            }
        }
    }
}