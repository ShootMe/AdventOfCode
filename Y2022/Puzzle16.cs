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
                    valve.Costs[j] = connection.FlowRate > 0 ? 1 : 0;
                }
            }

            for (int i = 0; i < valves.Length; i++) {
                Valve valve = valves[i];
                Flatten(valve);
            }

            AAID = ids["AA"];
        }
        private void Flatten(Valve valve) {
            if (!valve.NeedsFlattened()) { return; }

            Valve[] connections = valve.Connections;
            int[] costs = valve.Costs;
            Queue<(Valve, int)> open = new Queue<(Valve, int)>();
            Dictionary<Valve, int> found = new Dictionary<Valve, int>();

            for (int j = 0; j < costs.Length; j++) {
                if (connections[j] == valve) { continue; }

                if (costs[j] != 0) {
                    found.Add(connections[j], costs[j]);
                } else {
                    found.Add(connections[j], -1);
                    open.Enqueue((connections[j], 1));
                }
            }

            while (open.Count > 0) {
                (Valve next, int cost) = open.Dequeue();

                Flatten(open, found, valve, next, cost);
            }

            int i = 0;
            foreach (KeyValuePair<Valve, int> pair in found) {
                if (pair.Value > 0) { i++; }
            }

            connections = new Valve[i];
            costs = new int[i];
            i = 0;
            foreach (KeyValuePair<Valve, int> pair in found) {
                if (pair.Value > 0) {
                    connections[i] = pair.Key;
                    costs[i++] = pair.Value;
                }
            }

            valve.Connections = connections;
            valve.Costs = costs;
        }
        private void Flatten(Queue<(Valve, int)> open, Dictionary<Valve, int> found, Valve valve, Valve next, int cost) {
            Valve[] connections = next.Connections;
            int[] costs = next.Costs;
            for (int j = 0; j < costs.Length; j++) {
                if (connections[j] == valve) { continue; }

                if (costs[j] != 0) {
                    if (!found.TryGetValue(connections[j], out int currentCost) || currentCost > cost + costs[j]) {
                        found[connections[j]] = cost + costs[j];
                    }
                } else if (!found.TryGetValue(connections[j], out int currentCost) || currentCost < -cost - 1) {
                    found[connections[j]] = -cost - 1;
                    open.Enqueue((connections[j], cost + 1));
                }
            }
        }

        [Description("What is the most pressure you can release?")]
        public override string SolvePart1() {
            Queue<State> open = new(20000);
            Dictionary<State, int> seen = new(80000);
            State current = new State() { ID = AAID };
            open.Enqueue(current);
            seen.Add(current, 0);

            int maxRelease = 0;
            int maxVisited = (1 << FlowRateMaxID) - 1;
            while (open.Count > 0) {
                current = open.Dequeue();

                if (current.Minute > 30 || seen[current] > current.Release) { continue; }
                if (current.Release > maxRelease) { maxRelease = current.Release; }
                if (current.Visited == maxVisited || current.Minute == 30) { continue; }

                Valve valve = valves[current.ID];
                Valve[] connections = valve.Connections;
                int[] costs = valve.Costs;

                if (valve.FlowRate > 0 && (current.Visited & (1 << current.ID)) == 0) {
                    int newRelease = current.Release + valve.FlowRate * (29 - current.Minute);
                    int newVisited = current.Visited | (1 << current.ID);
                    byte newMinute = (byte)(current.Minute + 1);
                    State newState = new State() { ID = current.ID, Release = newRelease, Visited = newVisited, Minute = newMinute };
                    if (!seen.TryGetValue(newState, out int best) || best < newRelease) {
                        seen[newState] = newRelease;
                        open.Enqueue(newState);
                    }
                }

                for (int i = 0; i < connections.Length; i++) {
                    Valve next = connections[i];
                    byte newMinute = (byte)(current.Minute + costs[i]);
                    if (newMinute > 30) { continue; }

                    State newState = new State() { ID = next.ID, Release = current.Release, Visited = current.Visited, Minute = newMinute };
                    if (!seen.TryGetValue(newState, out int best) || best < current.Release) {
                        seen[newState] = current.Release;
                        open.Enqueue(newState);
                    }
                }
            }

            return $"{maxRelease}";
        }

        [Description("With you and an elephant working together for 26 minutes, what is the most pressure you could release?")]
        public override string SolvePart2() {
            return string.Empty;

            //Takes about 74 seconds to solve correctly
            Queue<State> open = new(2000000);
            Dictionary<State, int> seen = new(6500000);
            State current = new State() { ID = AAID, IDE = AAID, Minute = 4, MinuteE = 4 };
            open.Enqueue(current);
            seen.Add(current, 0);

            int maxRelease = 0;
            int maxVisited = (1 << FlowRateMaxID) - 1;
            while (open.Count > 0) {
                current = open.Dequeue();

                byte time = current.CurrentTime();
                if (time > 30 || seen[current] > current.Release) { continue; }
                if (current.Release > maxRelease) { maxRelease = current.Release; }
                if (current.Visited == maxVisited || time == 30) { continue; }

                if (current.Minute == time) {
                    Valve valve = valves[current.ID];
                    Valve[] connections = valve.Connections;
                    int[] costs = valve.Costs;

                    if (valve.FlowRate > 0 && (current.Visited & (1 << current.ID)) == 0) {
                        int newRelease = current.Release + valve.FlowRate * (29 - time);
                        int newVisited = current.Visited | (1 << current.ID);
                        byte newMinute = (byte)(time + 1);
                        State newState = new State() { ID = current.ID, IDE = current.IDE, Release = newRelease, Visited = newVisited, Minute = newMinute, MinuteE = current.MinuteE };
                        if (!seen.TryGetValue(newState, out int best) || best < newRelease) {
                            seen[newState] = newRelease;
                            open.Enqueue(newState);
                        }
                    }

                    for (int i = 0; i < connections.Length; i++) {
                        Valve next = connections[i];
                        byte newMinute = (byte)(current.Minute + costs[i]);
                        State newState = new State() { ID = next.ID, IDE = current.IDE, Release = current.Release, Visited = current.Visited, Minute = newMinute, MinuteE = current.MinuteE };
                        if (!seen.TryGetValue(newState, out int best) || best < current.Release) {
                            seen[newState] = current.Release;
                            open.Enqueue(newState);
                        }
                    }
                } else {
                    Valve valveE = valves[current.IDE];
                    Valve[] connectionsE = valveE.Connections;
                    int[] costsE = valveE.Costs;

                    if (valveE.FlowRate > 0 && (current.Visited & (1 << current.IDE)) == 0) {
                        int newRelease = current.Release + valveE.FlowRate * (29 - time);
                        int newVisited = current.Visited | (1 << current.IDE);
                        byte newMinute = (byte)(time + 1);
                        State newState = new State() { ID = current.ID, IDE = current.IDE, Release = newRelease, Visited = newVisited, Minute = current.Minute, MinuteE = newMinute };
                        if (!seen.TryGetValue(newState, out int best) || best < newRelease) {
                            seen[newState] = newRelease;
                            open.Enqueue(newState);
                        }
                    }

                    for (int i = 0; i < connectionsE.Length; i++) {
                        Valve next = connectionsE[i];
                        byte newMinute = (byte)(time + costsE[i]);
                        State newState = new State() { ID = current.ID, IDE = next.ID, Release = current.Release, Visited = current.Visited, Minute = current.Minute, MinuteE = newMinute };
                        if (!seen.TryGetValue(newState, out int best) || best < current.Release) {
                            seen[newState] = current.Release;
                            open.Enqueue(newState);
                        }
                    }
                }
            }

            return $"{maxRelease}";
        }
        private struct State : IEquatable<State>, IComparable<State> {
            public byte ID;
            public byte IDE;
            public byte Minute;
            public byte MinuteE;
            public int Release;
            public int Visited;

            public byte CurrentTime() { return Minute <= MinuteE ? Minute : MinuteE; }
            public int CompareTo(State other) { return ((Minute + MinuteE) * 5000 - Release).CompareTo((other.Minute + other.MinuteE) * 5000 - other.Release); }
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

            public bool NeedsFlattened() {
                for (int i = 0; i < Costs.Length; i++) {
                    if (Costs[i] == 0) {
                        return true;
                    }
                }
                return false;
            }
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