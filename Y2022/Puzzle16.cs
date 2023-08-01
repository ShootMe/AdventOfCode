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
        private byte AAID, FlowRateMaxID;
        public override void Setup() {
            Dictionary<string, byte> ids = new();
            string[] lines = Input.Split('\n');
            valves = new Valve[lines.Length];
            for (int i = 0; i < lines.Length; i++) {
                string line = lines[i];
                string[] splits = line.SplitOn("Valve ", " has flow rate=", "; tunnel", "valve", " ");
                int connections = splits[5].Split(", ").Length;
                Valve valve = new Valve() {
                    Name = splits[1],
                    FlowRate = splits[2].ToInt(),
                    Connections = new Valve[connections],
                    Costs = new int[connections]
                };

                valves[i] = valve;
            }

            Array.Sort(valves);
            for (int i = 0; i < valves.Length; i++) {
                valves[i].ID = (byte)i;
                ids.Add(valves[i].Name, (byte)i);
                if (valves[i].FlowRate > 0) { FlowRateMaxID = (byte)i; }
            }
            FlowRateMaxID++;

            for (int i = 0; i < lines.Length; i++) {
                string line = lines[i];
                string[] splits = line.SplitOn("Valve ", " has flow rate=", "; tunnel", "valve", " ");
                Valve valve = valves[ids[splits[1]]];
                splits = splits[5].Split(", ");
                for (int j = 0; j < splits.Length; j++) {
                    Valve connection = valves[ids[splits[j]]];
                    valve.Connections[j] = connection;
                    valve.Costs[j] = 1;
                }
            }

            AAID = ids["AA"];

            for (int i = 0; i < valves.Length; i++) {
                Valve valve = valves[i];
                if (i < FlowRateMaxID || i == AAID) {
                    Flatten(valve);
                }
            }
        }
        private void Flatten(Valve valve) {
            Queue<(Valve, int)> open = new();
            Dictionary<Valve, int> found = new();
            open.Enqueue((valve, 0));
            found.Add(valve, 0);

            while (open.Count > 0) {
                (Valve next, int cost) = open.Dequeue();

                Valve[] connections = next.Connections;
                int[] costs = next.Costs;
                for (int i = 0; i < costs.Length; i++) {
                    int nextCost = cost + costs[i];

                    if (!found.TryGetValue(connections[i], out int currentCost) || currentCost > nextCost) {
                        found[connections[i]] = nextCost;
                        open.Enqueue((connections[i], nextCost));
                    }
                }
            }

            FlattenComplete(valve, found);
        }
        private void FlattenComplete(Valve valve, Dictionary<Valve, int> found) {
            int i = 0;
            foreach (KeyValuePair<Valve, int> pair in found) {
                if (pair.Value > 0 && pair.Key.FlowRate > 0) { i++; }
            }

            Valve[] connections = new Valve[i];
            int[] costs = new int[i];
            i = 0;
            foreach (KeyValuePair<Valve, int> pair in found) {
                if (pair.Value > 0 && pair.Key.FlowRate > 0) {
                    connections[i] = pair.Key;
                    costs[i++] = pair.Value;
                }
            }

            valve.Connections = connections;
            valve.Costs = costs;
        }

        [Description("What is the most pressure you can release?")]
        public override string SolvePart1() {
            Queue<State> open = new(15000);
            Dictionary<State, int> seen = new(30000);
            State current = new State() { ID = AAID, MinuteE = 30 };
            open.Enqueue(current);
            seen.Add(current, 0);

            int maxRelease = 0;
            int maxVisited = (1 << FlowRateMaxID) - 1;
            while (open.Count > 0) {
                current = open.Dequeue();

                if (seen[current] > current.Release) { continue; }
                if (current.Release > maxRelease) { maxRelease = current.Release; }
                if (current.Visited == maxVisited) { continue; }

                OpenConnections(current, seen, open, false);
            }
            
            return $"{maxRelease}";
        }

        [Description("With you and an elephant working together for 26 minutes, what is the most pressure you could release?")]
        public override string SolvePart2() {
            Queue<State> open = new(700000);
            Dictionary<State, int> seen = new(1500000);
            State current = new State() { ID = AAID, IDE = AAID, Minute = 4, MinuteE = 4 };
            open.Enqueue(current);
            seen.Add(current, 0);

            int maxRelease = 0;
            int maxVisited = (1 << FlowRateMaxID) - 1;
            while (open.Count > 0) {
                current = open.Dequeue();

                if (seen[current] > current.Release) { continue; }
                if (current.Release > maxRelease) { maxRelease = current.Release; }
                if (current.Visited == maxVisited) { continue; }

                byte time = current.CurrentTime();
                OpenConnections(current, seen, open, current.Minute != time);
            }

            return $"{maxRelease}";
        }
        private void OpenConnections(State current, Dictionary<State, int> seen, Queue<State> open, bool isElephant) {
            Valve valve = valves[isElephant ? current.IDE : current.ID];
            Valve[] connections = valve.Connections;
            int[] costs = valve.Costs;
            byte minute = isElephant ? current.MinuteE : current.Minute;
            bool added = false;
            for (int i = 0; i < connections.Length; i++) {
                Valve next = connections[i];

                if ((current.Visited & (1 << next.ID)) == 0) {
                    byte newMinute = (byte)(minute + costs[i] + 1);
                    if (newMinute >= 30) { continue; }

                    int newRelease = current.Release + next.FlowRate * (30 - newMinute);
                    State newState = new State() {
                        ID = isElephant ? current.ID : next.ID,
                        IDE = isElephant ? next.ID : current.IDE,
                        Release = newRelease,
                        Visited = current.Visited | (1 << next.ID),
                        Minute = isElephant ? current.Minute : newMinute,
                        MinuteE = isElephant ? newMinute : current.MinuteE
                    };
                    if (!seen.TryGetValue(newState, out int best) || best < newRelease) {
                        seen[newState] = newRelease;
                        open.Enqueue(newState);
                        added = true;
                    }
                }
            }

            if (!added && current.Minute < 30 && current.MinuteE < 30) {
                if (isElephant) {
                    current.MinuteE = 30;
                } else {
                    current.Minute = 30;
                }
                open.Enqueue(current);
            }
        }
        private struct State : IEquatable<State> {
            public byte ID;
            public byte IDE;
            public byte Minute;
            public byte MinuteE;
            public int Release;
            public int Visited;

            public byte CurrentTime() { return Minute <= MinuteE ? Minute : MinuteE; }
            public override bool Equals(object obj) { return obj is State state && Equals(state); }
            public override int GetHashCode() { return (ID * 17 + IDE) * 17 + Visited; }
            public bool Equals(State other) { return ID == other.ID && IDE == other.IDE && Visited == other.Visited; }
            public override string ToString() { return $"ID: {ID} IDE: {IDE} Min: {Minute} MinE: {MinuteE} Rel: {Release} Vis: {Visited}"; }
        }
        private class Valve : IEquatable<Valve>, IComparable<Valve> {
            public byte ID;
            public string Name;
            public int FlowRate;
            public Valve[] Connections;
            public int[] Costs;

            public int CompareTo(Valve other) { return other.FlowRate.CompareTo(FlowRate); }
            public override bool Equals(object obj) { return obj is Valve valve && Equals(valve); }
            public override int GetHashCode() { return ID; }
            public bool Equals(Valve other) { return ID == other.ID; }
            public override string ToString() {
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < Connections.Length; i++) {
                    sb.Append($"{Connections[i].Name}-{Costs[i]},");
                }
                sb.Length--;
                return $"{Name}({FlowRate}) {sb}";
            }
        }
    }
}