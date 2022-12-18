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
            FlowRateMaxID++;

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
            seen.Add((AAID, 0), 0);
            List<(int id, int cost, int firstID)> costs = new();
            int maxRelease = 0;
            int maxVisited = (1 << FlowRateMaxID) - 1;
            while (open.Count > 0) {
                (int ID, int Release, int Visited, int Minute) = open.Dequeue();

                if (seen[(ID, Visited)] > Release) { continue; }
                if (Release > maxRelease) { maxRelease = Release; }
                if (Visited == maxVisited) { continue; }

                Valve valve = valves[ID];
                if (valve.FlowRate > 0 && (Visited & (1 << ID)) == 0) {
                    int newRelease = Release + valve.FlowRate * (29 - Minute);
                    int newVisited = Visited | (1 << ID);
                    if (!seen.TryGetValue((ID, newVisited), out int best) || best < newRelease) {
                        seen[(ID, newVisited)] = newRelease;
                        open.Enqueue((ID, newRelease, newVisited, Minute + 1));
                    }
                }

                CalculateCosts(costs, ID, Visited, Minute);

                for (int i = 0; i < 3 && i < costs.Count; i++) {
                    (int id, int cost, int firstID) = costs[i];

                    if (!seen.TryGetValue((firstID, Visited), out int best) || best < Release) {
                        seen[(firstID, Visited)] = Release;
                        open.Enqueue((firstID, Release, Visited, Minute + 1));
                    }
                }
            }
            return $"{maxRelease}";
        }

        [Description("With you and an elephant working together for 26 minutes, what is the most pressure you could release?")]
        public override string SolvePart2() {
            return string.Empty;
            //Fix Later
            //Queue<(int, int, int, int, int)> open = new();
            //Dictionary<(int, int, int), int> seen = new();
            //open.Enqueue((AAID, AAID, 0, 0, 8));
            //seen.Add((AAID, AAID, 0), 0);
            //List<(int id, int cost, int firstID)> costs = new();
            //int maxRelease = 0;
            //int maxVisited = (1 << FlowRateMaxID) - 1;
            //while (open.Count > 0) {
            //    (int ID, int IDE, int Release, int Visited, int Minute) = open.Dequeue();

            //    if (seen[(ID, IDE, Visited)] > Release) { continue; }
            //    if (Release > maxRelease) { maxRelease = Release; }
            //    if (Visited == maxVisited) { continue; }

            //    int currentID = (Minute & 1) == 0 ? ID : IDE;
            //    int minute = Minute / 2;

            //    Valve valve = valves[currentID];
            //    if (valve.FlowRate > 0 && (Visited & (1 << currentID)) == 0) {
            //        int newRelease = Release + valve.FlowRate * (29 - minute);
            //        int newVisited = Visited | (1 << currentID);
            //        if (!seen.TryGetValue((ID, IDE, newVisited), out int best) || best < newRelease) {
            //            seen[(ID, IDE, newVisited)] = newRelease;
            //            open.Enqueue((ID, IDE, newRelease, newVisited, Minute + 1));
            //        }
            //    }

            //    CalculateCosts(costs, currentID, Visited, minute);

            //    for (int i = 0; i < 3 && i < costs.Count; i++) {
            //        (int id, int cost, int firstID) = costs[i];

            //        if ((Minute & 1) == 0) {
            //            if (!seen.TryGetValue((firstID, IDE, Visited), out int best) || best < Release) {
            //                seen[(firstID, IDE, Visited)] = Release;
            //                open.Enqueue((firstID, IDE, Release, Visited, Minute + 1));
            //            }
            //        } else if (!seen.TryGetValue((ID, firstID, Visited), out int best) || best < Release) {
            //            seen[(ID, firstID, Visited)] = Release;
            //            open.Enqueue((ID, firstID, Release, Visited, Minute + 1));
            //        }
            //    }
            //}
            //return $"{maxRelease}";
        }
        private void CalculateCosts(List<(int id, int cost, int firstID)> costs, int id, int visited, int minute) {
            costs.Clear();
            for (int i = 0; i < FlowRateMaxID; i++) {
                if (id == i || (visited & (1 << i)) != 0) { continue; }
                (int cost, int firstID) = CalculateCost(id, i);
                if (minute + cost >= 29) { continue; }
                costs.Add((i, cost, firstID));
            }
            costs.Sort((left, right) => { return (valves[right.id].FlowRate * (29 - minute - right.cost)).CompareTo(valves[left.id].FlowRate * (29 - minute - left.cost)); });
        }
        private Dictionary<(int, int), (int, int)> calculatedCosts = new();
        private (int cost, int firstID) CalculateCost(int from, int to) {
            if (calculatedCosts.TryGetValue((from, to), out var cost)) { return cost; }

            Queue<(int, int, int)> open = new();
            HashSet<int> seen = new();
            open.Enqueue((from, -1, 0));
            seen.Add(from);

            while (open.Count > 0) {
                (int ID, int FromID, int Cost) = open.Dequeue();

                if (ID == to) {
                    calculatedCosts.Add((from, to), (Cost, FromID));
                    return (Cost, FromID);
                }

                Valve valve = valves[ID];

                for (int i = 0; i < valve.Connections.Count; i++) {
                    Valve next = valves[valve.Connections[i]];
                    if (seen.Add(next.ID)) {
                        open.Enqueue((next.ID, FromID < 0 ? next.ID : FromID, Cost + 1));
                    }
                }
            }
            return (-1, -1);
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