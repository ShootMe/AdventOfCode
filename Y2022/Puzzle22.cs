using AdventOfCode.Common;
using AdventOfCode.Core;
using System.ComponentModel;
namespace AdventOfCode.Y2022 {
    [Description("Monkey Map")]
    public class Puzzle22 : ASolver {
        private Cube cube;
        private string instructions;

        public override void Setup() {
            string[] lines = Input.Split('\n');
            int width = lines[0].Length;
            int height = 0;

            for (int i = 0; i < lines.Length; i++) {
                string line = lines[i];
                if (line.Length > width) { width = line.Length; }
                if (string.IsNullOrEmpty(line)) {
                    instructions = lines[i + 1];
                    height = i;
                    break;
                }
            }

            int size = Extensions.GCD(width, height);
            if (size == width && size == height) {
                size /= 3;
            } else if (size == width || size == height) {
                size /= 2;
            }

            cube = new Cube(size);
            Face face = null;
            for (int y = 0; y < height; y++) {
                string line = lines[y];

                for (int x = 0; x < line.Length; x++) {
                    char c = line[x];
                    switch (c) {
                        case ' ': x += size - 1; break;
                        default:
                            if ((x % size) == 0) {
                                face = cube.FindFace(x, y);
                                if (face.X == int.MaxValue) { face.X = x; face.Y = y; }
                            }
                            face[x - face.X, y - face.Y] = c == '#';
                            break;
                    }
                }
            }
        }

        [Description("Follow the path given in the monkeys' notes. What is the final password?")]
        public override string SolvePart1() {
            return $"{FindPassword(false)}";
        }

        [Description("Fold the map into a cube, then follow the path given in the monkeys' notes. What is the final password?")]
        public override string SolvePart2() {
            return $"{FindPassword(true)}";
        }

        private int FindPassword(bool onCube) {
            cube.SetConnections(onCube);
            Person person = new Person(cube);

            for (int i = 0; i < instructions.Length;) {
                char instruction = ' ';
                int amount = 0;
                while (i < instructions.Length && char.IsDigit(instruction = instructions[i++])) {
                    amount = amount * 10 + instruction - '0';
                }

                while (amount-- > 0 && person.Move()) { }

                person.Turn(instruction);
            }

            return person.Password();
        }

        private class Person {
            public int X, Y, DirX, DirY, Face;
            public Cube Cube;
            public Person(Cube cube) {
                X = 0; Y = 0;
                DirX = 1;
                DirY = 0;
                Face = 0;
                Cube = cube;
            }
            public bool Move() {
                int x = X + DirX;
                int y = Y + DirY;
                int face = Face;
                Connection connection = null;
                if (y < 0) {
                    connection = Cube.Faces[Face].Connections[Direction.UP];
                    y += Cube.Size;
                } else if (x >= Cube.Size) {
                    connection = Cube.Faces[Face].Connections[Direction.RIGHT];
                    x -= Cube.Size;
                } else if (y >= Cube.Size) {
                    connection = Cube.Faces[Face].Connections[Direction.DOWN];
                    y -= Cube.Size;
                } else if (x < 0) {
                    connection = Cube.Faces[Face].Connections[Direction.LEFT];
                    x += Cube.Size;
                }

                int dirX = DirX; int dirY = DirY;
                if (connection != null) {
                    face = connection.FaceID;
                    (x, y, dirX, dirY) = connection.Apply(this, x, y);
                }

                if (Cube.Faces[face][x, y]) {
                    return false;
                }

                X = x; Y = y; Face = face;
                DirX = dirX; DirY = dirY;
                return true;
            }
            public void Turn(char instruction) {
                if (instruction == 'R') {
                    (DirX, DirY) = (DirX, DirY) switch { (1, 0) => (0, 1), (0, 1) => (-1, 0), (-1, 0) => (0, -1), _ => (1, 0) };
                } else if (instruction == 'L') {
                    (DirX, DirY) = (DirX, DirY) switch { (1, 0) => (0, -1), (0, -1) => (-1, 0), (-1, 0) => (0, 1), _ => (1, 0) };
                }
            }
            public int Facing() {
                return (DirX, DirY) switch { (1, 0) => 0, (0, 1) => 1, (-1, 0) => 2, _ => 3 };
            }
            public int Password() {
                return 1000 * (Cube.Faces[Face].Y + Y + 1) + 4 * (Cube.Faces[Face].X + X + 1) + Facing();
            }
        }
        private class Cube {
            public Face[] Faces = new Face[6];
            public int Size;
            public Cube(int size) {
                Size = size;
                for (int i = 0; i < 6; i++) {
                    Faces[i] = new Face(size, i);
                }
            }

