using AdventOfCode.Common;
using AdventOfCode.Core;
using System.ComponentModel;
using System.Text;
namespace AdventOfCode.Y2024 {
    [Description("Chronospatial Computer")]
    public class Puzzle17 : ASolver {
        private int[] program;
        private long regA, regB, regC;
        public override void Setup() {
            string[] lines = Input.Split('\n');
            regA = lines[0].ToInt();
            regB = lines[1].ToInt();
            regC = lines[2].ToInt();
            program = lines[4].ToInts(',');
        }

        [Description("What do you get if you use commas to join the values it output into a single string?")]
        public override string SolvePart1() {
            StringBuilder sb = new();
            RunProgram(regA, sb);
            sb.Length--;
            return sb.ToString();
        }

        [Description("What is the lowest positive initial value for register A that causes the program to output a copy of itself?")]
        public override string SolvePart2() {
            if (program.Length == 16) {
                return RunComp();
            }
            for (long i = 0; i < 2000000; i++) {
                if (RunProgram(i)) {
                    return $"{i}";
                }
            }
            return string.Empty;
        }
        private string RunComp() {
            long a = 0;
            long start = 0;
            int digit = program.Length - 1;
            while (digit >= 0 && digit < program.Length) {
                int index = 0;
                for (long i = start; i < 8; i++) {
                    index = digit;
                    long tempA = a | i;
                    long b = 0;
                    while (index < program.Length) {
                        b = (tempA & 7) ^ 6; //2,4,1,6
                        b ^= (tempA >> (int)b) ^ 7; //7,5,4,4,1,7
                        tempA >>= 3;   //0,3
                        if (program[index] != (b & 7)) { break; }//5,5,3,0
                        index++;
                    }
                    if (index == program.Length) {
                        a = (a | i) << 3;
                        digit--;
                        start = 0;
                        break;
                    }
                }
                if (index != program.Length) {
                    do {
                        a >>= 3;
                        start = (a & 7) + 1;
                        a &= ~0x7;
                        digit++;
                    } while (start == 8);
                }
            }
            return $"{a >> 3}";
        }
        private bool RunProgram(long a, StringBuilder sb = null) {
            long b = regB, c = regC, temp = regA;
            regA = a;
            int index = 0;
            for (int i = 0; i < program.Length; i += 2) {
                int opcode = program[i];
                int valueLit = program[i + 1];
                long valueCombo = valueLit;
                switch (valueCombo) {
                    case 4: valueCombo = regA; break;
                    case 5: valueCombo = regB; break;
                    case 6: valueCombo = regC; break;
                }
                switch (opcode) {
                    case 0: regA >>= (int)valueCombo; break;
                    case 1: regB ^= valueLit; break;
                    case 2: regB = valueCombo & 7; break;
                    case 3: i = regA != 0 ? valueLit - 2 : i; break;
                    case 4: regB ^= regC; break;
                    case 5:
                        if (sb != null) {
                            sb.Append($"{valueCombo & 7},");
                        } else if (program[index] != (valueCombo & 7)) {
                            return false;
                        }
                        index++;
                        break;
                    case 6: regB = regA >> (int)valueCombo; break;
                    case 7: regC = regA >> (int)valueCombo; break;
                }
            }
            regA = temp; regB = b; regC = c;
            return index == program.Length;
        }
    }
}