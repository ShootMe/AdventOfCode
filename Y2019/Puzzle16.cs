using AdventOfCode.Common;
using AdventOfCode.Core;
using System;
using System.ComponentModel;
namespace AdventOfCode.Y2019 {
    [Description("Flawed Frequency Transmission")]
    public class Puzzle16 : ASolver {
        [Description("What are the first eight digits in the final output list?")]
        public override string SolvePart1() {
            byte[] phase = new byte[Input.Length];
            for (int i = 0; i < Input.Length; i++) {
                phase[i] = (byte)(Input[i] - '0');
            }

            for (int k = 0; k < 100; k++) {
                for (int i = 0; i < phase.Length; i++) {
                    int patternIndex = 1;
                    int patternMultiple = 0;
                    int sum = 0;
                    for (int j = i; j < phase.Length; j++) {
                        if (patternIndex == 1) {
                            sum += phase[j];
                        } else {
                            sum -= phase[j];
                        }

                        patternMultiple++;
                        if (patternMultiple > i) {
                            patternMultiple = 0;
                            patternIndex += 2;
                            j += i + 1;

                            if (patternIndex >= 4) {
                                patternIndex = 1;
                            }
                        }
                    }

                    phase[i] = (byte)((sum < 0 ? -sum : sum) % 10);
                }
            }
            return $"{phase[0]}{phase[1]}{phase[2]}{phase[3]}{phase[4]}{phase[5]}{phase[6]}{phase[7]}";
        }

        [Description("What is the eight-digit message embedded in the final output list?")]
        public override string SolvePart2() {
            int offset = Input.Substring(0, 7).ToInt();
            int needed = 10000 - offset / Input.Length;
            offset = offset % Input.Length;

            byte[] phase = new byte[Input.Length * needed];
            for (int i = 0; i < Input.Length; i++) {
                phase[i] = (byte)(Input[i] - '0');
            }
            for (int i = 1; i < needed; i++) {
                Array.Copy(phase, 0, phase, i * Input.Length, Input.Length);
            }

            int nextSum = 0;
            int sum = 0;
            for (int i = offset; i < phase.Length; i++) {
                sum += phase[i];
            }

            for (int k = 0; k < 100; k++) {
                for (int i = offset; i < phase.Length; i++) {
                    int temp = phase[i];
                    byte next = (byte)(sum % 10);
                    phase[i] = next;
                    sum -= temp;
                    nextSum += next;
                }
                sum = nextSum;
                nextSum = 0;
            }
            return $"{phase[offset]}{phase[offset + 1]}{phase[offset + 2]}{phase[offset + 3]}{phase[offset + 4]}{phase[offset + 5]}{phase[offset + 6]}{phase[offset + 7]}";
        }
    }
}