            public Face FindFace(int x, int y) {
                Face avail = null;
                for (int i = 0; i < 6; i++) {
                    Face face = Faces[i];
                    if (face.X <= x && x < face.X + Size && face.Y <= y && y < face.Y + Size) {
                        return face;
                    } else if (avail == null && face.X == int.MaxValue) {
                        avail = face;
                    }
                }
                return avail;
            }
            public void SetConnections(bool cube) {
                for (int y = 0; y < 6; y++) {
                    for (int x = 0; x < 6; x++) {
                        Face face = FindFace(x * Size, y * Size);
                        if (face == null) { continue; }
                        face.Connections[0] = null; face.Connections[1] = null; face.Connections[2] = null; face.Connections[3] = null;

                        Face connection = FindFace(x * Size, (y - 1) * Size);
                        if (connection != null) {
                            face.Connections[Direction.UP] = new Connection() { FaceID = connection.ID, Transform = 0 };
                        }
                        connection = FindFace((x + 1) * Size, y * Size);
                        if (connection != null) {
                            face.Connections[Direction.RIGHT] = new Connection() { FaceID = connection.ID, Transform = 0 };
                        }
                        connection = FindFace(x * Size, (y + 1) * Size);
                        if (connection != null) {
                            face.Connections[Direction.DOWN] = new Connection() { FaceID = connection.ID, Transform = 0 };
                        }
                        connection = FindFace((x - 1) * Size, y * Size);
                        if (connection != null) {
                            face.Connections[Direction.LEFT] = new Connection() { FaceID = connection.ID, Transform = 0 };
                        }
                    }
                }

                if (!cube) {
                    SetConnections(Faces[0]); SetConnections(Faces[1]); SetConnections(Faces[2]);
                    SetConnections(Faces[3]); SetConnections(Faces[4]); SetConnections(Faces[5]);
                } else if (Size == 4) {
                    //Example Cube
                    Face face = Faces[0];
                    face.Connections[Direction.LEFT] = new Connection() { FaceID = 2, Transform = 2 };
                    face.Connections[Direction.UP] = new Connection() { FaceID = 1, Transform = 3 };
                    face.Connections[Direction.RIGHT] = new Connection() { FaceID = 5, Transform = 3 };

                    face = Faces[1];
                    face.Connections[Direction.LEFT] = new Connection() { FaceID = 5, Transform = 1 };
                    face.Connections[Direction.UP] = new Connection() { FaceID = 0, Transform = 3 };
                    face.Connections[Direction.DOWN] = new Connection() { FaceID = 4, Transform = 3 };

                    face = Faces[2];
                    face.Connections[Direction.UP] = new Connection() { FaceID = 0, Transform = 1 };
                    face.Connections[Direction.DOWN] = new Connection() { FaceID = 4, Transform = 2 };

                    face = Faces[3];
                    face.Connections[Direction.RIGHT] = new Connection() { FaceID = 5, Transform = 1 };

                    face = Faces[4];
                    face.Connections[Direction.LEFT] = new Connection() { FaceID = 2, Transform = 1 };
                    face.Connections[Direction.DOWN] = new Connection() { FaceID = 1, Transform = 3 };

                    face = Faces[5];
                    face.Connections[Direction.UP] = new Connection() { FaceID = 3, Transform = 2 };
                    face.Connections[Direction.RIGHT] = new Connection() { FaceID = 1, Transform = 2 };
                    face.Connections[Direction.DOWN] = new Connection() { FaceID = 0, Transform = 3 };
                } else {
                    //Input Cube
                    Face face = Faces[0];
                    face.Connections[Direction.LEFT] = new Connection() { FaceID = 3, Transform = 3 };
                    face.Connections[Direction.UP] = new Connection() { FaceID = 5, Transform = 1 };

                    face = Faces[1];
                    face.Connections[Direction.UP] = new Connection() { FaceID = 5, Transform = 0 };
                    face.Connections[Direction.RIGHT] = new Connection() { FaceID = 4, Transform = 3 };
                    face.Connections[Direction.DOWN] = new Connection() { FaceID = 2, Transform = 1 };

                    face = Faces[2];
                    face.Connections[Direction.LEFT] = new Connection() { FaceID = 3, Transform = 2 };
                    face.Connections[Direction.RIGHT] = new Connection() { FaceID = 1, Transform = 2 };

                    face = Faces[3];
                    face.Connections[Direction.LEFT] = new Connection() { FaceID = 0, Transform = 3 };
                    face.Connections[Direction.UP] = new Connection() { FaceID = 2, Transform = 1 };

                    face = Faces[4];
                    face.Connections[Direction.RIGHT] = new Connection() { FaceID = 1, Transform = 3 };
                    face.Connections[Direction.DOWN] = new Connection() { FaceID = 5, Transform = 1 };

                    face = Faces[5];
                    face.Connections[Direction.LEFT] = new Connection() { FaceID = 0, Transform = 2 };
                    face.Connections[Direction.RIGHT] = new Connection() { FaceID = 4, Transform = 2 };
                    face.Connections[Direction.DOWN] = new Connection() { FaceID = 1, Transform = 0 };
                }
            }
            private void SetConnections(Face face) {
                if (face.Connections[Direction.UP] == null) {
                    Face connection = FollowConnection(face, Direction.DOWN);
                    face.Connections[Direction.UP] = new Connection() { FaceID = connection.ID, Transform = 0 };
                    connection.Connections[Direction.DOWN] = new Connection() { FaceID = face.ID, Transform = 0 };
                }
                if (face.Connections[Direction.RIGHT] == null) {
                    Face connection = FollowConnection(face, Direction.LEFT);
                    face.Connections[Direction.RIGHT] = new Connection() { FaceID = connection.ID, Transform = 0 };
                    connection.Connections[Direction.LEFT] = new Connection() { FaceID = face.ID, Transform = 0 };
                }
                if (face.Connections[Direction.DOWN] == null) {
                    Face connection = FollowConnection(face, Direction.UP);
                    face.Connections[Direction.DOWN] = new Connection() { FaceID = connection.ID, Transform = 0 };
                    connection.Connections[Direction.UP] = new Connection() { FaceID = face.ID, Transform = 0 };
                }
                if (face.Connections[Direction.LEFT] == null) {
                    Face connection = FollowConnection(face, Direction.RIGHT);
                    face.Connections[Direction.LEFT] = new Connection() { FaceID = connection.ID, Transform = 0 };
                    connection.Connections[Direction.RIGHT] = new Connection() { FaceID = face.ID, Transform = 0 };
                }
            }
            private Face FollowConnection(Face face, int direction) {
                while (face.Connections[direction] != null) {
                    face = Faces[face.Connections[direction].FaceID];
                }
                return face;
            }
            public override string ToString() {
                return $"[{Size}][{Faces[0]},{Faces[1]},{Faces[2]},{Faces[3]},{Faces[4]},{Faces[5]}]";
            }
        }
        private class Face {
            public int ID;
            public bool[,] Grid;
            public Connection[] Connections = new Connection[4];
            public int X, Y;
            public Face(int size, int id) {
                ID = id;
                Grid = new bool[size, size];
                X = int.MaxValue;
                Y = int.MaxValue;
            }
            public bool this[int x, int y] {
                get { return Grid[y, x]; }
                set { Grid[y, x] = value; }
            }
            public override string ToString() {
                return $"[{ID},{X},{Y}]({Connections[0]},{Connections[1]},{Connections[2]},{Connections[3]})";
            }
        }
        private class Connection {
            public int FaceID;
            public int Transform;
            public (int x, int y, int dirX, int dirY) Apply(Person person, int x, int y) {
                switch (Transform) {
                    case 0: return (x, y, person.DirX, person.DirY);
                    case 1: return (person.Cube.Size - 1 - y, x, -person.DirY, person.DirX);
                    case 2: return (y, person.Cube.Size - 1 - x, person.DirY, -person.DirX);
                    default: return (person.Cube.Size - 1 - x, person.Cube.Size - 1 - y, -person.DirX, -person.DirY);
                }
            }
            public override string ToString() {
                return $"{FaceID}-{Transform}";
            }
        }
        private static class Direction {
            public const int UP = 0;
            public const int RIGHT = 1;
            public const int DOWN = 2;
            public const int LEFT = 3;
        }
    }
}