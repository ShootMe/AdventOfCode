using AdventOfCode.Common;
using AdventOfCode.Core;
using System.Collections.Generic;
using System.ComponentModel;
namespace AdventOfCode.Y2018 {
    [Description("Reservoir Research")]
    public class Puzzle17 : ASolver {
        private BlockType[] reservoir;
        private int width, height, minX, minY;

        public override void Setup() {
            List<string> items = Input.Lines();
            List<Line> lines = new List<Line>();
            minX = int.MaxValue;
            int maxX = int.MinValue;
            minY = int.MaxValue;
            int maxY = int.MinValue;
            for (int i = 0; i < items.Count; i++) {
                string item = items[i];

                int index1 = item.IndexOf(',');
                int value1 = item.Substring(2, index1 - 2).ToInt();
                int index2 = item.IndexOf('.');
                int value2 = item.Substring(index1 + 4, index2 - index1 - 4).ToInt();
                int value3 = item.Substring(index2 + 2).ToInt();

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
            reservoir = new BlockType[width * height];

            for (int i = 0; i < lines.Count; i++) {
                Line line = lines[i];
                int index = line.StartPosition.X - minX + (line.StartPosition.Y - minY) * width;
                if (line.StartPosition.X == line.EndPosition.X) {
                    for (int j = line.StartPosition.Y; j <= line.EndPosition.Y; j++) {
                        reservoir[index] = BlockType.Clay;
                        index += width;
                    }
                } else {
                    for (int j = line.StartPosition.X; j <= line.EndPosition.X; j++) {
                        reservoir[index++] = BlockType.Clay;
                    }
                }
            }
        }

        [Description("How many tiles can the water reach within the range of y values in your scan?")]
        public override string SolvePart1() {
            return $"{FillReservoir()}";
        }

        [Description("How many water tiles remain after the spring stops producing water and all has drained?")]
        public override string SolvePart2() {
            return $"{FillReservoir(true)}";
        }

        private int FillReservoir(bool stillWater = false) {
            Stack<(int, int)> points = new Stack<(int, int)>();
            points.Push((500 - minX, 0));

            while (points.Count > 0) {
                (int position, int state) = points.Pop();

                switch (state) {
                    case 0:
                        ref BlockType value = ref reservoir[position];
                        if (value != BlockType.Sand) { continue; }

                        value = BlockType.MovingWater;

                        int downPos = position + width;
                        if (downPos >= reservoir.Length) { continue; }

                        points.Push((position, 1));
                        points.Push((downPos, 0));
                        break;
                    case 1:
                        BlockType down = reservoir[position + width];
                        if (down == BlockType.Clay || down == BlockType.StillWater) {
                            (BlockType left, int leftMost) = CheckDirection(position, -1);
                            (BlockType right, int rightMost) = CheckDirection(position, 1);
                            if (left == BlockType.Clay && right == BlockType.Clay) {
                                FillBlocks(leftMost + 1, rightMost - 1, BlockType.StillWater);
                            } else if (rightMost == leftMost + 2 || FillBlocks(leftMost + 1, rightMost - 1, BlockType.MovingWater) > 0) {
                                if (left != BlockType.Clay) {
                                    points.Push((leftMost, 0));
                                }
                                if (right != BlockType.Clay) {
                                    points.Push((rightMost, 0));
                                }
                            }
                        }
                        break;
                }
            }

            int count = 0;
            for (int i = width; i < reservoir.Length; i++) {
                BlockType block = reservoir[i];
                if ((stillWater && block == BlockType.StillWater) || (!stillWater && block >= BlockType.StillWater)) {
                    count++;
                }
            }

            return count;
        }
        private enum BlockType : byte {
            Sand,
            Clay,
            StillWater,
            MovingWater
        }
        private int FillBlocks(int left, int right, BlockType type) {
            int filled = 0;
            for (int i = left; i <= right; i++) {
                ref BlockType block = ref reservoir[i];
                if (block != type) {
                    block = type;
                    filled++;
                }
            }
            return filled;
        }
        private (BlockType, int) CheckDirection(int index, int direction) {
            if (index >= reservoir.Length) {
                return (BlockType.MovingWater, index);
            }

            BlockType block;
            while (((block = reservoir[index]) >= BlockType.StillWater || block == BlockType.Sand) && reservoir[index + width] != BlockType.Sand) {
                index += direction;
            }
            return (reservoir[index], index);
        }
    }
}