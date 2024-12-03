using AdventOfCode.Core;
using System.ComponentModel;
namespace AdventOfCode.Y2024 {
    [Description("Mull It Over")]
    public class Puzzle03 : ASolver {
        [Description("What do you get if you add up all of the results of the multiplications?")]
        public override string SolvePart1() {
            int total = 0;
            int index = 0;
            while ((index = Input.IndexOf("mul(", index)) >= 0) {
                index += 4;
                total += ParseMul(Input, ref index);
            }
            return $"{total}";
        }
        
        [Description("What do you get if you add up all of the results of just the enabled multiplications?")]
        public override string SolvePart2() {
            int total = 0;
            int index = 0;
            while ((index = Input.IndexOf("mul(", index)) >= 0) {
                int doIndex = Input.LastIndexOf("do()", index);
                int dontIndex = Input.LastIndexOf("don't()", index);
                index += 4;
                if (doIndex >= dontIndex) {
                    total += ParseMul(Input, ref index);
                }
            }
            return $"{total}";
        }

        private int ParseMul(string val, ref int index) {
            int ParseNum(char endChar, ref int index) {
                int num = 0;
                while (index < val.Length) {
                    char c = val[index++];
                    if (char.IsDigit(c)) {
                        num = num * 10 + (c - 0x30);
                        continue;
                    } else if (c != endChar) {
                        return -1;
                    }
                    break;
                }
                return num;
            }

            int num1 = ParseNum(',', ref index);
            if (num1 < 0) { return 0; }

            int num2 = ParseNum(')', ref index);
            return num2 >= 0 ? num1 * num2 : 0;
        }
    }
}