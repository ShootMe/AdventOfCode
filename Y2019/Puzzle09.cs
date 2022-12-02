using AdventOfCode.Common;
using AdventOfCode.Core;
using System.ComponentModel;
namespace AdventOfCode.Y2019 {
    [Description("Sensor Boost")]
    public class Puzzle09 : ASolver {
        private IntCode program;

        public override void Setup() {
            program = new IntCode(Input.ToLongs(','));
        }

        [Description("What BOOST keycode does it produce?")]
        public override string SolvePart1() {
            program.Reset();
            program.Run(1);
            return $"{program.Output}";
        }

        [Description("What are the coordinates of the distress signal?")]
        public override string SolvePart2() {
            program.Reset();
            program.Run(2);
            return $"{program.Output}";
        }
    }
}