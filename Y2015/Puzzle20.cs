using AdventOfCode.Core;
using System;
using System.ComponentModel;
namespace AdventOfCode.Y2015 {
    [Description("Infinite Elves and Infinite Houses")]
    public class Puzzle20 : ASolver {
        [Description("What is the lowest house number of the house to get the amount of presents?")]
        public override string SolvePart1() {
            int[] primes = new int[] { 2, 3, 5, 7, 11, 13, 17 };
            int[] counts = new int[] { 0, 0, 0, 0, 0, 0, 0 };
            int current = 1;

            int presents = Tools.ParseInt(Input);
            int maxValue = presents / 10;

            while (true) {
                current = GetNextNumber(primes, counts, current, maxValue);
                if (current < 0) { break; }

                int amount = CalculatePresents(primes, counts, 0);
                if (amount >= presents && current < maxValue) {
                    maxValue = current;
                }
            }

            return $"{maxValue}";
        }

        [Description("With these changes, what is the new lowest house number?")]
        public override string SolvePart2() {
            int[] primes = new int[] { 2, 3, 5, 7, 11, 13, 17 };
            int[] counts = new int[] { 0, 0, 0, 0, 0, 0, 0 };
            int current = 1;

            int presents = Tools.ParseInt(Input);
            int maxValue = presents / 10;

            while (true) {
                current = GetNextNumber(primes, counts, current, maxValue);
                if (current < 0) { break; }

                int amount = CalculatePresents(primes, counts, current);
                if (amount >= presents && current < maxValue) {
                    maxValue = current;
                }
            }

            return $"{maxValue}";
        }

        private int GetNextNumber(int[] primes, int[] counts, int current, int max) {
            for (int i = 0; i < primes.Length; i++) {
                int prime = primes[i];
                current *= prime;
                counts[i]++;
                if (current < max) {
                    return current;
                }

                for (int j = 0; j <= i; j++) {
                    int count = counts[j];
                    prime = primes[j];
                    while (count-- > 0) {
                        current /= prime;
                    }
                    counts[j] = 0;
                }
            }

            return -1;
        }
        private int CalculatePresents(int[] primes, int[] counts, int house) {
            Span<int> currentCounts = stackalloc int[counts.Length];

            int index = 0;
            int total = 0;
            do {
                total = counts[index++];
            } while (total == 0);
            int prime = primes[index - 1];

            int presents = 50 >= house ? 1 : 0;
            int current = 1;
            int last = current;
            while (true) {
                for (int i = 0; i < total; i++) {
                    current *= prime;
                    if (current * 50 >= house) {
                        presents += current;
                    }
                }
                current = last;

                int primeIndex = index;
                while (primeIndex < primes.Length) {
                    int count = counts[primeIndex];

                    int currentCount = ++currentCounts[primeIndex];
                    if (currentCount <= count) {
                        current *= primes[primeIndex];
                        if (current * 50 >= house) {
                            presents += current;
                        }
                        last = current;
                        break;
                    }

                    int lastPrime = primes[primeIndex];
                    while (count-- > 0) {
                        current /= lastPrime;
                    }
                    last = current;
                    currentCounts[primeIndex++] = 0;
                }

                if (primeIndex == primes.Length) { return presents * (house == 0 ? 10 : 11); }
            }
        }
    }
}