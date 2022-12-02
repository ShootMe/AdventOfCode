using AdventOfCode.Common;
using AdventOfCode.Core;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
namespace AdventOfCode.Y2016 {
    [Description("Security Through Obscurity")]
    public class Puzzle04 : ASolver {
        private List<string> rooms;
        private List<Room> validRooms = new List<Room>();

        public override void Setup() {
            rooms = Input.Lines();

            for (int i = 0; i < rooms.Count; i++) {
                Room room = rooms[i];
                string code = room.Code;
                int[] counts = new int[128];
                for (int j = 0; j < code.Length; j++) {
                    counts[code[j]]++;
                }

                List<CharacterCount> characters = new List<CharacterCount>();
                for (int j = 0; j < counts.Length; j++) {
                    if ((char)j != '-' && counts[j] > 0) {
                        characters.Add(new CharacterCount() { Count = counts[j], ID = j });
                    }
                }

                characters.Sort(delegate (CharacterCount left, CharacterCount right) {
                    int countCompare = right.Count.CompareTo(left.Count);
                    return countCompare == 0 ? left.ID.CompareTo(right.ID) : countCompare;
                });

                string calculated = $"{(char)characters[0].ID}{(char)characters[1].ID}{(char)characters[2].ID}{(char)characters[3].ID}{(char)characters[4].ID}";
                if (calculated == room.Checksum) {
                    validRooms.Add(room);
                }
            }
        }

        [Description("What is the sum of the sector IDs of the real rooms?")]
        public override string SolvePart1() {
            int sum = 0;
            for (int i = 0; i < validRooms.Count; i++) {
                sum += validRooms[i].ID;
            }
            return $"{sum}";
        }

        [Description("What is the sector ID of the room where North Pole objects are stored?")]
        public override string SolvePart2() {
            for (int i = 0; i < validRooms.Count; i++) {
                if (validRooms[i].Decrypt() == "NORTHPOLE OBJECT STORAGE") {
                    return $"{validRooms[i].ID}";
                }
            }
            return string.Empty;
        }

        private class Room {
            public int ID;
            public string Code, Checksum;

            public static implicit operator Room(string value) {
                int index1 = value.LastIndexOf('-');
                Room room = new Room();
                room.Code = value.Substring(0, index1);
                int index2 = value.IndexOf('[', index1);
                room.ID = Tools.ParseInt(value.Substring(index1 + 1, index2 - index1 - 1));
                room.Checksum = value.Substring(index2 + 1, value.Length - index2 - 2);
                return room;
            }
            public string Decrypt() {
                StringBuilder result = new StringBuilder();
                for (int i = 0; i < Code.Length; i++) {
                    char val = char.ToUpper(Code[i]);
                    if (val == '-') {
                        result.Append(' ');
                    } else {
                        char newVal = (char)((((int)val - 0x41 + ID) % 26) + 0x41);
                        result.Append(newVal);
                    }
                }
                return result.ToString();
            }
            public override string ToString() {
                return $"{Code} = {Decrypt()}";
            }
        }
        private class CharacterCount {
            public int ID;
            public int Count;
        }
    }
}