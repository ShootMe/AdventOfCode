using AdventOfCode.Common;
using AdventOfCode.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
namespace AdventOfCode.Y2021 {
    [Description("Hydrothermal Venture")]
    public class Puzzle05 : ASolver {
        private List<Line> lines;
        private int[,] grid;
        private int width, height, minX, maxX, minY, maxY;

        public override void Setup() {
            List<string> items = Tools.GetLines(Input);
            lines = new List<Line>();

            minX = int.MaxValue; maxX = int.MinValue; minY = int.MaxValue; maxY = int.MinValue;
            for (int i = 0; i < items.Count; i++) {
                string item = items[i];
                string[] splits = Tools.SplitOn(item, ",", " -> ", ",");
                int x1 = Tools.ParseInt(splits[0]);
                int y1 = Tools.ParseInt(splits[1]);

                int x2 = Tools.ParseInt(splits[2]);
                int y2 = Tools.ParseInt(splits[3]);

                if (x1 < minX) { minX = x1; }
                if (x2 < minX) { minX = x2; }
                if (x1 > maxX) { maxX = x1; }
                if (x2 > maxX) { maxX = x2; }
                if (y1 < minY) { minY = y1; }
                if (y2 < minY) { minY = y2; }
                if (y1 > maxY) { maxY = y1; }
                if (y2 > maxY) { maxY = y2; }

                lines.Add(new Line() { StartPosition = new Point() { X = x1, Y = y1 }, EndPosition = new Point() { X = x2, Y = y2 } });
            }

            width = maxX - minX + 1;
            height = maxY - minY + 1;
            grid = new int[height, width];
        }

        [Description("At how many points do at least two lines overlap?")]
        public override string SolvePart1() {
            for (int i = 0; i < lines.Count; i++) {
                Line line = lines[i];
                if (!line.IsFlat()) { continue; }

                MarkGrid(line);
            }

            //Display();
            return $"{CountOverlaps()}";
        }

        [Description("At how many points do at least two lines overlap?")]
        public override string SolvePart2() {
            for (int i = 0; i < lines.Count; i++) {
                Line line = lines[i];
                if (line.IsFlat()) { continue; }

                MarkGrid(line);
            }

            //Display();
            return $"{CountOverlaps()}";
        }
        private int CountOverlaps() {
            int count = 0;
            for (int i = 0; i < height; i++) {
                for (int j = 0; j < width; j++) {
                    if (grid[i, j] > 1) {
                        count++;
                    }
                }
            }
            return count;
        }
        private void MarkGrid(Line line) {
            int x = line.StartPosition.X - minX;
            int y = line.StartPosition.Y - minY;
            int xEnd = line.EndPosition.X - minX;
            int yEnd = line.EndPosition.Y - minY;
            int xInc = line.StartPosition.X < line.EndPosition.X ? 1 : line.StartPosition.X == line.EndPosition.X ? 0 : -1;
            int yInc = line.StartPosition.Y < line.EndPosition.Y ? 1 : line.StartPosition.Y == line.EndPosition.Y ? 0 : -1;
            while (x != xEnd || y != yEnd) {
                grid[y, x]++;
                x += xInc;
                y += yInc;
            }
            grid[y, x]++;
        }
        private void Display() {
            Console.WriteLine();
            for (int i = 0; i < height; i++) {
                for (int j = 0; j < width; j++) {
                    Console.Write($"{grid[i, j]:00} ");
                }
                Console.WriteLine();
            }
        }
    }
}