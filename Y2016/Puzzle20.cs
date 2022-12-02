using AdventOfCode.Common;
using AdventOfCode.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
namespace AdventOfCode.Y2016 {
    [Description("Firewall Rules")]
    public class Puzzle20 : ASolver {
        private List<BlockedRange> blocked = new();

        public override void Setup() {
            foreach (string item in Input.Split('\n')) {
                int index = item.IndexOf('-');
                blocked.Add(new BlockedRange() { Start = item.Substring(0, index).ToUInt(), End = item.Substring(index + 1).ToUInt() });
            }
            blocked.Sort();
        }

        [Description("What is the lowest-valued IP that is not blocked?")]
        public override string SolvePart1() {
            uint lastEnd = 0;
            uint firstUnused = 0;
            for (int i = 0; i < blocked.Count; i++) {
                BlockedRange range = blocked[i];
                if (range.End < lastEnd) {
                    blocked.RemoveAt(i);
                    i--;
                    continue;
                } else if (range.Start <= lastEnd) {
                    range.Start = lastEnd + 1;
                } else if (firstUnused == 0 && range.Start > lastEnd + 1) {
                    firstUnused = lastEnd + 1;
                }
                lastEnd = range.End;
            }
            return $"{firstUnused}";
        }

        [Description("How many IPs are allowed by the blacklist?")]
        public override string SolvePart2() {
            BlockedRange last = blocked[0];
            uint totalAllowed = 0;
            for (int i = 1; i < blocked.Count; i++) {
                BlockedRange range = blocked[i];
                totalAllowed += range - last;
                last = range;
            }
            totalAllowed += uint.MaxValue - last.End;
            return $"{totalAllowed}";
        }

        private class BlockedRange : IComparable<BlockedRange> {
            public uint Start;
            public uint End;

            public int CompareTo(BlockedRange other) {
                return Start.CompareTo(other.Start);
            }
            public override string ToString() {
                return $"[{Start,11}] - [{End,11}]";
            }
            public static uint operator -(BlockedRange left, BlockedRange right) {
                return left.Start - right.End - 1;
            }
        }
    }
}