using AdventOfCode.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
namespace AdventOfCode.Y2024 {
    [Description("Warehouse Woes")]
    public class Puzzle15 : ASolver {
        private char[,] map, mapWide;
        private int width, height, startX, startY;
        private string sequence;

        public override void Setup() {
            string[] lines = Input.Split('\n');
            for (int i = 0; i < lines.Length; i++) {
                string line = lines[i];
                if (string.IsNullOrEmpty(line)) {
                    height = i;
                    break;
                }
            }

            StringBuilder sb = new();
            for (int i = height + 1; i < lines.Length; i++) {
                string line = lines[i];
                sb.Append(line);
            }
            sequence = sb.ToString();

            width = lines[0].Length;
            map = new char[height, width];
            mapWide = new char[height, width * 2];

            for (int y = 0; y < height; y++) {
                string line = lines[y];
                for (int x = 0, xx = 0; x < width; x++, xx += 2) {
                    char c = line[x];
                    map[y, x] = c;

                    if (c == '@') {
                        startX = x;
                        startY = y;
                        mapWide[y, xx] = c;
                        mapWide[y, xx + 1] = '.';
                    } else if (c == 'O') {
                        mapWide[y, xx] = '[';
                        mapWide[y, xx + 1] = ']';
                    } else {
                        mapWide[y, xx] = c;
                        mapWide[y, xx + 1] = c;
                    }
                }
            }
        }

        [Description("What is the sum of all boxes' GPS coordinates?")]
        public override string SolvePart1() {
            void Update(ref int x, ref int y, int dirX, int dirY) {
                x += dirX; y += dirY;
                if (map[y, x] == '#') {
                    x -= dirX; y -= dirY;
                    return;
                }

                if (map[y, x] != 'O') {
                    map[y - dirY, x - dirX] = '.';
                    map[y, x] = '@';
                    return;
                }

                int boxX = x, boxY = y;
                do {
                    boxX += dirX; boxY += dirY;
                } while (map[boxY, boxX] == 'O');

                if (map[boxY, boxX] == '.') {
                    map[boxY, boxX] = 'O';
                    map[y, x] = '@';
                    map[y - dirY, x - dirX] = '.';
                } else {
                    x -= dirX; y -= dirY;
                }
            }

            int x = startX, y = startY;
            for (int i = 0; i < sequence.Length; i++) {
                char c = sequence[i];
                switch (c) {
                    case '>': Update(ref x, ref y, 1, 0); break;
                    case '<': Update(ref x, ref y, -1, 0); break;
                    case '^': Update(ref x, ref y, 0, -1); break;
                    case 'v': Update(ref x, ref y, 0, 1); break;
                }
            }
            return $"{SumBoxCoords(map)}";
        }

        [Description("What is the sum of all boxes' final GPS coordinates?")]
        public override string SolvePart2() {
            List<(int, int)> boxes = new();
            void UpdateWide(ref int x, ref int y, int dirX, int dirY) {
                bool CheckBox(int boxX, int boxY, int dirY) {
                    boxY += dirY;
                    ref char left = ref mapWide[boxY, boxX];
                    ref char right = ref mapWide[boxY, boxX + 1];
                    if (left == '.' && right == '.') {
                        boxes.Add((boxX, boxY - dirY));
                        return true;
                    }
                    if (left == '#' || right == '#') {
                        return false;
                    }
                    bool good = true;
                    if (left == '[' || left == ']') {
                        good = CheckBox(left == '[' ? boxX : boxX - 1, boxY, dirY);
                    }
                    if (right == '[' && good) {
                        good = CheckBox(boxX + 1, boxY, dirY);
                    }
                    if (good) { boxes.Add((boxX, boxY - dirY)); }
                    return good;
                }

                x += dirX; y += dirY;
                if (mapWide[y, x] == '#') {
                    x -= dirX; y -= dirY;
                    return;
                }

                char box = mapWide[y, x];
                if (box != '[' && box != ']') {
                    mapWide[y - dirY, x - dirX] = '.';
                    mapWide[y, x] = '@';
                    return;
                }

                if (dirY == 0) {
                    int boxX = x, boxY = y;
                    do {
                        boxX += dirX; boxY += dirY;
                    } while (mapWide[boxY, boxX] == '[' || mapWide[boxY, boxX] == ']');

                    if (mapWide[boxY, boxX] != '.') {
                        x -= dirX; y -= dirY;
                        return;
                    }

                    while (boxX != x || boxY != y) {
                        boxX -= dirX; boxY -= dirY;
                        mapWide[boxY + dirY, boxX + dirX] = mapWide[boxY, boxX];
                    }
                    mapWide[y, x] = '@';
                    mapWide[y - dirY, x - dirX] = '.';
                } else {
                    boxes.Clear();
                    bool good = CheckBox(box == '[' ? x : x - 1, y, dirY);
                    if (!good) {
                        x -= dirX; y -= dirY;
                        return;
                    }

                    boxes.Sort((left, right) => {
                        if (dirY < 0) {
                            return right.Item2.CompareTo(left.Item2);
                        }
                        return left.Item2.CompareTo(right.Item2);
                    });
                    for (int i = boxes.Count - 1; i >= 0; i--) {
                        (int boxX, int boxY) = boxes[i];
                        mapWide[boxY + dirY, boxX] = '[';
                        mapWide[boxY + dirY, boxX + 1] = ']';
                        mapWide[boxY, boxX] = '.';
                        mapWide[boxY, boxX + 1] = '.';
                    }
                    mapWide[y, x] = '@';
                    mapWide[y - dirY, x - dirX] = '.';
                }
            }

            int x = startX * 2, y = startY;
            for (int i = 0; i < sequence.Length; i++) {
                char c = sequence[i];
                switch (c) {
                    case '>': UpdateWide(ref x, ref y, 1, 0); break;
                    case '<': UpdateWide(ref x, ref y, -1, 0); break;
                    case '^': UpdateWide(ref x, ref y, 0, -1); break;
                    case 'v': UpdateWide(ref x, ref y, 0, 1); break;
                }
            }
            return $"{SumBoxCoords(mapWide)}";
        }
        private int SumBoxCoords(char[,] grid) {
            int total = 0;
            int w = grid.GetLength(1);
            for (int y = 0; y < height; y++) {
                for (int x = 0; x < w; x++) {
                    char c = grid[y, x];
                    if (c == 'O' || c == '[') { total += y * 100 + x; }
                }
            }
            return total;
        }
        private void Print(char[,] grid) {
            int w = grid.GetLength(1);
            for (int y = 0; y < height; y++) {
                for (int x = 0; x < w; x++) {
                    char c = grid[y, x];
                    Console.Write(c);
                }
                Console.WriteLine();
            }
            Console.WriteLine();
        }
    }
}