using AdventOfCode.Core;
using System.Collections.Generic;
using System.ComponentModel;
namespace AdventOfCode.Y2021 {
    [Description("Giant Squid")]
    public class Puzzle04 : ASolver {
        private List<Board> boards;
        private int[] numbersToCall;

        public override void Setup() {
            List<string> items = Tools.GetLines(Input);
            numbersToCall = Tools.GetInts(items[0], ',');
            boards = new List<Board>();

            for (int i = 2; i < items.Count; i += 6) {
                boards.Add(new Board(items, i));
            }
        }

        [Description("What will your final score be if you choose that board?")]
        public override string SolvePart1() {
            for (int i = 0; i < numbersToCall.Length; i++) {
                int number = numbersToCall[i];

                for (int j = 0; j < boards.Count; j++) {
                    Board board = boards[j];
                    if (board.CallNumber(number)) {
                        return $"{board.UnmarkedSum() * number}";
                    }
                }
            }
            return string.Empty;
        }

        [Description("What would the final score be for the last board to win?")]
        public override string SolvePart2() {
            int lastWinner = 0;
            for (int i = 0; i < numbersToCall.Length; i++) {
                int number = numbersToCall[i];

                for (int j = 0; j < boards.Count; j++) {
                    Board board = boards[j];
                    if (board.CallNumber(number)) {
                        lastWinner = board.UnmarkedSum() * number;
                    }
                }
            }
            return $"{lastWinner}";
        }

        private class Board {
            public int[,] Numbers;
            public bool[,] Marked;
            public bool Finished;

            public Board(List<string> lines, int start) {
                int end = start + 5;
                Numbers = new int[5, 5];
                Marked = new bool[5, 5];
                for (int i = start; i < end; i++) {
                    int[] values = Tools.GetInts(lines[i], ' ');
                    for (int j = 0; j < values.Length; j++) {
                        Numbers[i - start, j] = values[j];
                    }
                }
            }

            public int UnmarkedSum() {
                int sum = 0;
                for (int i = 0; i < 5; i++) {
                    for (int j = 0; j < 5; j++) {
                        if (!Marked[i, j]) {
                            sum += Numbers[i, j];
                        }
                    }
                }
                return sum;
            }
            public bool CallNumber(int number) {
                if (Finished) { return false; }

                for (int i = 0; i < 5; i++) {
                    int totalR = 0;
                    int totalC = 0;

                    for (int j = 0; j < 5; j++) {
                        if (Numbers[i, j] == number) {
                            Marked[i, j] = true;
                        } else if (Numbers[j, i] == number) {
                            Marked[j, i] = true;
                        }

                        if (Marked[i, j]) {
                            totalR++;
                        }
                        if (Marked[j, i]) {
                            totalC++;
                        }
                    }

                    if (totalR == 5 || totalC == 5) {
                        Finished = true;
                        return true;
                    }
                }

                return false;
            }
        }
    }
}