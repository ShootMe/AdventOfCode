using AdventOfCode.Common;
using AdventOfCode.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
namespace AdventOfCode.Y2022 {
    [Description("Pyroclastic Flow")]
    public class Puzzle17 : ASolver {
        private List<RockRow> rocks = new();

        [Description("How many units tall will the tower of rocks be after 2022 rocks have stopped falling?")]
        public override string SolvePart1() {
            RockType current = RockType.Minus;
            int placed = 0;
            int index = 0;
            while (placed < 2022) {
                index = Simulate(current, index);
                placed++;
                current = current switch { RockType.Minus => RockType.Plus, RockType.Plus => RockType.RevL, RockType.RevL => RockType.Pipe, RockType.Pipe => RockType.Square, _ => RockType.Minus };
            }
            return $"{FilledRows()}";
        }

        [Description("How tall will the tower be after 1000000000000 rocks have stopped?")]
        public override string SolvePart2() {
            rocks.Clear();
            RockType current = RockType.Minus;
            int placed = 0;
            int index = 0;
            List<(int position, int count)> positions = new();
            int maxLength = 0;
            while (true) {
                index = Simulate(current, index);
                placed++;
                current = current switch { RockType.Minus => RockType.Plus, RockType.Plus => RockType.RevL, RockType.RevL => RockType.Pipe, RockType.Pipe => RockType.Square, _ => RockType.Minus };
                if (current == RockType.Minus) {
                    positions.Add((index, FilledRows()));
                    if (positions.Count > 5) {
                        (int position, int length) = FindLongestDuplicate(positions);
                        if (position <= 0) { continue; }

                        if (length > maxLength) {
                            maxLength = length;
                        } else {
                            int baseCount = positions[position].count;
                            int duplicateCount = positions[position + length].count;
                            int difference = duplicateCount - baseCount;
                            int basePlaced = position * 5;
                            int duplicatePlaced = length * 5;
                            long repeats = (1000000000000L - basePlaced) / duplicatePlaced;
                            int leftOver = (int)(1000000000000L - basePlaced - repeats * duplicatePlaced);
                            if (leftOver != 0) {
                                baseCount = positions[position + leftOver / 5 - 1].count;
                            } else {
                                baseCount = positions[position - 1].count;
                            }
                            return $"{baseCount + repeats * difference}";
                        }
                    }
                }
            }
        }
        private (int index, int length) FindLongestDuplicate(List<(int value, int count)> positions) {
            int n = positions.Count;
            int[,] data = new int[n + 1, n + 1];

            int length = 0;
            int index = 0;
            for (int i = 1; i <= n; i++) {
                for (int j = i + 1; j <= n; j++) {
                    if (positions[i - 1].value == positions[j - 1].value && data[i - 1, j - 1] < (j - i)) {
                        data[i, j] = data[i - 1, j - 1] + 1;

                        if (data[i, j] > length) {
                            length = data[i, j];
                            index = Math.Max(i, index);
                        }
                    } else {
                        data[i, j] = 0;
                    }
                }
            }

            return (length > 0 ? index - length : -1, length);
        }
        private int Simulate(RockType rock, int windIndex) {
            int row = FilledRows();
            row += rock switch { RockType.Minus => 3, RockType.Plus => 5, RockType.RevL => 5, RockType.Pipe => 6, _ => 4 };
            while (rocks.Count <= row) {
                rocks.Add(new RockRow());
            }

            int offset = 0;
            while (true) {
                if (windIndex >= Input.Length) { windIndex = 0; }

                char wind = Input[windIndex++];
                offset += wind switch { '<' => -1, _ => 1 };
                if (!CanPlace(rock, row, offset)) {
                    offset += wind switch { '<' => 1, _ => -1 };
                }
                row--;
                if (!CanPlace(rock, row, offset)) {
                    Place(rock, row + 1, offset);
                    break;
                }
            }
            //Display();
            if (windIndex >= Input.Length) { windIndex = 0; }
            return windIndex;
        }
        private int FilledRows() {
            int row = rocks.Count - 1;
            while (row >= 0 && rocks[row] != 0) {
                row--;
            }
            return row + 1;
        }
        private void Place(RockType rock, int row, int offset) {
            offset += 2;
            switch (rock) {
                case RockType.Minus:
                    rocks[row] |= (byte)(0b1111 << offset);
                    break;
                case RockType.Plus:
                    rocks[row] |= (byte)(0b010 << offset);
                    rocks[row - 1] |= (byte)(0b111 << offset);
                    rocks[row - 2] |= (byte)(0b010 << offset);
                    break;
                case RockType.RevL:
                    rocks[row] |= (byte)(0b100 << offset);
                    rocks[row - 1] |= (byte)(0b100 << offset);
                    rocks[row - 2] |= (byte)(0b111 << offset);
                    break;
                case RockType.Pipe:
                    rocks[row] |= (byte)(0b1 << offset);
                    rocks[row - 1] |= (byte)(0b1 << offset);
                    rocks[row - 2] |= (byte)(0b1 << offset);
                    rocks[row - 3] |= (byte)(0b1 << offset);
                    break;
                case RockType.Square:
                    rocks[row] |= (byte)(0b11 << offset);
                    rocks[row - 1] |= (byte)(0b11 << offset);
                    break;
            }
        }
        private bool CanPlace(RockType rock, int row, int offset) {
            if (row < 0) { return false; }
            offset += 2;
            RockRow rockrow = rocks[row];
            switch (rock) {
                case RockType.Minus:
                    return offset <= 3 && offset >= 0 &&
                        (rockrow.Value & (0b1111 << offset)) == 0;
                case RockType.Plus:
                    return offset <= 4 && offset >= 0 && row > 1 &&
                        (rockrow.Value & (0b10 << offset)) == 0 &&
                        (rocks[row - 1].Value & (0b111 << offset)) == 0 &&
                        (rocks[row - 2].Value & (0b10 << offset)) == 0;
                case RockType.RevL:
                    return offset <= 4 && offset >= 0 && row > 1 &&
                        (rockrow.Value & (0b100 << offset)) == 0 &&
                        (rocks[row - 1].Value & (0b100 << offset)) == 0 &&
                        (rocks[row - 2].Value & (0b111 << offset)) == 0;
                case RockType.Pipe:
                    return offset <= 6 && offset >= 0 && row > 2 &&
                        (rockrow.Value & (0b1 << offset)) == 0 &&
                        (rocks[row - 1].Value & (0b1 << offset)) == 0 &&
                        (rocks[row - 2].Value & (0b1 << offset)) == 0 &&
                        (rocks[row - 3].Value & (0b1 << offset)) == 0;
                case RockType.Square:
                    return offset <= 5 && offset >= 0 && row > 0 &&
                        (rockrow.Value & (0b11 << offset)) == 0 &&
                        (rocks[row - 1].Value & (0b11 << offset)) == 0;
            }
            return false;
        }
        private void Display() {
            Console.WriteLine();
            for (int i = rocks.Count - 1; i >= 0; i--) {
                Console.WriteLine($"{rocks[i]}");
            }
        }
        private struct RockRow {
            public byte Value;
            public RockRow() { }
            public static implicit operator RockRow(byte value) {
                return new RockRow() { Value = value };
            }
            public static implicit operator byte(RockRow row) {
                return row.Value;
            }
            public override string ToString() {
                return $"{Val(0)}{Val(1)}{Val(2)}{Val(3)}{Val(4)}{Val(5)}{Val(6)}";
            }
            private char Val(int index) {
                return (Value & (1 << index)) != 0 ? '#' : '.';
            }
        }
        private enum RockType : byte {
            Minus,
            Plus,
            RevL,
            Pipe,
            Square
        }
    }
}