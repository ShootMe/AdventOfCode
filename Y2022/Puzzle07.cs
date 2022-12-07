using AdventOfCode.Common;
using AdventOfCode.Core;
using System.Collections.Generic;
using System.ComponentModel;
namespace AdventOfCode.Y2022 {
    [Description("No Space Left On Device")]
    public class Puzzle07 : ASolver {
        private Directory root = new(string.Empty);
        private List<Directory> dirs = new();

        public override void Setup() {
            dirs.Add(root);
            Directory current = root;

            string[] lines = Input.Split('\n');
            for (int i = 0; i < lines.Length; i++) {
                string line = lines[i];

                if (line[0] == '$') {
                    string[] cmds = line.Substring(2).Split(' ');
                    switch (cmds[0]) {
                        case "cd":
                            current = cmds[1] switch {
                                ".." => current.Parent,
                                "/" => root,
                                _ => current.Children[cmds[1]]
                            };
                            break;
                        case "ls":
                            while (++i < lines.Length) {
                                line = lines[i];
                                if (line[0] == '$') {
                                    i--;
                                    break;
                                }

                                string[] list = line.Split(' ');
                                switch (list[0]) {
                                    case "dir":
                                        Directory directory = new Directory(list[1]) { Parent = current };
                                        current.Children.Add(directory.Name, directory);
                                        dirs.Add(directory);
                                        break;
                                    default:
                                        File file = new File() { Name = list[1], Size = list[0].ToInt() };
                                        current.Files.Add(file.Name, file);
                                        break;
                                }
                            }
                            break;
                    }
                }
            }
        }

        [Description("What is the sum of the total sizes of those directories?")]
        public override string SolvePart1() {
            int total = 0;
            for (int i = 0; i < dirs.Count; i++) {
                int size = dirs[i].Size;
                if (size < 100000) {
                    total += size;
                }
            }
            return $"{total}";
        }

        [Description("What is the total size of that directory?")]
        public override string SolvePart2() {
            int neededSpace = root.Size - 40000000;
            dirs.Sort((left, right) => left.Size.CompareTo(right.Size));

            for (int i = 0; i < dirs.Count; i++) {
                int size = dirs[i].Size;
                if (size >= neededSpace) {
                    return $"{size}";
                }
            }
            return string.Empty;
        }

        private class Directory {
            public string Name;
            public Directory Parent;
            public Dictionary<string, Directory> Children = new();
            public Dictionary<string, File> Files = new();
            private int size;
            public int Size {
                get {
                    if (size == 0) { size = CalculateSize(); }
                    return size;
                }
            }
            public Directory(string name) {
                Name = name;
            }

            private int CalculateSize() {
                int total = 0;
                foreach (Directory sub in Children.Values) {
                    total += sub.Size;
                }
                foreach (File file in Files.Values) {
                    total += file.Size;
                }
                return total;
            }
            public override string ToString() {
                return $"{Parent}{Name}/";
            }
        }
        private class File {
            public string Name;
            public int Size;
            public override string ToString() {
                return $"{Name} Size={Size}";
            }
        }
    }
}