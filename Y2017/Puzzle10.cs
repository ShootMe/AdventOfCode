using AdventOfCode.Common;
using AdventOfCode.Core;
using System.ComponentModel;
using System.Text;
namespace AdventOfCode.Y2017 {
    [Description("Knot Hash")]
    public class Puzzle10 : ASolver {
        [Description("What is the result of multiplying the first two numbers in the list?")]
        public override string SolvePart1() {
            int[] numbers = Tools.GetInts(Input, ',');
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
            WrappingList<byte> list = KnotHash.Calculate(Input);

            StringBuilder result = new StringBuilder(32);
            foreach (int value in list) {
                result.Append(value.ToString("x2"));
            }
            return result.ToString();
        }
    }
    public static class KnotHash {
        public static WrappingList<byte> Calculate(string key) {
            byte[] values = new byte[key.Length + 5];
            for (int i = 0; i < key.Length; i++) {
                values[i] = (byte)key[i];
            }
            values[key.Length] = 17;
            values[key.Length + 1] = 31;
            values[key.Length + 2] = 73;
            values[key.Length + 3] = 47;
            values[key.Length + 4] = 23;

            WrappingList<byte> list = new WrappingList<byte>(256);

            for (int i = 0; i < 256; i++) {
                list.AddBefore((byte)i);
            }

            int skipSize = 0;
            int movesAhead = 0;
            for (int j = 0; j < 64; j++) {
                for (int i = 0; i < values.Length; i++) {
                    int value = values[i];
                    list.ReverseElements(value);
                    movesAhead += value + skipSize;
                    list.IncreasePosition(value + skipSize);
                    skipSize++;
                }
            }

            list.DecreasePosition(movesAhead % list.Count);

            for (int i = 0; i < 16; i++) {
                byte value = 0;
                for (int j = 0; j < 15; j++) {
                    value ^= list.Remove();
                }
                list.Current ^= value;
                list.IncreasePosition();
            }

            return list;
        }
    }
}