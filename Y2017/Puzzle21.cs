using AdventOfCode.Common;
using AdventOfCode.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
namespace AdventOfCode.Y2017 {
    [Description("Fractal Art")]
    public class Puzzle21 : ASolver {
        private Dictionary<Tile, Tile> enhancements;
        private string[] startTile = new string[] { ".#.", "..#", "###" };

        public override void Setup() {
            List<string> items = Tools.GetLines(Input);
            enhancements = new Dictionary<Tile, Tile>();

            for (int i = 0; i < items.Count; i++) {
                string item = items[i];

                int index = item.IndexOf('=');
                string[] input = item.Substring(0, index - 1).Split('/');
                Tile inTile = new Tile(input);

                string[] output = item.Substring(index + 3).Split('/');
                Tile outTile = new Tile(output);

                enhancements.Add(inTile, outTile);
            }
        }

        [Description("How many pixels stay on after 5 iterations?")]
        public override string SolvePart1() {
            Tile image = new Tile(startTile);
            for (int i = 0; i < 5; i++) {
                image = new Tile(image, enhancements);
            }

            return $"{image.CountActive()}";
        }

        [Description("How many pixels stay on after 18 iterations?")]
        public override string SolvePart2() {
            Tile image = new Tile(startTile);
            for (int i = 0; i < 18; i++) {
                image = new Tile(image, enhancements);
            }

            return $"{image.CountActive()}";
        }

        private class Tile : IEquatable<Tile> {
            public int Size;
            public int Variation;
            public bool[] Data;

