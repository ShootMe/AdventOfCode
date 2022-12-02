using AdventOfCode.Common;
using AdventOfCode.Core;
using System.ComponentModel;
namespace AdventOfCode.Y2019 {
    [Description("Amplification Circuit")]
    public class Puzzle07 : ASolver {
        private IntCode amp1, amp2, amp3, amp4, amp5;

        public override void Setup() {
            long[] code = Input.ToLongs(',');
            amp1 = new IntCode(code);
            amp2 = new IntCode(code);
            amp3 = new IntCode(code);
            amp4 = new IntCode(code);
            amp5 = new IntCode(code);
        }

        [Description("What is the highest signal that can be sent to the thrusters?")]
        public override string SolvePart1() {
            int best = int.MinValue;
            foreach (int[] seq in (new int[] { 0, 1, 2, 3, 4 }).Permute()) {
                amp1.Reset();
                amp1.WriteInput(seq[0]);
                amp1.Run(0);

                amp2.Reset();
                amp2.WriteInput(seq[1]);
                amp2.Run(amp1.Output);

                amp3.Reset();
                amp3.WriteInput(seq[2]);
                amp3.Run(amp2.Output);

                amp4.Reset();
                amp4.WriteInput(seq[3]);
                amp4.Run(amp3.Output);

                amp5.Reset();
                amp5.WriteInput(seq[4]);
                amp5.Run(amp4.Output);

                if (amp5.Output > best) {
                    best = (int)amp5.Output;
                }
            }

            return $"{best}";
        }

        [Description("What is the largest output signal that can be sent to the thrusters?")]
        public override string SolvePart2() {
            int best = int.MinValue;
            foreach (int[] seq in (new int[] { 5, 6, 7, 8, 9 }).Permute()) {
                amp1.Reset();
                amp2.Reset();
                amp3.Reset();
                amp4.Reset();
                amp5.Reset();

                amp1.WriteInput(seq[0]);
                amp1.Run(0);
                amp2.WriteInput(seq[1]);
                amp2.Run(amp1.Output);
                amp3.WriteInput(seq[2]);
                amp3.Run(amp2.Output);
                amp4.WriteInput(seq[3]);
                amp4.Run(amp3.Output);
                amp5.WriteInput(seq[4]);
                amp5.Run(amp4.Output);

                while (amp1.Run(amp5.Output)) {
                    amp2.Run(amp1.Output);
                    amp3.Run(amp2.Output);
                    amp4.Run(amp3.Output);
                    amp5.Run(amp4.Output);
                }

                if (amp5.Output > best) {
                    best = (int)amp5.Output;
                }
            }

            return $"{best}";
        }
    }
}