using AdventOfCode.Common;
using AdventOfCode.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
namespace AdventOfCode.Y2023 {
    [Description("Cube Conundrum")]
    public class Puzzle02 : ASolver {
        private List<Game> games = new();

        public override void Setup() {
            foreach (string line in Input.Split('\n')) {
                games.Add(new Game(line));
            }
        }

        [Description("What is the sum of the IDs of those games?")]
        public override string SolvePart1() {
            int total = 0;
            for (int i = 0; i < games.Count; i++) {
                Game game = games[i];
                if (game.IsValid(12, 13, 14)) {
                    total += game.ID;
                }
            }
            return $"{total}";
        }

        [Description("What is the sum of the power of these sets?")]
        public override string SolvePart2() {
            int total = 0;
            for (int i = 0; i < games.Count; i++) {
                Game game = games[i];
                total += game.Power();
            }
            return $"{total}";
        }

        private class Game {
            public int ID;
            public int MaxRed, MaxGreen, MaxBlue;
            public Game(string game) {
                int idIndex = game.IndexOf(':');
                ID = game.Substring(5, idIndex - 5).ToInt();
                string[] cubes = game.Substring(idIndex + 2).Split(new string[] { ", ", "; " }, StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < cubes.Length; i++) {
                    string cube = cubes[i];
                    int index = cube.IndexOf(' ');
                    int amount = cube.Substring(0, index).ToInt();
                    string color = cube.Substring(index + 1);
                    switch (color) {
                        case "red": if (MaxRed < amount) { MaxRed = amount; } break;
                        case "green": if (MaxGreen < amount) { MaxGreen = amount; } break;
                        case "blue": if (MaxBlue < amount) { MaxBlue = amount; } break;
                    }
                }
            }
            public bool IsValid(int redCount, int greenCount, int blueCount) {
                return MaxRed <= redCount && MaxGreen <= greenCount && MaxBlue <= blueCount;
            }
            public int Power() {
                return MaxRed * MaxGreen * MaxBlue;
            }
            public override string ToString() {
                return $"{ID}={MaxRed},{MaxGreen},{MaxBlue}";
            }
        }
    }
}