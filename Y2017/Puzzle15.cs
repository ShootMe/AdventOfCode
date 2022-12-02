using AdventOfCode.Common;
using AdventOfCode.Core;
using System.Collections.Generic;
using System.ComponentModel;
namespace AdventOfCode.Y2017 {
    [Description("Dueling Generators")]
    public class Puzzle15 : ASolver {
        private long generatorA;
        private long generatorB;

        public override void Setup() {
            List<string> items = Input.Lines();
            generatorA = items[0].Substring(24).ToLong();
            generatorB = items[1].Substring(24).ToLong();
        }

        [Description("After 40 million pairs, what is the judge's final count?")]
        public override string SolvePart1() {
            long genA = generatorA;
            long genB = generatorB;
            int count = 0;
            for (int i = 0; i < 40000000; i++) {
                genA = (genA * 16807) % int.MaxValue;
                genB = (genB * 48271) % int.MaxValue;
                if ((ushort)genA == (ushort)genB) {
                    count++;
                }
            }
            return $"{count}";
        }

        [Description("After 5 million pairs, what is the judge's final count?")]
        public override string SolvePart2() {
            long genA = generatorA;
            long genB = generatorB;
            int count = 0;
            for (int i = 0; i < 5000000; i++) {
                do {
                    genA = (genA * 16807) % int.MaxValue;
                } while ((genA & 3) != 0);
                do {
                    genB = (genB * 48271) % int.MaxValue;
                } while ((genB & 7) != 0);

                if ((ushort)genA == (ushort)genB) {
                    count++;
                }
            }
            return $"{count}";
        }
    }
}