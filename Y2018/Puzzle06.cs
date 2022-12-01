using AdventOfCode.Common;
using AdventOfCode.Core;
using System.Collections.Generic;
using System.ComponentModel;
namespace AdventOfCode.Y2018 {
    [Description("Chronal Coordinates")]
    public class Puzzle06 : ASolver {
        private Point[] points;
        private int minX, maxX, minY, maxY;
        private int width, height;

        public override void Setup() {
            List<string> items = Tools.GetLines(Input);
            points = new Point[items.Count];
            for (int i = 0; i < points.Length; i++) {
                string item = items[i];
                int comma = item.IndexOf(',');
                points[i] = new Point() {
                    X = Tools.ParseInt(item.Substring(0, comma)),
                    Y = Tools.ParseInt(item.Substring(comma + 1))
                };
            }

            maxX = int.MinValue;
            minX = int.MaxValue;
            maxY = int.MinValue;
            minY = int.MaxValue;
            for (int i = 0; i < points.Length; i++) {
                Point point = points[i];
                if (point.X < minX) {
                    minX = point.X;
                }
                if (point.X > maxX) {
                    maxX = point.X;
                }
                if (point.Y < minY) {
                    minY = point.Y;
                }
                if (point.Y > maxY) {
                    maxY = point.Y;
                }
            }

            width = maxX - minX;
            height = maxY - minY;
        }

        [Description("What is the size of the largest area that isn't infinite?")]
        public override string SolvePart1() {
            int[] distances = new int[width * height];
            HashSet<int> edgePoints = new HashSet<int>();

            for (int i = 0; i < distances.Length; i++) {
                Point position = new Point() { X = i % width + minX, Y = i / width + minY };

                int minDist = int.MaxValue;
                int bestIndex = points.Length;
                for (int j = 0; j < points.Length; j++) {
                    Point point = points[j];
                    int dist = point.Distance(position);
                    if (dist < minDist) {
                        minDist = dist;
                        bestIndex = j;
                    } else if (dist == minDist) {
                        bestIndex = points.Length;
                    }
                }

                distances[i] = bestIndex;

                if (position.X == minX || position.Y == minY || position.X == minX + width - 1 || position.Y == minY + height - 1) {
                    edgePoints.Add(bestIndex);
                }
            }

            int maxArea = 0;
            for (int j = 0; j < points.Length; j++) {
                if (edgePoints.Contains(j)) { continue; }

                int area = 0;
                for (int i = 0; i < distances.Length; i++) {
                    int index = distances[i];
                    if (index == j) {
                        area++;
                    }
                }

                if (area > maxArea) {
                    maxArea = area;
                }
            }
            return $"{maxArea}";
        }

        [Description("What is the size of the region which has a total distance to all, less than 10000?")]
        public override string SolvePart2() {
            int totalArea = width * height;
            int area = 0;

            for (int i = 0; i < totalArea; i++) {
                Point position = new Point() { X = i % width + minX, Y = i / width + minY };

                int distance = 0;
                for (int j = 0; j < points.Length; j++) {
                    Point point = points[j];
                    distance += point.Distance(position);
                }

                if (distance < 10000) {
                    area++;
                }
            }

            return $"{area}";
        }
    }
}