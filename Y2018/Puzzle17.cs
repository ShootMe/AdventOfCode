using AdventOfCode.Common;
using AdventOfCode.Core;
using System.Collections.Generic;
using System.ComponentModel;
namespace AdventOfCode.Y2018 {
    public class Puzzle17 : ASolver {
        private int[] reservoir;
        private int width, height, minX, minY, maxWater;
        public Puzzle17(string input) : base(input) { Name = "Reservoir Research"; }

        public override void Setup() {
            List<string> items = Tools.GetLines(Input);
            List<Line> lines = new List<Line>();
            minX = int.MaxValue;
            int maxX = int.MinValue;
            minY = int.MaxValue;
            int maxY = int.MinValue;
            for (int i = 0; i < items.Count; i++) {
                string item = items[i];

                int index1 = item.IndexOf(',');
                int value1 = Tools.ParseInt(item, 2, index1 - 2);
                int index2 = item.IndexOf('.');
                int value2 = Tools.ParseInt(item, index1 + 4, index2 - index1 - 4);
                int value3 = Tools.ParseInt(item, index2 + 2);

                Line line = new Line();
                if (item[0] == 'x') {
                    line.StartPosition = new Point() { X = value1, Y = value2 };
                    line.EndPosition = new Point() { X = value1, Y = value3 };
                    if (value1 < minX) { minX = value1; }
                    if (value1 > maxX) { maxX = value1; }
                    if (value2 < minY) { minY = value2; }
                    if (value3 > maxY) { maxY = value3; }
                } else {
                    line.StartPosition = new Point() { X = value2, Y = value1 };
                    line.EndPosition = new Point() { X = value3, Y = value1 };
                    if (value1 < minY) { minY = value1; }
                    if (value1 > maxY) { maxY = value1; }
                    if (value2 < minX) { minX = value2; }
                    if (value3 > maxX) { maxX = value3; }
                }

                lines.Add(line);
            }

            minX -= 2;
            maxX += 2;
            minY--;
            width = maxX - minX + 1;
            height = maxY - minY + 1;
            reservoir = new int[width * height];
            maxWater = 1;
            for (int i = 0; i < lines.Count; i++) {
                Line line = lines[i];
                int index = line.StartPosition.X - minX + (line.StartPosition.Y - minY) * width;
                if (line.StartPosition.X == line.EndPosition.X) {
                    for (int j = line.StartPosition.Y; j <= line.EndPosition.Y; j++) {
                        reservoir[index] = 1;
                        index += width;
                    }
                } else {
                    for (int j = line.StartPosition.X; j <= line.EndPosition.X; j++) {
                        reservoir[index++] = 1;
                    }
                }
            }
        }

        [Description("How many tiles can the water reach within the range of y values in your scan?")]
        public override string SolvePart1() {
            int lastCount = 0;
            while (true) {
                int count = Step();
                if (count <= lastCount) {
                    break;
                }
                //Display();
                lastCount = count;
            }
            return $"{lastCount}";
        }

        [Description("How many water tiles remain after the spring stops producing water and all has drained?")]
        public override string SolvePart2() {
            int lastCount = int.MaxValue;
            while (true) {
                int count = Step(false);
                if (count >= lastCount) {
                    break;
                }
                //Display();
                lastCount = count;
            }
            //Display(true);
            return $"{lastCount}";
        }

        private int GetLeft(int index, int x) {
            int left = x;
            if (index >= reservoir.Length || index < 0) {
                return -1;
            }

            while (left >= 0 && reservoir[index] >= 2) {
                left--;
                index--;
            }
            return left >= 0 ? reservoir[index] : -1;
        }
        private int GetRight(int index, int x) {
            int right = x;
            if (index >= reservoir.Length || index < 0) {
                return -1;
            }

            while (right < width && reservoir[index] >= 2) {
                right++;
                index++;
            }
            return right < width ? reservoir[index] : -1;
        }
        private int Step(bool infinite = true) {
            reservoir[500 - minX] = infinite ? 2 : 0;

            int count = 0;

            for (int y = 0; y < maxWater; y++) {
                int i = y * width;

                int last = 0;
                for (int x = 0; x < width; x++, i++) {
                    ref int value = ref reservoir[i];
                    int down = y + 1 < height ? reservoir[i + width] : 0;

                    if (value >= 2 && (down == 0 || (!infinite && (GetLeft(i, x) != 1 || GetRight(i, x) != 1)))) {
                        if (!infinite) {
                            value = 0;
                        } else if (y + 1 < height) {
                            reservoir[i + width] = 2;
                            if (y + 2 > maxWater) {
                                maxWater = y + 2;
                            }
                        }
                    } else {
                        int left = GetLeft(i + width, x);
                        int right = GetRight(i + width, x);
                        if (left == 1 && right == 1 && value >= 2) {
                            if (x + 1 < width && reservoir[i + 1] == 0) {
                                reservoir[i + 1] = last == 1 || last == 3 || value == 3 ? 3 : value;
                            }
                        }
                    }

                    last = value;
                }

                i--;
                last = 0;
                for (int x = width - 1; x >= 0; x--, i--) {
                    ref int value = ref reservoir[i];
                    int left = GetLeft(i + width, x);
                    int right = GetRight(i + width, x);

                    if (left == 1 && right == 1 && (value == 2 || value == 4)) {
                        if (x - 1 >= 0 && reservoir[i - 1] == 0) {
                            reservoir[i - 1] = last == 1 || last == 4 || value == 4 ? 4 : value;
                        }
                    }

                    last = value;
                }
            }

            for (int y = 1; y < height; y++) {
                int i = y * width;
                for (int x = 0; x < width; x++, i++) {
                    if (reservoir[i] >= 2) { count++; }
                }
            }
            return count;
        }

        //private GifEncoder gif;
        //private Bitmap image;
        //private void Display(bool final = false) {
        //    if (gif == null) {
        //        gif = new GifEncoder();
        //        gif.Start("out.gif");
        //        gif.Delay = 2;
        //        gif.Repeat = 0;
        //        gif.ColorBits = 8;
        //        gif.Disposal = 0;
        //        gif.Quality = 1;
        //        gif.UseLocalColorTable = false;
        //        image = new Bitmap(width, height);
        //        gif.AddColor(System.Drawing.Color.Blue);
        //    }

        //    int x = 0;
        //    int y = 0;
        //    for (int i = 0; y < height; i++) {
        //        int value = reservoir[i];
        //        switch (value) {
        //            case 1: image[i] = System.Drawing.Color.Black; break;
        //            case 2: image[i] = System.Drawing.Color.Blue; break;
        //            case 3: image[i] = System.Drawing.Color.Blue; break;
        //            case 4: image[i] = System.Drawing.Color.Blue; break;
        //            default: image[i] = System.Drawing.Color.White; break;
        //        }

        //        x++;
        //        if (x == width) {
        //            x = 0;
        //            y++;
        //        }
        //    }

        //    gif.AddFrame(image);
        //    if (final) {
        //        gif.Finish();
        //    }
        //}
    }
}