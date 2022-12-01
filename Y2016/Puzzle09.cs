using AdventOfCode.Core;
using System.ComponentModel;
namespace AdventOfCode.Y2016 {
    [Description("Explosives in Cyberspace")]
    public class Puzzle09 : ASolver {
        [Description("What is the decompressed length of the file?")]
        public override string SolvePart1() {
            return $"{DecompressedLength(Input, 0, Input.Length)}";
        }

        [Description("What is the decompressed length of the file using this improved format?")]
        public override string SolvePart2() {
            return $"{DecompressedLength(Input, 0, Input.Length, true)}";
        }

        private long DecompressedLength(string value, int start, int end, bool recursive = false) {
            int index = start;
            long length = 0;
            do {
                int indexNext = value.IndexOf('(', index);
                if (indexNext < 0 || indexNext > end) { indexNext = end; }
                length += indexNext - index;

                if (indexNext == end) { break; }

                int indexSplit = value.IndexOf('x', indexNext);
                int chars = Tools.ParseInt(value, indexNext + 1, indexSplit - indexNext - 1);
                index = value.IndexOf(')', indexSplit);
                int repeat = Tools.ParseInt(value, indexSplit + 1, index - indexSplit - 1);
                long totalChars = chars;
                if (recursive) {
                    totalChars = DecompressedLength(value, index + 1, index + chars + 1, true);
                }
                length += totalChars * repeat;
                index += chars + 1;
            } while (true);

            return length;
        }
    }
}