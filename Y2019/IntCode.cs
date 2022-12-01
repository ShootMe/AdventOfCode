using System;
using System.Collections.Generic;
namespace AdventOfCode.Y2019 {
    public class IntCode {
        private readonly long[] program;
        private readonly long[] state;
        private long instruction;
        private long relativeBase;
        private Dictionary<long, long> memory;
        public long Output { get; private set; }
        public bool InputRequired { get; private set; }
        public bool HasOutput { get; set; }
        private Queue<long> inputs = new Queue<long>();

        public IntCode(int[] programToRun) {
            program = new long[programToRun.Length];
            for (int i = 0; i < program.Length; i++) {
                program[i] = programToRun[i];
            }
            state = new long[program.Length];
            memory = new Dictionary<long, long>();
            Reset();
        }
        public IntCode(long[] programToRun) {
            program = new long[programToRun.Length];
            for (int i = 0; i < program.Length; i++) {
                program[i] = programToRun[i];
            }
            state = new long[program.Length];
            memory = new Dictionary<long, long>();
            Reset();
        }
        public IntCode Clone() {
            IntCode result = new IntCode(program);
            for (int i = 0; i < program.Length; i++) {
                result.state[i] = state[i];
            }
            result.instruction = instruction;
            result.relativeBase = relativeBase;
            result.Output = Output;
            result.InputRequired = InputRequired;
            foreach (long value in inputs) {
                result.inputs.Enqueue(value);
            }
            return result;
        }
        public void WriteInput(long inputValue) {
            inputs.Enqueue(inputValue);
        }
        public long this[int index] {
            get { return state[index]; }
            set { state[index] = value; }
        }
        public void Reset() {
            Array.Copy(program, state, program.Length);
            inputs.Clear();
            instruction = 0;
            relativeBase = 0;
            memory.Clear();
        }
        public bool Run(long inputValue) {
            inputs.Enqueue(inputValue);
            return Run();
        }
        public bool Run() {
            InputRequired = false;
            HasOutput = false;
            long code = state[instruction];

            do {
                long mode1 = (code / 100) % 10;
                long mode2 = (code / 1000) % 10;
                long mode3 = (code / 10000) % 10;

                long index1 = (instruction + 1 < state.Length ? state[instruction + 1] : 0) + (mode1 == 2 ? relativeBase : 0);
                long index2 = (instruction + 2 < state.Length ? state[instruction + 2] : 0) + (mode2 == 2 ? relativeBase : 0);
                long index3 = (instruction + 3 < state.Length ? state[instruction + 3] : 0) + (mode3 == 2 ? relativeBase : 0);

                long value1 = mode1 == 1 ? index1 : GetValue(index1);
                long value2 = mode2 == 1 ? index2 : GetValue(index2);

                int opcode = (int)(code % 100);
                switch (opcode) {
                    case 1: SetValue(index3, value1 + value2); instruction += 4; break;
                    case 2: SetValue(index3, value1 * value2); instruction += 4; break;
                    case 3: if (inputs.Count == 0) { InputRequired = true; return true; } long inputValue = inputs.Dequeue(); SetValue(index1, inputValue); instruction += 2; break;
                    case 4: Output = value1; instruction += 2; HasOutput = true; return true;
                    case 5: instruction = value1 != 0 ? value2 : instruction + 3; break;
                    case 6: instruction = value1 == 0 ? value2 : instruction + 3; break;
                    case 7: SetValue(index3, value1 < value2 ? 1 : 0); instruction += 4; break;
                    case 8: SetValue(index3, value1 == value2 ? 1 : 0); instruction += 4; break;
                    case 9: relativeBase += value1; instruction += 2; break;
                    case 99: return false;
                    default: return false;
                }

                code = state[instruction];
            } while (true);
        }
        public long GetValue(long index) {
            if (index < 0) { return 0; }

            if (index >= state.Length) {
                long result;
                if (!memory.TryGetValue(index, out result)) {
                    memory.Add(index, 0);
                }
                return result;
            }

            return state[index];
        }
        public void SetValue(long index, long value) {
            if (index < 0) { return; }

            if (index >= state.Length) {
                if (!memory.ContainsKey(index)) {
                    memory.Add(index, value);
                } else {
                    memory[index] = value;
                }
            } else {
                state[index] = value;
            }
        }
    }
}