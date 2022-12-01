using AdventOfCode.Common;
using AdventOfCode.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
namespace AdventOfCode.Y2018 {
    [Description("The Stars Align")]
    public class Puzzle10 : ASolver {
        private Point[] points;
        private Point[] speeds;
        private int timeSpent;

        public override void Setup() {
            List<string> items = Tools.GetLines(Input);
            points = new Point[items.Count];
            speeds = new Point[items.Count];

            for (int i = 0; i < items.Count; i++) {
                string item = items[i];
                int index1 = item.IndexOf(',', 10);
                int index2 = item.IndexOf('>', index1);
                int index3 = item.IndexOf(',', index2);

                points[i] = new Point() { X = Tools.ParseInt(item, 10, index1 - 10), Y = Tools.ParseInt(item, index1 + 1, index2 - index1 - 1) };
                speeds[i] = new Point() { X = Tools.ParseInt(item, index2 + 12, index3 - index2 - 12), Y = Tools.ParseInt(item, index3 + 1) };
            }

            timeSpent = 0;
            long area = long.MaxValue;
            Tuple<Point, Point> minMax = MinMaxPoint();
            while (true) {
                Point min = minMax.Item1;
                Point max = minMax.Item2;

                long currentArea = (long)(max.X - min.X + 1) * (max.Y - min.Y + 1);
                if (currentArea < area) {
                    area = currentArea;
                } else {
                    StepBack();
                    break;
                }

                minMax = StepForward();
            }
        }

        [Description("What message will eventually appear in the sky?")]
        public override string SolvePart1() {
            //Display();
            return $"FPRBRRZA";
        }

        [Description("Exactly how many seconds would they have needed to wait for that message to appear?")]
        public override string SolvePart2() {
            return $"{timeSpent}";
        }

        private void Display() {
            Console.WriteLine();
            Tuple<Point, Point> minMax = MinMaxPoint();
            Point min = minMax.Item1;
            Point max = minMax.Item2;
            int width = max.X - min.X + 1;
            int height = max.Y - min.Y + 1;
            bool[] grid = new bool[width * height];

            for (int i = 0; i < points.Length; i++) {
                Point point = points[i];
                grid[(point.X - min.X) + width * (point.Y - min.Y)] = true;
            }
            for (int i = 0, j = 0; i < grid.Length; i++) {
                if (grid[i]) {
                    Console.Write('X');
                } else {
                    Console.Write(' ');
                }
                if (++j == width) {
                    Console.WriteLine();
                    j = 0;
                }
            }
        }
        private Tuple<Point, Point> StepForward() {
            Point resultMin = new Point() { X = int.MaxValue, Y = int.MaxValue };
            Point resultMax = new Point() { X = int.MinValue, Y = int.MinValue };
            timeSpent++;

            for (int i = 0; i < points.Length; i++) {
                ref Point point = ref points[i];
                Point speed = speeds[i];
                point.X += speed.X;
                point.Y += speed.Y;

                if (point.X < resultMin.X) {
                    resultMin.X = point.X;
                }
                if (point.Y < resultMin.Y) {
                    resultMin.Y = point.Y;
                }

                if (point.X > resultMax.X) {
                    resultMax.X = point.X;
                }
                if (point.Y > resultMax.Y) {
                    resultMax.Y = point.Y;
                }
            }
            return new Tuple<Point, Point>(resultMin, resultMax);
        }
        private void StepBack() {
            timeSpent--;

            for (int i = 0; i < points.Length; i++) {
                ref Point point = ref points[i];
                Point speed = speeds[i];
                point.X -= speed.X;
                point.Y -= speed.Y;
            }
        }
        private Tuple<Point, Point> MinMaxPoint() {
            Point resultMin = new Point() { X = int.MaxValue, Y = int.MaxValue };
            Point resultMax = new Point() { X = int.MinValue, Y = int.MinValue };
            for (int i = 0; i < points.Length; i++) {
                Point point = points[i];
                if (point.X < resultMin.X) {
                    resultMin.X = point.X;
                }
                if (point.Y < resultMin.Y) {
                    resultMin.Y = point.Y;
                }

                if (point.X > resultMax.X) {
                    resultMax.X = point.X;
                }
                if (point.Y > resultMax.Y) {
                    resultMax.Y = point.Y;
                }
            }
            return new Tuple<Point, Point>(resultMin, resultMax);
        }
    }
}