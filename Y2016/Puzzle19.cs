using AdventOfCode.Common;
using AdventOfCode.Core;
using System.ComponentModel;
namespace AdventOfCode.Y2016 {
    [Description("An Elephant Named Joseph")]
    public class Puzzle19 : ASolver {
        [Description("Which Elf gets all the presents?")]
        public override string SolvePart1() {
            int amount = Input.ToInt();
            int powOf2 = 2;
            while (powOf2 < amount) {
                powOf2 <<= 1;
            }
            powOf2 >>= 1;

            int count = 0;
            int value = 1;
            for (int i = powOf2; i < amount; i++) {
                count++;
                value += 2;

                if (count >= powOf2) {
                    count = 0;
                    value = 1;
                    powOf2 <<= 1;
                }
            }

            return $"{value}";
        }

        [Description("Which Elf now gets all the presents?")]
        public override string SolvePart2() {
            int amount = Input.ToInt();
            int nextPowOf3 = 1;
            while (nextPowOf3 < amount) {
                nextPowOf3 *= 3;
            }
            int powOf3 = nextPowOf3 / 3;

            int count = 0;
            int value = 1;
            for (int i = powOf3 + 1; i < amount; i++) {
                count++;
                value++;
                if (count >= powOf3) {
                    count++;
                    value++;
                }
                if (count >= nextPowOf3) {
                    count = 0;
                    value = 1;
                    powOf3 = nextPowOf3;
                    nextPowOf3 *= 3;
                }
            }

            return $"{value}";
        }
    }
}