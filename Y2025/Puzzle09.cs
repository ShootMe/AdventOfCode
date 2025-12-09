using System;
using System.Collections.Generic;
using System.ComponentModel;
using AdventOfCode.Common;
using AdventOfCode.Core;
namespace AdventOfCode.Y2025 {
    [Description("Movie Theater")]
    public class Puzzle09 : ASolver {
        private List<(int x, int y)> tiles = new();
        private List<int> distinctX = new();
        private List<int> distinctY = new();

        public override void Setup() {
            string[] lines = Input.Split('\n');
            HashSet<int> xs = new();
            HashSet<int> ys = new();
            foreach (string line in lines) {
                string[] coords = line.Split(',');
                int x = coords[0].ToInt();
                int y = coords[1].ToInt();
                tiles.Add((x, y));
                xs.Add(x); ys.Add(y);
            }
            distinctX.AddRange(xs); distinctY.AddRange(ys);
            distinctX.Sort(); distinctY.Sort();
        }

        [Description("What is the largest area of any rectangle you can make?")]
        public override string SolvePart1() {
            long maxArea = 0;
            for (int i = 0; i < tiles.Count; i++) {
                (int x1, int y1) = tiles[i];
                for (int j = i + 1; j < tiles.Count; j++) {
                    (int x2, int y2) = tiles[j];

                    long area = Area(x1, y1, x2, y2);
                    if (area > maxArea) { maxArea = area; }
                }
            }
            return $"{maxArea}";
        }

        [Description("What is the largest area of any rectangle you can make using only red and green tiles?")]
        public override string SolvePart2() {
            long maxArea = 0;
            for (int i = 0; i < tiles.Count; i++) {
                (int x1, int y1) = tiles[i];
                for (int j = i + 1; j < tiles.Count; j++) {
                    (int x2, int y2) = tiles[j];

                    if (x1 == x2 || y1 == y2 || IsValidRect(x1, y1, x2, y2)) {
                        long area = Area(x1, y1, x2, y2);
                        if (area > maxArea) { maxArea = area; }
                    }
                }
            }
            return $"{maxArea}";
        }

        private long Area(int x1, int y1, int x2, int y2) {
            return (long)(Math.Abs(x1 - x2) + 1) * (Math.Abs(y1 - y2) + 1);
        }
        private bool IsValidRect(int x1, int y1, int x2, int y2) {
            int minX = x1;
            int maxX = x2;
            if (minX > maxX) { minX = x2; maxX = x1; }
            int minY = y1;
            int maxY = y2;
            if (minY > maxY) { minY = y2; maxY = y1; }
            for (int i = 0; i < distinctX.Count; i++) {
                int x = distinctX[i];
                if (x < minX) { continue; }
                if (x > maxX) { break; }
                if (!IsValid(x, minY) || !IsValid(x, maxY)) { return false; }
            }
            for (int i = 0; i < distinctY.Count; i++) {
                int y = distinctY[i];
                if (y < minY) { continue; }
                if (y > maxY) { break; }
                if (!IsValid(minX, y) || !IsValid(maxX, y)) { return false; }
            }
            return true;
        }
        private bool IsValid(int x, int y) {
            bool result = false;
            (int x, int y) a = tiles[^1];
            for (int i = 0; i < tiles.Count; i++) {
                (int x, int y) b = tiles[i];
                if (x == b.x && y == b.y) { return true; }

                int minX = a.x;
                int maxX = b.x;
                if (minX > maxX) { minX = b.x; maxX = a.x; }
                if (a.y == b.y && y == a.y && minX <= x && x <= maxX) { return true; }

                int minY = a.y;
                int maxY = b.y;
                if (minY > maxY) { minY = b.y; maxY = a.y; }
                if (a.x == b.x && x == a.x && minY <= y && y <= maxY) { return true; }

                if (minY < y && y <= maxY) {
                    if (b.x + (long)(y - b.y) * (a.x - b.x) / (a.y - b.y) <= x) {
                        result = !result;
                    }
                }
                a = b;
            }
            return result;
        }
    }
}