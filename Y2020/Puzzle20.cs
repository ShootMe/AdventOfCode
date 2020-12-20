using AdventOfCode.Common;
using AdventOfCode.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
namespace AdventOfCode.Y2020 {
    public class Puzzle20 : ASolver {
        private List<Tile> tiles;
        private int imageSize;
        private Tile[] image;
        public Puzzle20(string input) : base(input) { Name = "Jurassic Jigsaw"; }

        public override void Setup() {
            List<string> items = Tools.GetSections(Input);
            tiles = new List<Tile>();

            for (int i = 0; i < items.Count; i++) {
                string[] lines = items[i].Split('\n');
                int id = Tools.ParseInt(lines[0], 5, 4);

                Tile tile = new Tile() { ID = id, Size = lines[1].Length, Variation = 0 };
                tile.Ordering = new bool[tile.Size * tile.Size];
                int index = 0;
                for (int j = 1; j < lines.Length; j++) {
                    string row = lines[j];
                    for (int k = 0; k < row.Length; k++) {
                        tile.Ordering[index++] = row[k] == '#';
                    }
                }

                tiles.Add(tile);
            }

            imageSize = (int)Math.Sqrt(tiles.Count);
        }

        [Description("What do you get if you multiply together the IDs of the four corner tiles?")]
        public override string SolvePart1() {
            image = new Tile[imageSize * imageSize];
            List<Tile> choices = new List<Tile>(tiles);
            FindTiling(choices, image, 0);
            return $"{(long)image[0].ID * image[imageSize - 1].ID * image[(imageSize - 1) * imageSize].ID * image[image.Length - 1].ID}";
        }

        [Description("How many # are not part of a sea monster?")]
        public override string SolvePart2() {
            int tileSize = image[0].Size - 2;
            Tile sea = new Tile() { ID = 0, Size = tileSize * imageSize, Variation = 0 };
            sea.Ordering = new bool[sea.Size * sea.Size];

            int index = 0;
            int x = 0;
            for (int i = 0; i < image.Length; i++) {
                Tile tile = image[i];
                tile.AddTo(sea, index);
                index += tileSize;
                x++;
                if (x == imageSize) {
                    x = 0;
                    index += sea.Size * (tileSize - 1);
                }
            }

            int monsters = 0;
            for (int i = 0; i < 8; i++) {
                sea.Variation = i;

                bool[] row1 = sea.GetTop(0);
                bool[] row2 = sea.GetTop(1);
                bool[] row3 = sea.GetTop(2);
                int j = 3;
                do {
                    int k = 19;
                    do {
                        if (row1[k - 1]
                            && row2[k] && row2[k - 1] && row2[k - 2] && row2[k - 7] && row2[k - 8] && row2[k - 13] && row2[k - 14] && row2[k - 19]
                            && row3[k - 3] && row3[k - 6] && row3[k - 9] && row3[k - 12] && row3[k - 15] && row3[k - 18]) {
                            monsters++;
                        }
                        k++;
                    } while (k < sea.Size);

                    if (j == sea.Size) { break; }
                    row1 = row2;
                    row2 = row3;
                    row3 = sea.GetTop(j++);
                } while (true);

                if (monsters > 0) {
                    int waters = 0;
                    for (j = 0; j < sea.Ordering.Length; j++) {
                        if (sea.Ordering[j]) {
                            waters++;
                        }
                    }
                    return $"{waters - monsters * 15}";
                }
            }
            return string.Empty;
        }

        private bool FindTiling(List<Tile> choices, Tile[] image, int index) {
            int x = index % imageSize;
            int y = index / imageSize;
            Tile left = x > 0 ? image[index - 1] : null;
            Tile top = y > 0 ? image[index - imageSize] : null;

            for (int i = 0; i < choices.Count; i++) {
                Tile choice = choices[i];

                for (int j = 0; j < 8; j++) {
                    choice.Variation = j;
                    if ((left == null || choice.ValidLeft(left)) && (top == null || choice.ValidTop(top))) {
                        choices.RemoveAt(i);
                        image[index] = choice;

                        if (index + 1 == image.Length || FindTiling(choices, image, index + 1)) {
                            return true;
                        }

                        image[index] = null;
                        choices.Insert(i, choice);
                    }
                }
            }

            return false;
        }

        private class Tile {
            public int ID;
            public int Size;
            public int Variation;
            public bool[] Ordering;

            public bool ValidLeft(Tile tile) {
                bool[] left = GetLeft();
                bool[] right = tile.GetRight();
                return ArrayComparer<bool>.Comparer.Equals(right, left);
            }
            public bool ValidTop(Tile tile) {
                bool[] top = GetTop();
                bool[] bottom = tile.GetBottom();
                return ArrayComparer<bool>.Comparer.Equals(top, bottom);
            }
            public override string ToString() {
                return $"{ID} {Variation}";
            }
            public bool[] GetRight(int column = 0) {
                return GetRight(Variation, column);
            }
            public void AddTo(Tile tile, int index) {
                for (int i = 1; i + 1 < Size; i++) {
                    bool[] row = GetTop(i);
                    for (int j = 1; j + 1 < row.Length; j++) {
                        tile.Ordering[index++] = row[j];
                    }
                    index += tile.Size - row.Length + 2;
                }
            }
            private bool[] GetRight(int variation, int column = 0) {
                bool[] result = new bool[Size];
                int offset;
                switch (variation) {
                    case 0:
                        offset = Size - column - 1;
                        for (int i = 0; i < Size; i++) {
                            result[i] = Ordering[offset + i * Size];
                        }
                        break;
                    case 1:
                        offset = Size * Size - column - 1;
                        for (int i = 0; i < Size; i++) {
                            result[i] = Ordering[offset - i * Size];
                        }
                        break;
                    case 2:
                        offset = Size * (Size - column) - 1;
                        for (int i = 0; i < Size; i++) {
                            result[i] = Ordering[offset - i];
                        }
                        break;
                    case 3:
                        offset = Size * (Size - column - 1);
                        for (int i = 0; i < Size; i++) {
                            result[i] = Ordering[offset + i];
                        }
                        break;
                    case 4:
                        offset = column;
                        for (int i = 0; i < Size; i++) {
                            result[i] = Ordering[offset + i * Size];
                        }
                        break;
                    case 5:
                        offset = Size * (Size - 1) + column;
                        for (int i = 0; i < Size; i++) {
                            result[i] = Ordering[offset - i * Size];
                        }
                        break;
                    case 6:
                        offset = (Size * (column + 1)) - 1;
                        for (int i = 0; i < Size; i++) {
                            result[i] = Ordering[offset - i];
                        }
                        break;
                    case 7:
                        offset = Size * column;
                        for (int i = 0; i < Size; i++) {
                            result[i] = Ordering[offset + i];
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
        }
    }
}