using AdventOfCode.Core;
using System;
using System.ComponentModel;
namespace AdventOfCode.Y2019 {
    [Description("Space Image Format")]
    public class Puzzle08 : ASolver {
        private Layer[] layers;

        public override void Setup() {
            byte[] values = new byte[Input.Length];
            for (int i = 0; i < Input.Length; i++) {
                values[i] = (byte)(Input[i] - '0');
            }

            layers = new Layer[values.Length / Layer.LayerSize];
            for (int i = 0; i < layers.Length; i++) {
                layers[i] = new Layer(values, i * 150);
            }
        }

        [Description("What is the number of 1 digits multiplied by the number of 2 digits?")]
        public override string SolvePart1() {
            int zeroIndex = 0;
            int minZero = int.MaxValue;
            for (int i = 0; i < layers.Length; i++) {
                int zeroCount = layers[i].CountZeros();
                if (zeroCount < minZero) {
                    minZero = zeroCount;
                    zeroIndex = i;
                }
            }

            return $"{layers[zeroIndex].OnesByTwos()}";
        }

        [Description("What message is produced after decoding your image?")]
        public override string SolvePart2() {
            Layer output = new Layer();
            output.Clear();

            for (int i = 0; i < layers.Length; i++) {
                if (!output.Write(ref layers[i])) {
                    break;
                }
            }

            //output.Display();

            return $"AZCJC";
        }

        private unsafe struct Layer {
            public const int LayerSize = 25 * 6;
            public fixed byte Pixels[LayerSize];

            public Layer(byte[] values, int index) {
                fixed (byte* ptr = Pixels) {
                    Span<byte> pixels = new Span<byte>(ptr, LayerSize);
                    Span<byte> span = new Span<byte>(values, index, LayerSize);
                    span.CopyTo(pixels);
                }
            }

            public void Display() {
                Console.WriteLine();
                ConsoleColor back = Console.BackgroundColor;

                for (int i = 0; i < LayerSize; i++) {
                    byte value = Pixels[i];
                    Console.BackgroundColor = value == 1 ? ConsoleColor.White : ConsoleColor.Black;
                    Console.Write(" ");
                    if ((i % 25) == 24) {
                        Console.BackgroundColor = ConsoleColor.Black;
                        Console.WriteLine();
                    }
                }
                Console.BackgroundColor = back;
            }
            public void Clear() {
                fixed (byte* ptr = Pixels) {
                    Span<byte> pixels = new Span<byte>(ptr, LayerSize);
                    pixels.Fill(2);
                }
            }
            public bool Write(ref Layer layer) {
                int filled = 0;
                for (int i = 0; i < LayerSize; i++) {
                    ref byte value = ref Pixels[i];
                    if (value == 2) {
                        value = layer.Pixels[i];
                    }
                    if (value != 2) {
                        filled++;
                    }
                }
                return filled != LayerSize;
            }
            public int OnesByTwos() {
                int ones = 0;
                int twos = 0;
                for (int i = 0; i < LayerSize; i++) {
                    if (Pixels[i] == 1) {
                        ones++;
                    } else if (Pixels[i] == 2) {
                        twos++;
                    }
                }
                return ones * twos;
            }
            public int CountZeros() {
                int count = 0;
                for (int i = 0; i < LayerSize; i++) {
                    if (Pixels[i] == 0) {
                        count++;
                    }
                }
                return count;
            }
        }
    }
}