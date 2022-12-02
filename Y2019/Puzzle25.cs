using AdventOfCode.Common;
using AdventOfCode.Core;
using System.ComponentModel;
using System.Text;
namespace AdventOfCode.Y2019 {
    [Description("Cryostasis")]
    public class Puzzle25 : ASolver {
        private IntCode program;

        public override void Setup() {
            program = new IntCode(Input.ToLongs(','));
        }

        [Description("What is the password for the main airlock?")]
        public override string SolvePart1() {
            //while (true) {
            //    while (program.Run() && !program.InputRequired) {
            //        Console.Write((char)program.Output);
            //    }

            //    string line = $"{Console.ReadLine()}\n";
            //    for (int i = 0; i < line.Length; i++) {
            //        program.WriteInput(line[i]);
            //    }
            //}

            //N = Escape Pod = Fail
            //NEE = Molten Lava = Fail
            //SWNN = Infinite Loop = Fail
            //EE = Photons = Fail
            //EEE = Giant Electromagnet = Fail

            //NE = Coin = Too Heavy
            //SW = Sand = Too Heavy
            //E = Cake = Too Heavy

            //ESWW = Jam = Not Correct

            //S = Food Ration = Good
            //SWNNE = Astrolabe = Good
            //ES = Weather Machine = Good
            //ESW = Ornament = Good

            string solution =
@"south
take food ration
west
north
north
east
take astrolabe
west
south
south
east
north
east
south
take weather machine
west
take ornament
east
north
east
east
east
south
".Replace("\r", string.Empty);

            for (int i = 0; i < solution.Length; i++) {
                program.WriteInput(solution[i]);
            }

            StringBuilder sb = new StringBuilder();
            while (program.Run()) {
                sb.Append((char)program.Output);
            }

            string answer = sb.ToString();
            int index = answer.IndexOf(" typing ");
            int endIndex = answer.IndexOf(' ', index + 8);
            return answer.Substring(index + 8, endIndex - index - 8);
        }
    }
}