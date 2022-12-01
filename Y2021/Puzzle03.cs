using AdventOfCode.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
namespace AdventOfCode.Y2021 {
    [Description("Binary Diagnostic")]
    public class Puzzle03 : ASolver {
        private List<string> numbers;

        public override void Setup() {
            numbers = Tools.GetLines(Input);
        }

        [Description("What is the power consumption of the submarine?")]
        public override string SolvePart1() {
            int gamma = 0;
            int epsilon = 0;
            int mask = 1;
            int bits = numbers[0].Length;
            for (int i = bits - 1; i >= 0; i--) {
                (int ones, int zeros) = CountBits(numbers, i);

                if (ones > zeros) {
                    gamma |= mask;
                } else {
                    epsilon |= mask;
                }
                mask <<= 1;
            }
            return $"{gamma * epsilon}";
        }

        [Description("What is the life support rating of the submarine?")]
        public override string SolvePart2() {
            List<string> oxy = new List<string>(numbers);
            List<string> co2 = new List<string>(numbers);

            int bits = numbers[0].Length;
            while (co2.Count > 1 && co2.Count > 1) {
                for (int i = 0; i < bits; i++) {
                    (int ones, int zeros) = CountBits(oxy, i);

                    RemoveValues(oxy, i, zeros <= ones ? '0' : '1');

                    if (co2.Count > 1) {
                        (ones, zeros) = CountBits(co2, i);

                        RemoveValues(co2, i, zeros > ones ? '0' : '1');
                    }
                }
            }

            return $"{Convert.ToInt32(oxy[0], 2) * Convert.ToInt32(co2[0], 2)}";
        }
        private static (int ones, int zeros) CountBits(List<string> values, int position) {
            int ones = 0;
            for (int i = 0; i < values.Count; i++) {
                ones += values[i][position] - '0';
            }
            return (ones, values.Count - ones);
        }
        private static void RemoveValues(List<string> values, int position, char removeDigit) {
            for (int i = values.Count - 1; i >= 0; i--) {
                if (values[i][position] == removeDigit) {
                    values.RemoveAt(i);
                }
            }
        }
    }
}