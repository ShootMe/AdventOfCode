using AdventOfCode.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
namespace AdventOfCode.Y2021 {
    [Description("Amphipod")]
    public class Puzzle23 : ASolver {
        private static readonly int[] energies = new int[] { 1, 10, 100, 1000 };
        private Amphipod[] rooms;
        private Amphipod[] hallway;
        private int bestEnergy;
        public override void Setup() {
            List<string> lines = Tools.GetLines(Input);
            hallway = new Amphipod[7];
            rooms = new Amphipod[16];
            Array.Fill(hallway, Amphipod.EMPTY);
            Array.Fill(rooms, Amphipod.EMPTY);

            string line = lines[3];
            for (int j = 3; j < 10; j += 2) {
                int type = line[j] - 'A';
                int room = (j - 3) / 2;
                rooms[room + 4] = new Amphipod(type);
            }
            line = lines[2];
            for (int j = 3; j < 10; j += 2) {
                int type = line[j] - 'A';
                int room = (j - 3) / 2;
                rooms[room] = new Amphipod(type);
            }
        }

        [Description("What is the least energy required to organize the amphipods?")]
        public override string SolvePart1() {
            bestEnergy = int.MaxValue;
            Run(0, 2);
            return $"{bestEnergy}";
        }

        [Description("What is the least energy required to organize the amphipods?")]
        public override string SolvePart2() {
            Setup();

            for (int i = 4; i < 8; i++) {
                Amphipod amp = rooms[i];
                rooms[i + 8] = new Amphipod(amp.Type);
            }

            rooms[4] = new Amphipod(3);
            rooms[5] = new Amphipod(2);
            rooms[6] = new Amphipod(1);
            rooms[7] = new Amphipod(0);

            rooms[8] = new Amphipod(3);
            rooms[9] = new Amphipod(1);
            rooms[10] = new Amphipod(0);
            rooms[11] = new Amphipod(2);

            bestEnergy = int.MaxValue;
            Run(0, 4);
            return $"{bestEnergy}";
        }
        private void Run(int energy, int count) {
            int[] solved = RoomsSolved(count);
            if (solved[0] == 0 && solved[1] == 0 && solved[2] == 0 && solved[3] == 0) {
                if (energy < bestEnergy) {
                    bestEnergy = energy;
                }
                return;
            }

            int minEnergy = MinimumEnergy();
            if (minEnergy + energy >= bestEnergy) { return; }

            List<(int, int, int)> moves = GetMoves(solved);
            for (int i = 0; i < moves.Count; i++) {
                (int from, int to, int e) = moves[i];

                int nextEnergy = energy + Math.Abs(e);
                if (nextEnergy >= bestEnergy) { continue; }

                if (e < 0) {
                    rooms[to] = hallway[from];
                    hallway[from] = Amphipod.EMPTY;
                } else {
                    hallway[to] = rooms[from];
                    rooms[from] = Amphipod.EMPTY;
                }

                Run(nextEnergy, count);

                if (e < 0) {
                    hallway[from] = rooms[to];
                    rooms[to] = Amphipod.EMPTY;
                } else {
                    rooms[from] = hallway[to];
                    hallway[to] = Amphipod.EMPTY;
                }
            }
        }
        private List<(int, int, int)> GetMoves(int[] solved) {
            List<(int, int, int)> moves = new List<(int, int, int)>();

            for (int i = 0; i < 7; i++) {
                Amphipod amp = hallway[i];
                if (amp == Amphipod.EMPTY || solved[amp.Type] < 0) { continue; }

                (int roomPos, int steps) = PathToRoom(solved, i, amp.Type);
                if (roomPos < 0) { continue; }

                moves.Add((i, roomPos, -steps * energies[amp.Type]));
            }

            if (moves.Count > 0) { return moves; }

            for (int i = 0; i < 4; i++) {
                if (solved[i] >= 0) { continue; }

                for (int j = 0; j < 4; j++) {
                    Amphipod amp = rooms[j * 4 + i];
                    if (amp == Amphipod.EMPTY) { continue; }

                    PathsToHallway(moves, i, j, amp.Type);
                    break;
                }
            }

            return moves;
        }
        private int MinimumEnergy() {
            int energy = 0;
            for (int i = 0; i < 7; i++) {
                Amphipod amp = hallway[i];
                if (amp == Amphipod.EMPTY) { continue; }

                switch (i) {
                    case 0: energy += energies[amp.Type] * (amp.Type * 2 + 3); break;
                    case 1: energy += energies[amp.Type] * (amp.Type * 2 + 2); break;
                    case 2: energy += energies[amp.Type] * (amp.Type == 0 || amp.Type == 1 ? 2 : amp.Type * 2); break;
                    case 3: energy += energies[amp.Type] * (amp.Type == 1 || amp.Type == 2 ? 2 : 4); break;
                    case 4: energy += energies[amp.Type] * (amp.Type == 2 || amp.Type == 3 ? 2 : (3 - amp.Type) * 2); break;
                    case 5: energy += energies[amp.Type] * ((3 - amp.Type) * 2 + 2); break;
                    case 6: energy += energies[amp.Type] * ((3 - amp.Type) * 2 + 3); break;
                }
            }

            return energy;
        }
        private void PathsToHallway(List<(int, int, int)> moves, int r, int d, int t) {
            int energy = energies[t];
            int from = d * 4 + r;
            switch (r) {
                case 0:
                    if (hallway[1] == Amphipod.EMPTY) {
                        moves.Add((from, 1, (2 + d) * energy));
                        if (hallway[0] == Amphipod.EMPTY) {
                            moves.Add((from, 0, (3 + d) * energy));
                        }
                    }
                    if (hallway[2] == Amphipod.EMPTY) {
                        moves.Add((from, 2, (2 + d) * energy));
                        if (hallway[3] == Amphipod.EMPTY) {
                            moves.Add((from, 3, (4 + d) * energy));
                            if (hallway[4] == Amphipod.EMPTY) {
                                moves.Add((from, 4, (6 + d) * energy));
                                if (hallway[5] == Amphipod.EMPTY) {
                                    moves.Add((from, 5, (8 + d) * energy));
                                    if (hallway[6] == Amphipod.EMPTY) {
                                        moves.Add((from, 6, (9 + d) * energy));
                                    }
                                }
                            }
                        }
                    }
                    break;
                case 1:
                    if (hallway[2] == Amphipod.EMPTY) {
                        moves.Add((from, 2, (2 + d) * energy));
                        if (hallway[1] == Amphipod.EMPTY) {
                            moves.Add((from, 1, (4 + d) * energy));
                            if (hallway[0] == Amphipod.EMPTY) {
                                moves.Add((from, 0, (5 + d) * energy));
                            }
                        }
                    }
                    if (hallway[3] == Amphipod.EMPTY) {
                        moves.Add((from, 3, (2 + d) * energy));
                        if (hallway[4] == Amphipod.EMPTY) {
                            moves.Add((from, 4, (4 + d) * energy));
                            if (hallway[5] == Amphipod.EMPTY) {
                                moves.Add((from, 5, (6 + d) * energy));
                                if (hallway[6] == Amphipod.EMPTY) {
                                    moves.Add((from, 6, (7 + d) * energy));
                                }
                            }
                        }
                    }
                    break;
                case 2:
                    if (hallway[3] == Amphipod.EMPTY) {
                        moves.Add((from, 3, (2 + d) * energy));
                        if (hallway[2] == Amphipod.EMPTY) {
                            moves.Add((from, 2, (4 + d) * energy));
                            if (hallway[1] == Amphipod.EMPTY) {
                                moves.Add((from, 1, (6 + d) * energy));
                                if (hallway[0] == Amphipod.EMPTY) {
                                    moves.Add((from, 0, (7 + d) * energy));
                                }
                            }
                        }
                    }
                    if (hallway[4] == Amphipod.EMPTY) {
                        moves.Add((from, 4, (2 + d) * energy));
                        if (hallway[5] == Amphipod.EMPTY) {
                            moves.Add((from, 5, (4 + d) * energy));
                            if (hallway[6] == Amphipod.EMPTY) {
                                moves.Add((from, 6, (5 + d) * energy));
                            }
                        }
                    }
                    break;
                case 3:
                    if (hallway[4] == Amphipod.EMPTY) {
                        moves.Add((from, 4, (2 + d) * energy));
                        if (hallway[3] == Amphipod.EMPTY) {
                            moves.Add((from, 3, (4 + d) * energy));
                            if (hallway[2] == Amphipod.EMPTY) {
                                moves.Add((from, 2, (6 + d) * energy));
                                if (hallway[1] == Amphipod.EMPTY) {
                                    moves.Add((from, 1, (8 + d) * energy));
                                    if (hallway[0] == Amphipod.EMPTY) {
                                        moves.Add((from, 0, (9 + d) * energy));
                                    }
                                }
                            }
                        }
                    }
                    if (hallway[5] == Amphipod.EMPTY) {
                        moves.Add((from, 5, (2 + d) * energy));
                        if (hallway[6] == Amphipod.EMPTY) {
                            moves.Add((from, 6, (3 + d) * energy));
                        }
                    }
                    break;
            }
        }
        private (int, int) PathToRoom(int[] solved, int h, int r) {
            int from = (solved[r] - 1) * 4 + r;
            int to = solved[r];
            switch (h) {
                case 0:
                    if (hallway[1] == Amphipod.EMPTY) {
                        if (r == 0) {
                            return (from, to + 2);
                        } else if (hallway[2] == Amphipod.EMPTY) {
                            if (r == 1) {
                                return (from, to + 4);
                            } else if (hallway[3] == Amphipod.EMPTY) {
                                if (r == 2) {
                                    return (from, to + 6);
                                } else if (hallway[4] == Amphipod.EMPTY) {
                                    return (from, to + 8);
                                }
                            }
                        }
                    }
                    break;
                case 1:
                    if (r == 0) {
                        return (from, to + 1);
                    } else if (hallway[2] == Amphipod.EMPTY) {
                        if (r == 1) {
                            return (from, to + 3);
                        } else if (hallway[3] == Amphipod.EMPTY) {
                            if (r == 2) {
                                return (from, to + 5);
                            } else if (hallway[4] == Amphipod.EMPTY) {
                                return (from, to + 7);
                            }
                        }
                    }
                    break;
                case 2:
                    if (r == 0 || r == 1) {
                        return (from, to + 1);
                    } else if (hallway[3] == Amphipod.EMPTY) {
                        if (r == 2) {
                            return (from, to + 3);
                        } else if (hallway[4] == Amphipod.EMPTY) {
                            return (from, to + 5);
                        }
                    }
                    break;
                case 3:
                    if (r == 0) {
                        if (hallway[2] == Amphipod.EMPTY) {
                            return (from, to + 3);
                        }
                    } else if (r == 1 || r == 2) {
                        return (from, to + 1);
                    } else if (hallway[4] == Amphipod.EMPTY) {
                        return (from, to + 3);
                    }
                    break;
                case 4:
                    if (r == 2 || r == 3) {
                        return (from, to + 1);
                    } else if (hallway[3] == Amphipod.EMPTY) {
                        if (r == 1) {
                            return (from, to + 3);
                        } else if (hallway[2] == Amphipod.EMPTY) {
                            return (from, to + 5);
                        }
                    }
                    break;
                case 5:
                    if (r == 3) {
                        return (from, to + 1);
                    } else if (hallway[4] == Amphipod.EMPTY) {
                        if (r == 2) {
                            return (from, to + 3);
                        } else if (hallway[3] == Amphipod.EMPTY) {
                            if (r == 1) {
                                return (from, to + 5);
                            } else if (hallway[2] == Amphipod.EMPTY) {
                                return (from, to + 7);
                            }
                        }
                    }
                    break;
                case 6:
                    if (hallway[5] == Amphipod.EMPTY) {
                        if (r == 3) {
                            return (from, to + 2);
                        } else if (hallway[4] == Amphipod.EMPTY) {
                            if (r == 2) {
                                return (from, to + 4);
                            } else if (hallway[3] == Amphipod.EMPTY) {
                                if (r == 1) {
                                    return (from, to + 6);
                                } else if (hallway[2] == Amphipod.EMPTY) {
                                    return (from, to + 8);
                                }
                            }
                        }
                    }
                    break;
            }
            return (-1, -1);
        }
        private int[] RoomsSolved(int max) {
            int[] solved = new int[4];
            for (int j = 0; j < 4; j++) {
                int count = max;
                for (int i = max - 1; i >= 0; i--) {
                    Amphipod amp = rooms[i * 4 + j];
                    if (amp == Amphipod.EMPTY || amp.Type == j) {
                        if (amp != Amphipod.EMPTY) {
                            count--;
                        }
                    } else {
                        count = -1;
                        break;
                    }
                }
                solved[j] = count;
            }
            return solved;
        }
        public struct Amphipod {
            public static readonly Amphipod EMPTY = new Amphipod();
            public int Type;

            public Amphipod() {
                Type = -1;
            }
            public Amphipod(int type) {
                Type = type;
            }
            public static bool operator ==(Amphipod a, Amphipod b) {
                return a.Type == b.Type;
            }
            public static bool operator !=(Amphipod a, Amphipod b) {
                return a.Type != b.Type;
            }
            public override bool Equals(object obj) {
                return obj is Amphipod amp && amp == this;
            }
            public override int GetHashCode() {
                return Type;
            }
            public override string ToString() {
                return $"{(char)(Type + 'A')}";
            }
        }
    }
}