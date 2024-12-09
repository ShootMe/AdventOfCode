using AdventOfCode.Core;
using System.Collections.Generic;
using System.ComponentModel;
namespace AdventOfCode.Y2024 {
    [Description("Disk Fragmenter")]
    public class Puzzle09 : ASolver {
        private int[] blocks;
        private List<Block> blocksList = new();

        public override void Setup() {
            int total = 0;
            for (int i = 0; i < Input.Length; i++) {
                int num = Input[i] - 0x30;
                total += num;
            }

            blocks = new int[total];
            int id = 1;
            int index = 0;
            for (int i = 0; i < Input.Length; i++) {
                int num = Input[i] - 0x30;
                if ((i & 1) == 0) {
                    for (int j = 0; j < num; j++) {
                        blocks[index++] = id;
                    }
                    blocksList.Add(new Block() { ID = id, Size = num });
                    id++;
                } else if (num > 0) {
                    blocksList.Add(new Block() { Size = num });
                    index += num;
                }
            }
        }

        [Description("What is the resulting filesystem checksum?")]
        public override string SolvePart1() {
            int minIndex = 0;
            int maxIndex = blocks.Length - 1;

            while (minIndex < maxIndex) {
                while (minIndex < maxIndex) {
                    if (blocks[minIndex] == 0) { break; }
                    minIndex++;
                }
                while (maxIndex > minIndex) {
                    if (blocks[maxIndex] != 0) { break; }
                    maxIndex--;
                }
                if (minIndex < maxIndex) {
                    blocks[minIndex] = blocks[maxIndex];
                    blocks[maxIndex] = 0;
                }
            }

            long total = 0;
            for (int i = 0; i < blocks.Length; i++) {
                int id = blocks[i];
                if (id == 0) { break; }
                total += (id - 1) * i;
            }

            return $"{total}";
        }

        [Description("What is the resulting filesystem checksum?")]
        public override string SolvePart2() {
            int fileID = blocksList[^1].ID;
            for (int j = blocksList.Count - 1; j > 0; j--) {
                Block block = blocksList[j];
                if (block.ID != fileID) { continue; }
                fileID--;

                for (int i = 0; i < j; i++) {
                    Block freeBlock = blocksList[i];
                    if (freeBlock.ID != 0 || block.Size > freeBlock.Size) { continue; }

                    blocksList[i] = block;
                    blocksList[j] = freeBlock;
                    if (block.Size != freeBlock.Size) {
                        blocksList.Insert(i + 1, new Block() { Size = freeBlock.Size - block.Size });
                        freeBlock.Size = block.Size;
                        j++;
                    }
                    if (j + 1 < blocksList.Count && blocksList[j + 1].ID == 0) {
                        freeBlock.Size += blocksList[j + 1].Size;
                        blocksList.RemoveAt(j + 1);
                    }
                    if (blocksList[j - 1].ID == 0) {
                        blocksList[j - 1].Size += freeBlock.Size;
                        blocksList.RemoveAt(j);
                    }
                    break;
                }
            }

            long total = 0;
            int index = 0;
            for (int i = 0; i < blocksList.Count; i++) {
                Block block = blocksList[i];
                if (block.ID != 0) {
                    total += (block.ID - 1) * ((long)index * block.Size + (long)block.Size * (block.Size - 1) / 2);
                }
                index += block.Size;
            }

            return $"{total}";
        }

        private class Block {
            public int ID, Size;
            public override string ToString() {
                if (ID == 0) {
                    return $"Free Size: {Size}";
                }
                return $"ID: {ID - 1} Size: {Size}";
            }
        }
    }
}