using AdventOfCode.Common;
using AdventOfCode.Core;
using System.ComponentModel;
namespace AdventOfCode.Y2018 {
    [Description("Marble Mania")]
    public class Puzzle09 : ASolver {
        [Description("What is the winning Elf's score?")]
        public override string SolvePart1() {
            int index1 = Input.IndexOf(' ');
            int players = Tools.ParseInt(Input, 0, index1);

            index1 = Input.IndexOf("worth");
            int index2 = Input.IndexOf(" points");
            int lastMarble = Tools.ParseInt(Input, index1 + 6, index2 - index1 - 6);

            return $"{GetBestScore(lastMarble, players)}";
        }

        [Description("What would the winning Elf's score be if the last marble were 100 times larger?")]
        public override string SolvePart2() {
            int index1 = Input.IndexOf(' ');
            int players = Tools.ParseInt(Input, 0, index1);

            index1 = Input.IndexOf("worth");
            int index2 = Input.IndexOf(" points");
            int lastMarble = Tools.ParseInt(Input, index1 + 6, index2 - index1 - 6);

            return $"{GetBestScore(lastMarble * 100, players)}";
        }

        private long GetBestScore(int lastMarble, int players) {
            WrappingList<int> marbles = new WrappingList<int>(lastMarble);
            marbles.AddAfter(0);

            int marble = 1;
            long[] playerScores = new long[players];
            int currentPlayer = 0;

            while (marble <= lastMarble) {
                if ((marble % 23) != 0) {
                    marbles.IncreasePosition();
                    marbles.AddAfter(marble, true);
                } else {
                    marbles.DecreasePosition(7);
                    int value = marbles.Remove();
                    playerScores[currentPlayer] += marble + value;
                }

                marble++;
                if (++currentPlayer == players) {
                    currentPlayer = 0;
                }
            }

            long best = long.MinValue;
            for (int i = 0; i < players; i++) {
                if (playerScores[i] > best) {
                    best = playerScores[i];
                }
            }
            return best;
        }
    }
}