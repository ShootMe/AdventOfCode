using AdventOfCode.Core;
using System.ComponentModel;
using System.Text;
namespace AdventOfCode.Y2020 {
    [Description("Crab Cups")]
    public class Puzzle23 : ASolver {
        [Description("What are the labels on the cups after cup 1?")]
        public override string SolvePart1() {
            int[] cups = new int[Input.Length + 1];

            int previous = Input[0] - '0';
            int current = previous;
            for (int i = 1; i < Input.Length; ++i) {
                int value = Input[i] - '0';
                cups[previous] = value;
                previous = value;
            }
            cups[previous] = current;

            for (int i = 0; i < 100; i++) {
                Step(cups, current);
                current = cups[current];
            }

            StringBuilder sb = new StringBuilder();
            for (int i = 2, index = 1; i < cups.Length; i++) {
                index = cups[index];
                sb.Append(index);
            }
            return sb.ToString();
        }

        [Description("What do you get if you multiply their labels together?")]
        public override string SolvePart2() {
            int[] cups = new int[1000001];

            int previous = Input[0] - '0';
            int current = previous;
            for (int i = 1; i < Input.Length; i++) {
                int value = Input[i] - '0';
                cups[previous] = value;
                previous = value;
            }
            cups[previous] = Input.Length + 1;

            for (int i = Input.Length + 2; i < cups.Length; i++) {
                cups[i - 1] = i;
            }
            cups[cups.Length - 1] = current;

            for (int i = 0; i < 10000000; i++) {
                Step(cups, current);
                current = cups[current];
            }

            int value1 = cups[1];
            int value2 = cups[value1];
            return $"{(long)value1 * value2}";
        }

        private void Step(int[] cups, int current) {
            int val1 = cups[current];
            int val2 = cups[val1];
            int val3 = cups[val2];
            cups[current] = cups[val3];

            do {
                current--;
                if (current == 0) {
                    current = cups.Length - 1;
                }
            } while (current == val1 || current == val2 || current == val3);

            cups[val3] = cups[current];
            cups[current] = val1;
        }
    }
}