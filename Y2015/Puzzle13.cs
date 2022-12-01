using AdventOfCode.Common;
using AdventOfCode.Core;
using System.Collections.Generic;
using System.ComponentModel;
namespace AdventOfCode.Y2015 {
    [Description("Knights of the Dinner Table")]
    public class Puzzle13 : ASolver {
        private WrappingList<Person> people = new();

        public override void Setup() {
            Person person = null;
            Input.Slice('\n', item => {
                string[] splits = item.SplitOn(" would ", " happiness ", " next to ", ".");
                string name = splits[0];

                if (person == null || person.Name != name) {
                    person = new Person(name);
                    people.AddBefore(person);
                }

                int amount = splits[1].Substring(5).ToInt();
                if (splits[1][0] == 'l') { amount = -amount; }

                string otherName = splits[3];

                person.Happiness.Add(otherName, amount);
            });
        }

        [Description("What is the total happiness for the optimal seating arrangement?")]
        public override string SolvePart1() {
            int bestHappiness = int.MinValue;
            foreach (WrappingList<Person> seq in people.Permute()) {
                int nextHappieness = 0;
                for (int i = 0; i < seq.Count; i++) {
                    nextHappieness += seq.Current.HappinessAmount(seq.Previous, seq.Next);
                    seq.IncreasePosition();
                }

                if (nextHappieness > bestHappiness) {
                    bestHappiness = nextHappieness;
                }
            }
            return $"{bestHappiness}";
        }

        [Description("What is the total happiness for the optimal seating arrangement that includes yourself?")]
        public override string SolvePart2() {
            Person you = new Person("You");
            foreach (Person person in people) {
                person.Happiness.Add("You", 0);
                you.Happiness.Add(person.Name, 0);
            }
            people.AddBefore(you);

            int bestHappiness = int.MinValue;
            foreach (WrappingList<Person> seq in people.Permute()) {
                int nextHappieness = 0;
                for (int i = 0; i < seq.Count; i++) {
                    nextHappieness += seq.Current.HappinessAmount(seq.Previous, seq.Next);
                    seq.IncreasePosition();
                }

                if (nextHappieness > bestHappiness) {
                    bestHappiness = nextHappieness;
                }
            }

            return $"{bestHappiness}";
        }

        private class Person {
            public string Name;
            public Dictionary<string, int> Happiness = new Dictionary<string, int>();

            public Person(string name) {
                Name = name;
            }

            public int HappinessAmount(Person left, Person right) {
                return Happiness[left.Name] + Happiness[right.Name];
            }
            public override string ToString() {
                return Name;
            }
        }
    }
}