using AdventOfCode.Common;
using AdventOfCode.Core;
using System.Collections.Generic;
using System.ComponentModel;
namespace AdventOfCode.Y2021 {
    [Description("Trench Map")]
    public class Puzzle20 : ASolver {
        private byte[] algorithm;
        private BitGrid image, output;

        public override void Setup() {
            List<string> items = Input.Lines();

            algorithm = new byte[512];
            string algo = items[0];
            for (int i = 0; i < algo.Length; i++) {
                algorithm[i] = (byte)(algo[i] == '#' ? 1 : 0);
            }

            image = new BitGrid(items[2].Length + 110, items.Count + 108);
            output = new BitGrid(image.Width, image.Height);
            for (int i = 2; i < items.Count; i++) {
                string line = items[i];
                for (int j = 0; j < line.Length; j++) {
                    if (line[j] == '#') {
                        image[j + 55, i + 53] = 1;
                    }
                }
            }
        }

        [Description("How many pixels are lit in the resulting image?")]
        public override string SolvePart1() {
            Step(algorithm[0] == 0 ? (byte)0 : algorithm[511]);
            Step(algorithm[0]);
            return $"{image.Count()}";
        }

        [Description("How many pixels are lit in the resulting image?")]
        public override string SolvePart2() {
            for (int i = 2; i < 50; i += 2) {
                Step(algorithm[0] == 0 ? (byte)0 : algorithm[511]);
                Step(algorithm[0]);
            }
            return $"{image.Count()}";
        }

        private void Step(byte spaceValue) {
            int height = output.Height - 1;
            int width = output.Width - 1;

            image.SetRow(0, spaceValue);
            image.SetRow(height, spaceValue);
            image.SetColumn(0, spaceValue);
            image.SetColumn(width, spaceValue);

            for (int i = 1; i < height; i++) {
                for (int j = 1; j < width; j++) {
                    uint index = (image.GetBits(j - 1, i - 1, 3) << 6) | (image.GetBits(j - 1, i, 3) << 3) | image.GetBits(j - 1, i + 1, 3);
                    output[j, i] = algorithm[index];
                }
            }

            BitGrid temp = image;
            image = output;
            output = temp;
        }
    }
}