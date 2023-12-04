using AdventOfCode.Common;
using AdventOfCode.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
namespace AdventOfCode.Y2023 {
    [Description("Scratchcards")]
    public class Puzzle04 : ASolver {
        private int[] scorecardCounts;

        [Description("Take a seat in the large pile of colorful cards. How many points are they worth in total?")]
        public override string SolvePart1() {
            int total = 0;
            string[] cards = Input.Split('\n');
            scorecardCounts = new int[cards.Length];
            for (int j = 0; j < cards.Length; j++) {
                string card = cards[j];
                int winners = CountOfWinningNumbers(card);

                int current = scorecardCounts[j] + 1;
                if (winners > 0) {
                    for (int i = 0; i < winners; i++) {
                        scorecardCounts[i + j + 1] += current;
                    }
                    total += 1 << (winners - 1);
                }
            }
            return $"{total}";
        }

        [Description("Including the original set of scratchcards, how many total scratchcards do you end up with?")]
        public override string SolvePart2() {
            int total = 0;
            for (int j = 0; j < scorecardCounts.Length; j++) {
                total += scorecardCounts[j] + 1;
            }
            return $"{total}";
        }
        private int CountOfWinningNumbers(string card) {
            int start = card.IndexOf(':');
            int separator = card.IndexOf('|');

            string[] winners = card.Substring(start + 2, separator - start - 3).Split(' ', StringSplitOptions.RemoveEmptyEntries);
            HashSet<int> winningSet = new HashSet<int>();
            for (int i = 0; i < winners.Length; i++) {
                winningSet.Add(winners[i].ToInt());
            }

            string[] myNumbers = card.Substring(separator + 2).Split(' ', StringSplitOptions.RemoveEmptyEntries);
            int match = 0;
            for (int i = 0; i < myNumbers.Length; i++) {
                if (winningSet.Contains(myNumbers[i].ToInt())) {
                    match++;
                }
            }

            return match;
        }
    }
}