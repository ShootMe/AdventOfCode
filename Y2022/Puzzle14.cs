using AdventOfCode.Common;
using AdventOfCode.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
namespace AdventOfCode.Y2022 {
    [Description("Regolith Reservoir")]
    public class Puzzle14 : ASolver {
        private BlockType[] cave;
        private int width, height, minX, minY;

        public override void Setup() {
            string[] lines = Input.Split('\n');
            List<Line> rockFormations = new();

            minX = int.MaxValue;
            int maxX = int.MinValue;
            minY = 0;
            int maxY = int.MinValue;
            for (int i = 0; i < lines.Length; i++) {
                string line = lines[i];
                string[] points = line.Split(" -> ");

                Line formation = null;
                for (int j = 0; j < points.Length; j++) {
                    string[] point = points[j].Split(',');
                    if (formation == null) {
                        formation = new Line();
                        formation.StartPosition = new Point() { X = point[0].ToInt(), Y = point[1].ToInt() };
                    } else {
                        formation.EndPosition = new Point() { X = point[0].ToInt(), Y = point[1].ToInt() };
                        rockFormations.Add(formation);
                        Line nextFormation = new Line();
                        nextFormation.StartPosition = formation.EndPosition;
                        formation = nextFormation;
                    }

                    if (formation.StartPosition.X < minX) { minX = formation.StartPosition.X; }
                    if (formation.StartPosition.X > maxX) { maxX = formation.StartPosition.X; }
                    if (formation.StartPosition.Y < minY) { minY = formation.StartPosition.Y; }
                    if (formation.StartPosition.Y > maxY) { maxY = formation.StartPosition.Y; }
                }
            }

            minX = 500 - minX < maxY + 2 - minY ? 500 - (maxY + 3 - minY) : minX;
            maxX = maxX - 500 < maxY + 2 - minY ? 500 + maxY + 3 - minY : maxX;
            maxY += 2;

            width = maxX - minX + 1;
            height = maxY - minY + 1;
            cave = new BlockType[width * height];

            for (int i = 0; i < rockFormations.Count; i++) {
                Line formation = rockFormations[i];
                if (formation.StartPosition.X > formation.EndPosition.X) {
                    Point temp = formation.EndPosition;
                    formation.EndPosition = formation.StartPosition;
                    formation.StartPosition = temp;
                } else if (formation.StartPosition.Y > formation.EndPosition.Y) {
                    Point temp = formation.EndPosition;
                    formation.EndPosition = formation.StartPosition;
                    formation.StartPosition = temp;
                }
            }

            AddRock(rockFormations);
        }

        [Description("How many units of sand come to rest before sand starts flowing into the abyss below?")]
        public override string SolvePart1() {
            return $"{FillSand()}";
        }

        [Description("How many units of sand come to rest?")]
        public override string SolvePart2() {
            for (int i = 0; i < cave.Length; i++) {
                BlockType block = cave[i];
                if (block == BlockType.Sand) { cave[i] = BlockType.Empty; }
            }
            List<Line> lines = new();
            lines.Add(new Line() { StartPosition = new Point() { X = minX, Y = minY + height - 1 }, EndPosition = new Point() { X = minX + width - 1, Y = minY + height - 1 } });
            AddRock(lines);
            return $"{FillSand()}";
        }

        private void AddRock(List<Line> lines) {
            for (int i = 0; i < lines.Count; i++) {
                Line line = lines[i];
                int index = line.StartPosition.X - minX + (line.StartPosition.Y - minY) * width;
                if (line.StartPosition.X == line.EndPosition.X) {
                    for (int j = line.StartPosition.Y; j <= line.EndPosition.Y; j++) {
                        cave[index] = BlockType.Rock;
                        index += width;
                    }
                } else {
                    for (int j = line.StartPosition.X; j <= line.EndPosition.X; j++) {
                        cave[index++] = BlockType.Rock;
                    }
                }
            }
        }
        private int FillSand() {
            int totalSand = 0;
        StartLoop:
            int position = 500 - minX;
            if (cave[position] == BlockType.Sand) {
                //Display(position);
                return totalSand;
            }

            while (position + width < cave.Length) {
                position += width;

                switch (cave[position]) {
                    case BlockType.Empty: continue;
                    case BlockType.Rock:
                    case BlockType.Sand:
                        if (cave[position - 1] == BlockType.Empty) {
                            position--;
                        } else if (cave[position + 1] == BlockType.Empty) {
                            position++;
                        } else {
                            cave[position - width] = BlockType.Sand;
                            totalSand++;
                            //Display(position - width);
                            goto StartLoop;
                        }
                        break;
                }
            }
            //Display(position);
            return totalSand;
        }
        private void Display(int position) {
            Console.WriteLine();
            for (int i = 0; i < cave.Length; i++) {
                BlockType block = cave[i];
                if (i == position) {
                    Console.Write('*');
                } else {
                    Console.Write(block == BlockType.Sand ? 'O' : block == BlockType.Rock ? '#' : '.');
                }
                if (((i + 1) % width) == 0) {
                    Console.WriteLine();
                }
            }
        }
        private enum BlockType : byte {
            Empty,
            Sand,
            Rock
        }
    }
}