using AdventOfCode.Common;
using AdventOfCode.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
namespace AdventOfCode.Y2022 {
    [Description("Beacon Exclusion Zone")]
    public class Puzzle15 : ASolver {
        private List<Sensor> sensors = new();

        public override void Setup() {
            foreach (string line in Input.Split('\n')) {
                string[] splits = line.SplitOn("x=", ", y=", ": closest beacon is at x=", ", y=");
                sensors.Add(new Sensor() {
                    Position = new Point() { X = splits[1].ToInt(), Y = splits[2].ToInt() },
                    Beacon = new Point() { X = splits[3].ToInt(), Y = splits[4].ToInt() }
                });
            }
        }

        [Description("In row 2000000, how many positions cannot contain a beacon?")]
        public override string SolvePart1() {
            ValueRangeList emptyPositions = new();
            HashSet<int> beacons = new();
            for (int i = 0; i < sensors.Count; i++) {
                Sensor sensor = sensors[i];
                sensor.AddEmptyPositions(2000000, emptyPositions, beacons);
            }
            return $"{emptyPositions.Range() - beacons.Count}";
        }

        [Description("Find the only possible position for the distress beacon. What is its tuning frequency?")]
        public override string SolvePart2() {
            ValueRangeList emptyPositions = new();
            for (int y = 4000000; y >= 0; y--) {
                for (int i = 0; i < sensors.Count; i++) {
                    Sensor sensor = sensors[i];
                    sensor.AddEmptyPositions(y, emptyPositions);
                }

                if (emptyPositions.Values.Count > 1) {
                    return $"{(long)(emptyPositions.Values[0].End + 1) * 4000000 + y}";
                }
                emptyPositions.Clear();
            }
            return string.Empty;
        }

        private class Sensor {
            public Point Position;
            public Point Beacon;

            public void AddEmptyPositions(int row, ValueRangeList emptyPositions, HashSet<int> beacons = null) {
                int distance = Math.Abs(Position.X - Beacon.X) + Math.Abs(Position.Y - Beacon.Y);
                bool isInRange = false;
                if (Position.Y <= row && Position.Y + distance >= row) {
                    isInRange = true;
                } else if (Position.Y >= row && Position.Y - distance <= row) {
                    isInRange = true;
                }
                if (!isInRange) { return; }

                distance -= Math.Abs(row - Position.Y);
                int minX = Position.X - distance;
                int maxX = Position.X + distance;
                if (beacons != null && Beacon.Y == row && Beacon.X >= minX && Beacon.X <= maxX) {
                    beacons.Add(Beacon.X);
                }
                emptyPositions.AddRange(new ValueRange() { Start = minX, End = maxX });
            }
        }
        private class ValueRange : IComparable<ValueRange> {
            public int Start;
            public int End;
            public int Range { get { return End - Start + 1; } }

            public int CompareTo(ValueRange other) {
                return Start.CompareTo(other.Start);
            }
            public bool Connects(ValueRange other) {
                return Start - 1 <= other.End && End + 1 >= other.Start;
            }
            public void Expand(ValueRange other) {
                Start = Start < other.Start ? Start : other.Start;
                End = End > other.End ? End : other.End;
            }
            public override string ToString() {
                return $"{Start}-{End}";
            }
        }
        private class ValueRangeList {
            public readonly List<ValueRange> Values = new List<ValueRange>();
            public void Clear() {
                Values.Clear();
            }
            public void AddRange(ValueRange range) {
                if (Values.Count == 0) {
                    Values.Add(new ValueRange() { Start = range.Start, End = range.End });
                    return;
                }

                for (int i = 0; i < Values.Count; i++) {
                    ValueRange value = Values[i];
                    if (value.Connects(range)) {
                        value.Expand(range);
                        while (i + 1 < Values.Count && Values[i + 1].Start <= value.End) {
                            ValueRange next = Values[i + 1];
                            Values.RemoveAt(i + 1);
                            value.Expand(next);
                        }
                        return;
                    }
                }
                Values.Add(range);
                Values.Sort();
            }
            public int Range() {
                int range = 0;
                for (int i = 0; i < Values.Count; i++) {
                    range += Values[i].Range;
                }
                return range;
            }
            public override string ToString() {
                return $"Ranges={Values.Count} Range={Range()}";
            }
        }
    }
}