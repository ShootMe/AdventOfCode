using AdventOfCode.Common;
using AdventOfCode.Core;
using System.ComponentModel;
using System.Text;
namespace AdventOfCode.Y2017 {
    public class Puzzle10 : ASolver {
        public Puzzle10(string input) : base(input) { Name = "Knot Hash"; }

        [Description("What is the result of multiplying the first two numbers in the list?")]
        public override string SolvePart1() {
            int[] numbers = Tools.GetNumbers(Input, ',');
            WrappingList<int> list = new WrappingList<int>(256);

            for (int i = 0; i < 256; i++) {
                list.AddBefore(i);
            }

            int skipSize = 0;
            int movesAhead = 0;
            for (int i = 0; i < numbers.Length; i++) {
                int value = numbers[i];
                list.ReverseElements(value);
                movesAhead += value + skipSize;
                list.IncreasePosition(value + skipSize);
                skipSize++;
            }

            list.DecreasePosition(movesAhead % list.Count);
            int value1 = list.Current;
            int value2 = list.Next;
            return $"{value1 * value2}";
        }

        [Description("What is the Knot Hash of your puzzle input?")]
        public override string SolvePart2() {
            int[] numbers = new int[Input.Length + 5];
            for (int i = 0; i < Input.Length; i++) {
                numbers[i] = Input[i];
            }
            numbers[Input.Length] = 17;
            numbers[Input.Length + 1] = 31;
            numbers[Input.Length + 2] = 73;
            numbers[Input.Length + 3] = 47;
            numbers[Input.Length + 4] = 23;

            WrappingList<int> list = new WrappingList<int>(256);

            for (int i = 0; i < 256; i++) {
                list.AddBefore(i);
            }

            int skipSize = 0;
            int movesAhead = 0;
            for (int j = 0; j < 64; j++) {
                for (int i = 0; i < numbers.Length; i++) {
                    int value = numbers[i];
                    list.ReverseElements(value);
                    movesAhead += value + skipSize;
                    list.IncreasePosition(value + skipSize);
                    skipSize++;
                }
            }

            list.DecreasePosition(movesAhead % list.Count);

            for (int i = 0; i < 16; i++) {
                int value = 0;
                for (int j = 0; j < 15; j++) {
                    value ^= list.Remove();
                }
                list.Current ^= value;
                list.IncreasePosition();
            }

            StringBuilder result = new StringBuilder(32);
            foreach (int value in list) {
                result.Append(value.ToString("x2"));
            }
            return result.ToString();
        }
    }
}