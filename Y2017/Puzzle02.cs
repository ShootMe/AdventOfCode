using AdventOfCode.Core;
using System.Collections.Generic;
using System.ComponentModel;
namespace AdventOfCode.Y2017 {
    public class Puzzle02 : ASolver {
        private List<string> items;
        public Puzzle02(string input) : base(input) { Name = "Corruption Checksum"; }

        public override void Setup() {
            items = Tools.GetLines(Input);
        }

        [Description("What is the checksum for the spreadsheet?")]
        public override string SolvePart1() {
            int sum = 0;
            for (int i = 0; i < items.Count; i++) {
                string row = items[i];
                string[] numbers = row.Split('\t', System.StringSplitOptions.RemoveEmptyEntries);
                int min = int.MaxValue;
                int max = 0;
                for (int j = 0; j < numbers.Length; j++) {
                    int number = Tools.ParseInt(numbers[j]);

                    if (number > max) {
                        max = number;
                    }
                    if (number < min) {
                        min = number;
                    }
                }

                sum += max - min;
            }
            return $"{sum}";
        }

        [Description("What is the sum of each row's result?")]
        public override string SolvePart2() {
            int sum = 0;
            for (int i = 0; i < items.Count;) {
                string row = items[i];
                string[] numbers = row.Split('\t', System.StringSplitOptions.RemoveEmptyEntries);

                for (int j = 0; j < numbers.Length; j++) {
                    int number1 = Tools.ParseInt(numbers[j]);

                    for (int k = 0; k < numbers.Length; k++) {
                        if (j == k) { continue; }

                        int number2 = Tools.ParseInt(numbers[k]);

                        if (number2 != 0 && number1 != 0 && (number1 % number2) == 0) {
                            sum += number1 / number2;
                            goto NextNumber;
                        }
                    }
                }
            NextNumber:
                i++;
            }

            return $"{sum}";
        }
    }
}