using AdventOfCode.Core;
using System.Collections.Generic;
using System.ComponentModel;
namespace AdventOfCode.Y2024 {
    [Description("Garden Groups")]
    public class Puzzle12 : ASolver {
        private char[,] map;
        private int width, height;

        public override void Setup() {
            string[] lines = Input.Split('\n');
            height = lines.Length;
            width = lines[0].Length;
            map = new char[height, width];

            for (int y = 0; y < height; y++) {
                string line = lines[y];
                for (int x = 0; x < width; x++) {
                    char c = line[x];
                    map[y, x] = c;
                }
            }
        }

        [Description("What is the total price of fencing all regions on your map?")]
        public override string SolvePart1() {
            int total = 0;
            char plant = '\0';
            Queue<(int, int)> open = new();
            while (plant != '.') {
                char NextPlant() {
                    for (int y = 0; y < height; y++) {
                        for (int x = 0; x < width; x++) {
                            char value = map[y, x];
                            if (value > 'Z') { continue; }
                            open.Enqueue((x, y));
                            map[y, x] += (char)0x30;
                            return value;
                        }
                    }
                    return '.';
                }
                int AddNext(int x, int y, char next) {
                    if (x < 0 || x >= width || y < 0 || y >= height) { return 0; }
                    char m = map[y, x];
                    if (m != next) {
                        if (m == next + 0x30) { return 1; }
                        return 0;
                    }
                    open.Enqueue((x, y));
                    map[y, x] += (char)'0';
                    return 1;
                }

                plant = NextPlant();
                int area = 0, perimeter = 0;
                while (open.Count > 0) {
                    (int x, int y) = open.Dequeue();

                    int added = AddNext(x - 1, y, plant);
                    added += AddNext(x + 1, y, plant);
                    added += AddNext(x, y - 1, plant);
                    added += AddNext(x, y + 1, plant);

                    area++;
                    perimeter += 4 - added;
                }
                total += area * perimeter;
            }
            return $"{total}";
        }

        [Description("What is the new total price of fencing all regions on your map?")]
        public override string SolvePart2() {
            int total = 0;
            char plant = '\0';
            Queue<(int, int)> open = new();
            while (plant != '.') {
                char NextPlant() {
                    for (int y = 0; y < height; y++) {
                        for (int x = 0; x < width; x++) {
                            char value = map[y, x];
                            if (value <= 'Z') { continue; }
                            open.Enqueue((x, y));
                            map[y, x] -= (char)0x30;
                            return value;
                        }
                    }
                    return '.';
                }
                int AddNext(int x, int y, char next) {
                    if (x < 0 || x >= width || y < 0 || y >= height) { return 0; }
                    char m = map[y, x];
                    if (m != next) {
                        if (m == next - 0x30) { return 1; }
                        return 0;
                    }
                    open.Enqueue((x, y));
                    map[y, x] -= (char)'0';
                    return 1;
                }
                bool GetValue(int x, int y, char plant) {
                    if (x < 0 || x >= width || y < 0 || y >= height) { return false; }
                    char m = map[y, x];
                    return m == plant || m == plant - 0x30;
                }

                plant = NextPlant();
                int area = 0, perimeter = 0;
                while (open.Count > 0) {
                    (int x, int y) = open.Dequeue();

                    bool addedLeft = AddNext(x - 1, y, plant) == 1;
                    bool addedRight = AddNext(x + 1, y, plant) == 1;
                    bool addedTop = AddNext(x, y - 1, plant) == 1;
                    bool addedBottom = AddNext(x, y + 1, plant) == 1;

                    bool leftTop = GetValue(x - 1, y - 1, plant);
                    bool rightTop = GetValue(x + 1, y - 1, plant);
                    bool leftBottom = GetValue(x - 1, y + 1, plant);
                    bool rightBottom = GetValue(x + 1, y + 1, plant);

                    area++;
                    if ((!addedLeft && !addedTop) || (addedLeft && addedTop && !leftTop)) { perimeter++; }
                    if ((!addedTop && !addedRight) || (addedTop && addedRight && !rightTop)) { perimeter++; }
                    if ((!addedRight && !addedBottom) || (addedRight && addedBottom && !rightBottom)) { perimeter++; }
                    if ((!addedBottom && !addedLeft) || (addedBottom && addedLeft && !leftBottom)) { perimeter++; }
                }
                total += area * perimeter;
            }
            return $"{total}";
        }
    }
}