using AdventOfCode.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
namespace AdventOfCode.Y2015 {
    public class Puzzle02 : ASolver {
        private Present[] boxes;
        public Puzzle02(string input) : base(input) { Name = "I Was Told There Would Be No Math"; }

        public override void Setup() {
            List<string> lines = Tools.GetLines(Input);
            boxes = new Present[lines.Count];
            for (int i = 0; i < boxes.Length; i++) {
                boxes[i] = lines[i];
            }
        }

        [Description("How many total square feet of wrapping paper should they order?")]
        public override string SolvePart1() {
            int total = 0;
            for (int i = 0; i < boxes.Length; i++) {
                total += boxes[i].AreaNeeded();
            }
            return $"{total}";
        }

        [Description("How many total feet of ribbon should they order?")]
        public override string SolvePart2() {
            int total = 0;
            for (int i = 0; i < boxes.Length; i++) {
                total += boxes[i].RibbonNeeded();
            }
            return $"{total}";
        }

        private class Present {
            public int Length, Width, Height;

            public static implicit operator Present(string item) {
                int index1 = item.IndexOf("x", StringComparison.OrdinalIgnoreCase);
                int index2 = item.IndexOf("x", index1 + 1, StringComparison.OrdinalIgnoreCase);
                return new Present() {
                    Length = Tools.ParseInt(item, 0, index1),
                    Width = Tools.ParseInt(item, index1 + 1, index2 - index1 - 1),
                    Height = Tools.ParseInt(item, index2 + 1)
                };
            }

            public int AreaNeeded() {
                int lw = Length * Width;
                int lh = Length * Height;
                int wh = Width * Height;
                int minSide = Math.Min(lw, Math.Min(lh, wh));
                return 2 * lw + 2 * lh + 2 * wh + minSide;
            }
            public int RibbonNeeded() {
                int maxSide = Math.Max(Length, Math.Max(Width, Height));
                int ribbon = Length * 2 + Width * 2 + Height * 2 - maxSide * 2;
                ribbon += Length * Width * Height;

                return ribbon;
            }
        }
    }
}