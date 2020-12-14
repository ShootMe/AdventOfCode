using AdventOfCode.Common;
using AdventOfCode.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
namespace AdventOfCode.Y2017 {
    public class Puzzle15 : ASolver {
        private long generatorA;
        private long generatorB;
        public Puzzle15(string input) : base(input) { Name = "Dueling Generators"; }

        public override void Setup() {
            List<string> items = Tools.GetLines(Input);
            generatorA = Tools.ParseLong(items[0], "starts with ");
            generatorB = Tools.ParseLong(items[1], "starts with ");
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

        [Description("What is the answer?")]
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