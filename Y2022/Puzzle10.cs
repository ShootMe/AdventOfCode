using AdventOfCode.Common;
using AdventOfCode.Core;
using System.ComponentModel;
namespace AdventOfCode.Y2022 {
    [Description("Cathode-Ray Tube")]
    public class Puzzle10 : ASolver {
        [Description("What is the sum of these six signal strengths?")]
        public override string SolvePart1() {
            int spriteX = 1;
            int cycle = 0;
            int power = 0;
            foreach (string line in Input.Split('\n')) {
                cycle++;
                if ((cycle - 20) % 40 == 0) {
                    power += cycle * spriteX;
                }

                if (line[0] == 'a') {
                    cycle++;
                    if ((cycle - 20) % 40 == 0) {
                        power += cycle * spriteX;
                    }
                    spriteX += line[5..].ToInt();
                }
            }
            return $"{power}";
        }
        
        [Description("Render the image given by your program. What eight capital letters appear on your CRT?")]
        public override string SolvePart2() {
            int spriteX = 1;
            int cycleX = 0;
            int cycleY = 0;
            bool[,] crt = new bool[6, 40];
            foreach (string line in Input.Split('\n')) {
                crt[cycleY, cycleX] = cycleX >= spriteX - 1 && cycleX <= spriteX + 1;

                cycleX++;
                if (cycleX == 40) {
                    cycleX = 0;
                    cycleY++;
                }

                if (line[0] == 'a') {
                    crt[cycleY, cycleX] = cycleX >= spriteX - 1 && cycleX <= spriteX + 1;

                    cycleX++;
                    if (cycleX == 40) {
                        cycleX = 0;
                        cycleY++;
                    }
                    spriteX += line[5..].ToInt();
                }
            }

            return Extensions.FindStringInGrid(crt);
        }
    }
}