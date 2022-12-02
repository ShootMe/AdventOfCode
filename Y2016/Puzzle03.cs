using AdventOfCode.Common;
using AdventOfCode.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
namespace AdventOfCode.Y2016 {
    [Description("Squares With Three Sides")]
    public class Puzzle03 : ASolver {
        private List<string> items;

        public override void Setup() {
            items = Input.Lines();
        }

        [Description("How many of the listed triangles are possible?")]
        public override string SolvePart1() {
            int count = 0;
            for (int i = 0; i < items.Count; i++) {
                if (((Triangle)items[i]).IsValid()) {
                    count++;
                }
            }

            return $"{count}";
        }

        [Description("How many of the listed triangles are possible?")]
        public override string SolvePart2() {
            StringBuilder triangle = new StringBuilder();
            int sideCount = 0;
            int count = 0;
            for (int c = 0; c < 3; c++) {
                for (int i = 0; i < items.Count; i++) {
                    string[] sides = items[i].Split(' ', StringSplitOptions.RemoveEmptyEntries);
                    triangle.Append($"{sides[c]} ");
                    sideCount++;
                    if (sideCount == 3) {
                        if (((Triangle)triangle.ToString()).IsValid()) {
                            count++;
                        }
                        sideCount = 0;
                        triangle.Clear();
                    }
                }
            }

            return $"{count}";
        }

        private class Triangle {
            public int Side1, Side2, Side3;

            public static implicit operator Triangle(string item) {
                string[] sides = item.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                return new Triangle() {
                    Side1 = Tools.ParseInt(sides[0]),
                    Side2 = Tools.ParseInt(sides[1]),
                    Side3 = Tools.ParseInt(sides[2])
                };
            }

            public bool IsValid() {
                return Side1 + Side2 > Side3 && Side1 + Side3 > Side2 && Side2 + Side3 > Side1;
            }
        }
    }
}