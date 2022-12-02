using AdventOfCode.Common;
using AdventOfCode.Core;
using System.Collections.Generic;
using System.ComponentModel;
namespace AdventOfCode.Y2018 {
    [Description("Chocolate Charts")]
    public class Puzzle14 : ASolver {
        [Description("What are the scores of the ten recipes after your number of recipes?")]
        public override string SolvePart1() {
            int total = Input.ToInt() + 10;

            List<int> recipes = new List<int>();
            recipes.Add(3);
            recipes.Add(7);
            int elf1 = 0;
            int elf2 = 1;

            while (recipes.Count < total) {
                int score1 = recipes[elf1];
                int score2 = recipes[elf2];
                int score = score1 + score2;
                if (score >= 10) {
                    recipes.Add(1);
                    score -= 10;
                }
                recipes.Add(score);

                elf1 = (elf1 + score1 + 1) % recipes.Count;
                elf2 = (elf2 + score2 + 1) % recipes.Count;
            }
            return $"{recipes[total - 10]}{recipes[total - 9]}{recipes[total - 8]}{recipes[total - 7]}{recipes[total - 6]}{recipes[total - 5]}{recipes[total - 4]}{recipes[total - 3]}{recipes[total - 2]}{recipes[total - 1]}";
        }

        [Description("How many recipes appear on the scoreboard to the left of the score sequence?")]
        public override string SolvePart2() {
            List<int> recipes = new List<int>();
            recipes.Add(3);
            recipes.Add(7);
            int elf1 = 0;
            int elf2 = 1;
            int matching = 0;

            while (true) {
                int score1 = recipes[elf1];
                int score2 = recipes[elf2];
                int score = score1 + score2;
                if (score >= 10) {
                    recipes.Add(1);
                    score -= 10;
                    if (Input[matching] == '1') {
                        matching++;
                        if (matching == Input.Length) {
                            return $"{recipes.Count - Input.Length}";
                        }
                    } else {
                        matching = 0;
                    }
                }

                recipes.Add(score);
                if (Input[matching] == (char)(score + 0x30)) {
                    matching++;
                    if (matching == Input.Length) {
                        return $"{recipes.Count - Input.Length}";
                    }
                } else {
                    matching = 0;
                }

                elf1 = (elf1 + score1 + 1) % recipes.Count;
                elf2 = (elf2 + score2 + 1) % recipes.Count;
            }
        }
    }
}