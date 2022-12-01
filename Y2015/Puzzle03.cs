using AdventOfCode.Common;
using AdventOfCode.Core;
using System.Collections.Generic;
using System.ComponentModel;
namespace AdventOfCode.Y2015 {
    [Description("Perfectly Spherical Houses in a Vacuum")]
    public class Puzzle03 : ASolver {
        [Description("How many houses receive at least one present?")]
        public override string SolvePart1() {
            Dictionary<Point, int> gifted = new Dictionary<Point, int>();

            Point current = default;
            gifted.Add(current, 1);

            for (int i = 0; i < Input.Length; i++) {
                char dir = Input[i];
                switch (dir) {
                    case '<': current.X--; break;
                    case '^': current.Y--; break;
                    case '>': current.X++; break;
                    case 'v': current.Y++; break;
                }

                if (gifted.ContainsKey(current)) {
                    gifted[current]++;
                } else {
                    gifted.Add(current, 1);
                }
            }

            return $"{gifted.Count}";
        }

        [Description("How many houses receive at least one present?")]
        public override string SolvePart2() {
            Dictionary<Point, int> gifted = new Dictionary<Point, int>();

            Point santa = default;
            Point roboSanta = default;
            gifted.Add(santa, 2);

            for (int i = 0; i < Input.Length; i++) {
                char dir = Input[i];
                Point current = (i & 1) == 0 ? santa : roboSanta;
                switch (dir) {
                    case '<': current.X--; break;
                    case '^': current.Y--; break;
                    case '>': current.X++; break;
                    case 'v': current.Y++; break;
                }
                if ((i & 1) == 0) {
                    santa = current;
                } else {
                    roboSanta = current;
                }

                if (gifted.ContainsKey(current)) {
                    gifted[current]++;
                } else {
                    gifted.Add(current, 1);
                }
            }

            return $"{gifted.Count}";
        }
    }
}