            public Tile(bool[] data, int size) {
                Data = data;
                Size = size;
                Variation = 0;
            }
            public Tile(Tile previous, Dictionary<Tile, Tile> enhancements) {
                int subSize;
                int subLength;
                int tileCount;
                if ((previous.Size & 1) == 0) {
                    subSize = 2;
                    subLength = 4;
                    tileCount = previous.Size / 2;
                    Size = tileCount * 3;
                } else {
                    subSize = 3;
                    subLength = 9;
                    tileCount = previous.Size / 3;
                    Size = tileCount * 4;
                }

                Data = new bool[Size * Size];

                int index = 0;
                int tileIndex = 0;
                int added = 0;
                while (tileIndex < Data.Length) {
                    bool[] subImage = new bool[subLength];
                    for (int i = 0, j = 0; i < subLength; i++) {
                        subImage[i] = previous.Data[index++];
                        j++;
                        if (j == subSize) {
                            j = 0;
                            index += previous.Size - subSize;
                        }
                    }
                    index -= previous.Size * subSize - subSize;

                    Tile sub = new Tile(subImage, subSize);
                    Tile newSub = enhancements[sub];
                    newSub.AddTo(this, tileIndex);
                    tileIndex += newSub.Size;
                    added++;
                    if (added == tileCount) {
                        added = 0;
                        index += previous.Size * (subSize - 1);
                        tileIndex += newSub.Size * (newSub.Size - 1) * tileCount;
                    }
                }
            }
            public Tile(string[] rows) {
                Size = rows.Length;
                Variation = 0;
                Data = new bool[Size * Size];
                for (int i = 0, k = 0; i < Size; i++) {
                    string row = rows[i];

                    for (int j = 0; j < Size; j++) {
                        Data[k++] = row[j] == '#';
                    }
                }
            }
            public int CountActive() {
                int count = 0;
                for (int i = 0; i < Data.Length; i++) {
                    if (Data[i]) { count++; }
                }
                return count;
            }
            public override string ToString() {
                char[] rows = new char[Data.Length + Size - 1];
                for (int i = 0, k = 0; i < Size; i++) {
                    if (i > 0) { rows[k++] = '/'; }
                    bool[] row = GetTop(i);

                    for (int j = 0; j < Size; j++) {
                        rows[k++] = row[j] ? '#' : '.';
                    }
                }

                return new string(rows);
            }
            public bool[] GetRight(int column = 0) {
                return GetRight(Variation, column);
            }
            public void AddTo(Tile tile, int index) {
                for (int i = 0; i < Size; i++) {
                    bool[] row = GetTop(i);
                    for (int j = 0; j < row.Length; j++) {
                        tile.Data[index++] = row[j];
                    }
                    index += tile.Size - row.Length;
                }
            }
            private bool[] GetRight(int variation, int column = 0) {
                bool[] result = new bool[Size];
                int offset;
                switch (variation) {
                    case 0:
                        offset = Size - column - 1;
                        for (int i = 0; i < Size; i++) {
                            result[i] = Data[offset + i * Size];
                        }
                        break;
                    case 1:
                        offset = Size * Size - column - 1;
                        for (int i = 0; i < Size; i++) {
                            result[i] = Data[offset - i * Size];
                        }
                        break;
                    case 2:
                        offset = Size * (Size - column) - 1;
                        for (int i = 0; i < Size; i++) {
                            result[i] = Data[offset - i];
                        }
                        break;
                    case 3:
                        offset = Size * (Size - column - 1);
                        for (int i = 0; i < Size; i++) {
                            result[i] = Data[offset + i];
                        }
                        break;
                    case 4:
                        offset = column;
                        for (int i = 0; i < Size; i++) {
                            result[i] = Data[offset + i * Size];
                        }
                        break;
                    case 5:
                        offset = Size * (Size - 1) + column;
                        for (int i = 0; i < Size; i++) {
                            result[i] = Data[offset - i * Size];
                        }
                        break;
                    case 6:
                        offset = (Size * (column + 1)) - 1;
                        for (int i = 0; i < Size; i++) {
                            result[i] = Data[offset - i];
                        }
                        break;
                    case 7:
                        offset = Size * column;
                        for (int i = 0; i < Size; i++) {
                            result[i] = Data[offset + i];
                        }
                        break;
                }
                return result;
            }
            public bool[] GetLeft(int column = 0) {
                switch (Variation) {
                    case 0: return GetRight(4, column);
                    case 1: return GetRight(5, column);
                    case 2: return GetRight(6, column);
                    case 3: return GetRight(7, column);
                    case 4: return GetRight(0, column);
                    case 5: return GetRight(1, column);
                    case 6: return GetRight(2, column);
                    case 7: return GetRight(3, column);
                }
                return null;
            }
            public bool[] GetTop(int row = 0) {
                switch (Variation) {
                    case 0: return GetRight(7, row);
                    case 1: return GetRight(3, row);
                    case 2: return GetRight(0, row);
                    case 3: return GetRight(4, row);
                    case 4: return GetRight(6, row);
                    case 5: return GetRight(2, row);
                    case 6: return GetRight(1, row);
                    case 7: return GetRight(5, row);
                }
                return null;
            }
            public bool[] GetBottom(int row = 0) {
                switch (Variation) {
                    case 0: return GetRight(3, row);
                    case 1: return GetRight(7, row);
                    case 2: return GetRight(4, row);
                    case 3: return GetRight(0, row);
                    case 4: return GetRight(2, row);
                    case 5: return GetRight(6, row);
                    case 6: return GetRight(5, row);
                    case 7: return GetRight(1, row);
                }
                return null;
            }
            public bool Equals(Tile other) {
                if (Size != other.Size) { return false; }

                int variation = Variation;
                for (int i = 0; i < 8; i++) {
                    Variation = i;

                    bool good = true;
                    for (int j = 0; j < Size; j++) {
                        bool[] top1 = GetTop(j);
                        bool[] top2 = other.GetTop(j);
                        if (!ArrayComparer<bool>.Comparer.Equals(top1, top2)) {
                            good = false;
                            break;
                        }
                    }

                    if (good) {
                        Variation = variation;
                        return true;
                    }
                }

                Variation = variation;
                return false;
            }
            public override bool Equals(object obj) {
                return obj is Tile tile && Equals(tile);
            }
            public override int GetHashCode() {
                return Size + CountActive();
            }
        }
    }
}