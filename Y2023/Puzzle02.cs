using AdventOfCode.Common;
using AdventOfCode.Core;
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
            public List<Cube> Cubes = new List<Cube>();
            public Game(string game) {
                int idIndex = game.IndexOf(':');
                ID = game.Substring(5, idIndex - 5).ToInt();
                string[] splits = game.Substring(idIndex + 2).Split("; ");
                for (int i = 0; i < splits.Length; i++) {
                    string[] items = splits[i].Split(", ");
                    Cube cube = new Cube();
                    for (int j = 0; j < items.Length; j++) {
                        string item = items[j];
                        int index = item.IndexOf(' ');
                        int value = item.Substring(0, index).ToInt();
                        string color = item.Substring(index + 1);
                        switch (color) {
                            case "red": cube.Red = value; break;
                            case "green": cube.Green = value; break;
                            case "blue": cube.Blue = value; break;
                        }
                    }
                    Cubes.Add(cube);
                }
            }
            public bool IsValid(int redCount, int greenCount, int blueCount) {
                for (int i = 0; i < Cubes.Count; i++) {
                    Cube cube = Cubes[i];
                    if (cube.Red > redCount || cube.Green > greenCount || cube.Blue > blueCount) {
                        return false;
                    }
                }
                return true;
            }
            public int Power() {
                int redCount = 0; int greenCount = 0; int blueCount = 0;
                for (int i = 0; i < Cubes.Count; i++) {
                    Cube cube = Cubes[i];
                    if (cube.Red > redCount) { redCount = cube.Red; }
                    if (cube.Green > greenCount) { greenCount = cube.Green; }
                    if (cube.Blue > blueCount) { blueCount = cube.Blue; }
                }
                return redCount * greenCount * blueCount;
            }
        }
        private class Cube {
            public int Red, Green, Blue;
            public override string ToString() {
                return $"{Red},{Green},{Blue}";
            }
        }
    }
}