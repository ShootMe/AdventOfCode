using AdventOfCode.Core;
using System.Collections.Generic;
using System.ComponentModel;
namespace AdventOfCode.Y2023 {
    [Description("Trebuchet?!")]
    public class Puzzle01 : ASolver {
        private string[] digits = new[] { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "zero", "one", "two", "three", "four", "five", "six", "seven", "eight", "nine" };
        private List<string> calibrations = new();

        public override void Setup() {
            foreach (string line in Input.Split('\n')) {
                calibrations.Add(line);
            }
        }

        [Description("What is the sum of all of the calibration values?")]
        public override string SolvePart1() {
            int total = 0;
            for (int i = 0; i < calibrations.Count; i++) {
                string calibration = calibrations[i];
                total += GetFirstCalibrationDigit(calibration, 10) * 10 + GetSecondCalibrationDigit(calibration, 10);
            }
            return $"{total}";
        }

        [Description("What is the sum of all of the calibration values?")]
        public override string SolvePart2() {
            int total = 0;
            for (int i = 0; i < calibrations.Count; i++) {
                string calibration = calibrations[i];
                total += GetFirstCalibrationDigit(calibration, 20) * 10 + GetSecondCalibrationDigit(calibration, 20);
            }
            return $"{total}";
        }
        private int GetFirstCalibrationDigit(string calibration, int maxStrings) {
            int minimum = calibration.Length;
            int firstDigit = 0;
            for (int i = 0; i < maxStrings; i++) {
                int index = calibration.IndexOf(digits[i]);
                if (index >= 0 && index < minimum) {
                    minimum = index;
                    firstDigit = i;
                }
            }
            return firstDigit % 10;
        }
        private int GetSecondCalibrationDigit(string calibration, int maxStrings) {
            int maximum = -1;
            int secondDigit = 0;
            for (int i = 0; i < maxStrings; i++) {
                int index = calibration.LastIndexOf(digits[i]);
                if (index >= 0 && index > maximum) {
                    maximum = index;
                    secondDigit = i;
                }
            }
            return secondDigit % 10;
        }
    }
}