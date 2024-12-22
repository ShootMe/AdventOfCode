using AdventOfCode.Common;
using AdventOfCode.Core;
using System.Collections.Generic;
using System.ComponentModel;
namespace AdventOfCode.Y2024 {
    [Description("Keypad Conundrum")]
    public class Puzzle21 : ASolver {
        private readonly Dictionary<char, (int, int)> numpad = new() {
            { '7', (0, 0) }, { '8', (1, 0) }, { '9', (2, 0) },
            { '4', (0, 1) }, { '5', (1, 1) }, { '6', (2, 1) },
            { '1', (0, 2) }, { '2', (1, 2) }, { '3', (2, 2) },
            { '^', (1, 3) }, { '0', (1, 3) }, { 'A', (2, 3) },
            { '<', (0, 4) }, { 'v', (1, 4) }, { '>', (2, 4) }
        };
        private readonly Dictionary<(char, char, int), long> cache = new();

        [Description("What is the sum of the complexities of the five codes on your list?")]
        public override string SolvePart1() {
            long total = 0;
            foreach (string line in Input.Split('\n')) {
                total += Translate(line, 3);
            }
            return $"{total}";
        }

        [Description("What is the sum of the complexities of the five codes on your list?")]
        public override string SolvePart2() {
            long total = 0;
            foreach (string line in Input.Split('\n')) {
                total += Translate(line, 26);
            }
            return $"{total}";
        }

        private long Translate(string data, int chain) {
            long moves = 0;
            char last = 'A';
            for (int i = 0; i < data.Length; i++) {
                char next = data[i];
                moves += Translate(last, next, chain);
                last = next;
            }
            return moves * data.ToLong();
        }
        private long Translate(char last, char next, int level) {
            if (level == 0) { return 1; }
            if (cache.TryGetValue((last, next, level), out long best)) { return best; }

            (int x0, int y0) = numpad[last];
            (int x1, int y1) = numpad[next];
            int dY = y1 - y0; int dX = x1 - x0;
            best = long.MaxValue;

            if (x0 != 0 || y1 != 3) {
                best = 0; char lastPos = 'A';
                while (dY < 0) { best += Translate(lastPos, '^', level - 1); lastPos = '^'; dY++; }
                while (dY > 0) { best += Translate(lastPos, 'v', level - 1); lastPos = 'v'; dY--; }
                while (dX < 0) { best += Translate(lastPos, '<', level - 1); lastPos = '<'; dX++; }
                while (dX > 0) { best += Translate(lastPos, '>', level - 1); lastPos = '>'; dX--; }
                best += Translate(lastPos, 'A', level - 1);
            }

            if (y0 != 3 || x1 != 0) {
                dY = y1 - y0; dX = x1 - x0;
                long moves = 0; char lastPos = 'A';
                while (dX < 0) { moves += Translate(lastPos, '<', level - 1); lastPos = '<'; dX++; }
                while (dX > 0) { moves += Translate(lastPos, '>', level - 1); lastPos = '>'; dX--; }
                while (dY < 0) { moves += Translate(lastPos, '^', level - 1); lastPos = '^'; dY++; }
                while (dY > 0) { moves += Translate(lastPos, 'v', level - 1); lastPos = 'v'; dY--; }
                moves += Translate(lastPos, 'A', level - 1);
                if (moves < best) { best = moves; }
            }

            cache.Add((last, next, level), best);
            return best;
        }
    }
}