using AdventOfCode.Core;
using System.ComponentModel;
using System.Numerics;
namespace AdventOfCode.Y2020 {
    [Description("Combo Breaker")]
    public class Puzzle25 : ASolver {
        private int CardKey, DoorKey;

        public override void Setup() {
            int[] numbers = Tools.GetInts(Input);
            CardKey = numbers[0];
            DoorKey = numbers[1];
        }

        [Description("What encryption key is the handshake trying to establish?")]
        public override string SolvePart1() {
            int value = 1;
            int loopAmount = 0;
            while (true) {
                value *= 7;
                if (value > 20201227) { value %= 20201227; }

                loopAmount++;
                if (value == CardKey) {
                    return $"{BigInteger.ModPow(DoorKey, loopAmount, 20201227)}";
                } else if (value == DoorKey) {
                    return $"{BigInteger.ModPow(CardKey, loopAmount, 20201227)}";
                }
            }
        }
    }
}