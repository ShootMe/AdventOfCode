using AdventOfCode.Common;
using AdventOfCode.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
namespace AdventOfCode.Y2021 {
    [Description("Sea Cucumber")]
    public class Puzzle25 : ASolver {
        private char[] map,nextMap;
        private int width, height;

        public override void Setup() {
            List<string> items = Input.Lines();
            width = items[0].Length;
            height = items.Count;
            map = new char[width * height];
            nextMap = new char[map.Length];
            for (int i = 0, pos = 0; i < items.Count; i++) {
                string line = items[i];
                for (int j = 0; j < width; j++) {
                    map[pos++] = line[j];
                }
            }
        }

        [Description("What is the first step on which no sea cucumbers move?")]
        public override string SolvePart1() {
            int steps = 1;
            while (Run() > 0) {
                steps++;
            }
            return $"{steps}";
        }

        private int Run() {
            int moves = 0;
            Array.Copy(map, nextMap, map.Length);

            for (int i = 0, pos = 0; i < height; i++) {
                for (int j = 0; j < width; j++, pos++) {
                    char value = map[pos];
                    if (value != '>') { continue; }

                    int nextPos = j + 1 < width ? pos + 1 : pos + 1 - width;
                    if (map[nextPos] == '.') {
                        nextMap[pos] = '.';
                        nextMap[nextPos] = '>';
                        moves++;
                    }
                }
            }

            Array.Copy(nextMap, map, nextMap.Length);

            for (int i = 0, pos = 0; i < height; i++) {
                for (int j = 0; j < width; j++, pos++) {
                    char value = nextMap[pos];
                    if (value != 'v') { continue; }

                    int nextPos = i + 1 < height ? pos + width : pos % width;
                    if (nextMap[nextPos] == '.') {
                        map[pos] = '.';
                        map[nextPos] = 'v';
                        moves++;
                    }
                }
            }

            return moves;
        }
    }
}