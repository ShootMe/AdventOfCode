using AdventOfCode.Common;
using AdventOfCode.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
namespace AdventOfCode.Y2020 {
    [Description("Passport Processing")]
    public class Puzzle04 : ASolver {
        [Description("In your batch file, how many passports are valid?")]
        public override string SolvePart1() {
            int validCount = 0;
            Input.Sections(passport => {
                int count = 0;

                passport.Replace('\n',' ').Slice(' ', field => {
                    int index = field.IndexOf(':');
                    field = field.Substring(0, index);
                    switch (field) {
                        case "byr":
                        case "iyr":
                        case "eyr":
                        case "hgt":
                        case "hcl":
                        case "ecl":
                        case "pid":
                            count++;
                            break;
                    }
                });

                if (count == 7) {
                    validCount++;
                }
            });
            return $"{validCount}";
        }

        [Description("In your batch file, how many passports are valid?")]
        public override string SolvePart2() {
            HashSet<string> validECL = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { { "amb" }, { "blu" }, { "brn" }, { "gry" }, { "grn" }, { "hzl" }, { "oth" } };
            Dictionary<string, Func<string, bool>> validations = new Dictionary<string, Func<string, bool>>(StringComparer.OrdinalIgnoreCase) {
                {"byr", value => value.Length == 4 && int.TryParse(value, out int byr) && byr >= 1920 && byr <= 2002 },
                {"iyr", value => value.Length == 4 && int.TryParse(value, out int iyr) && iyr >= 2010 && iyr <= 2020 },
                {"eyr", value => value.Length == 4 && int.TryParse(value, out int eyr) && eyr >= 2020 && eyr <= 2030 },
                {"hgt", value => value.Length >= 4 && value.Length <= 5
                            && ((value.EndsWith("cm") && int.TryParse(value.Substring(0, 3), out int cm) && cm >= 150 && cm <= 193)
                            || (value.EndsWith("in") && int.TryParse(value.Substring(0, 2), out int inch) && inch >= 59 && inch <= 76)) },
                {"hcl", value => value.Length == 7 && value[0] == '#' && int.TryParse(value.Substring(1), NumberStyles.HexNumber, null, out int hcl) },
                {"ecl", value => value.Length == 3 && validECL.Contains(value) },
                {"pid", value => value.Length == 9 && int.TryParse(value, out int pid) },
            };

            int validCount = 0;
            Input.Sections(passport => {
                int count = 0;

                passport.Replace('\n', ' ').Slice(' ', field => {
                    int index = field.IndexOf(':');
                    string type = field.Substring(0, index);
                    string value = field.Substring(index + 1);

                    if (validations.TryGetValue(type, out Func<string, bool> validation) && validation(value)) {
                        count++;
                    }
                });

                if (count == 7) {
                    validCount++;
                }
            });
            return $"{validCount}";
        }
    }
}