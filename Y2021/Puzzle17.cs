using AdventOfCode.Common;
using AdventOfCode.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
namespace AdventOfCode.Y2021 {
    [Description("Trick Shot")]
    public class Puzzle17 : ASolver {
        private int x1, x2, y1, y2;

        public override void Setup() {
            string[] splits = Input.SplitOn("x=", "..", ", y=", "..");
            x1 = splits[1].ToInt();
            x2 = splits[2].ToInt();

            y1 = splits[3].ToInt();
            y2 = splits[4].ToInt();
        }

        [Description("What is the highest y position it reaches on this trajectory?")]
        public override string SolvePart1() {
            int maxY = -y1 - 1;
            return $"{maxY * (maxY + 1) / 2}";
        }

        [Description("How many distinct velocity values cause the probe to be within the target area?")]
        public override string SolvePart2() {
            int endY = -y1 - 1;

            Dictionary<int, ValueRange> validYRanges = new Dictionary<int, ValueRange>();
            int yStep = 0;
            int ys = y1;
            while (ys <= endY) {
                int yPos = (2 * ys - yStep) * (yStep + 1) / 2;
                yStep++;

                if (yPos < y1) {
                    int diff = (y1 - yPos) / yStep;
                    ys += diff;
                    yPos += diff * yStep;
                    if (yPos < y1) {
                        yPos += yStep;
                        ys++;
                    }
                }
                if (yPos > y2) { continue; }

                int amount = (y2 - yPos) / yStep;
                ValueRange range = new ValueRange() { Start = ys, End = ys + amount };
                validYRanges[yStep] = range;
            }

            int startX = (int)Math.Sqrt(x1 * 2);
            int count = 0;
            ValueRangeList validRanges = new ValueRangeList();
            for (int x = startX; x <= x2; x++) {
                int step = 1;
                int xPos = x;
                int xVel = x - 1;

                while (xPos <= x2 && (xVel != 0 || step < yStep)) {
                    if (xPos >= x1 && validYRanges.TryGetValue(step, out ValueRange yRange)) {
                        validRanges.AddRange(yRange);
                    }

                    step++;
                    xPos += xVel;
                    xVel -= xVel > 0 ? 1 : 0;
                }

                count += validRanges.Range();
                validRanges.Clear();
            }

            return $"{count}";
        }
        private class ValueRange {
            public int Start;
            public int End;
            public int Range { get { return End - Start + 1; } }
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
            private readonly List<ValueRange> Values = new List<ValueRange>();
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
                        return;
                    }
                }
                Values.Add(range);
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