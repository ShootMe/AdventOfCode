using AdventOfCode.Common;
using AdventOfCode.Core;
using System.ComponentModel;
namespace AdventOfCode.Y2019 {
    [Description("Tractor Beam")]
    public class Puzzle19 : ASolver {
        private IntCode program;

        public override void Setup() {
            program = new IntCode(Input.ToLongs(','));
        }

        [Description("How many points are affected by the tractor beam in the 50x50 area?")]
        public override string SolvePart1() {
            int count = 0;
            for (int i = 0; i < 50; i++) {
                for (int j = 0; j < 50; j++) {
                    if (CheckLocation(j, i)) {
                        count++;
                    }
                }
            }
            return $"{count}";
        }

        [Description("Find a 100x100 sqaure, what value do you get if you do X * 10000 + Y?")]
        public override string SolvePart2() {
            program.Reset();

            int max = 25 * 65;
            for (int i = 17 * 65; i < max; i++) {
                int j = i * 3 / 5;
                bool isInBeam = false;

                while (j < max && program.Output == 0) {
                    isInBeam = CheckLocation(j++, i);
                }

                j--;
                while (isInBeam) {
                    bool rightCorner = CheckLocation(j + 99, i);
                    if (rightCorner && CheckLocation(j, i + 99) && CheckLocation(j + 99, i + 99)) {
                        return $"{j * 10000 + i}";
                    }
                    isInBeam = rightCorner && CheckLocation(++j, i);
                }
            }
            return string.Empty;
        }

        private bool CheckLocation(int x, int y) {
            program.Reset();
            program.WriteInput(x);
            program.Run(y);
            return program.Output == 1;
        }
    }
}