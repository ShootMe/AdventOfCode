using AdventOfCode.Common;
using AdventOfCode.Core;
using System.Collections.Generic;
using System.ComponentModel;
namespace AdventOfCode.Y2015 {
    public class Puzzle13 : ASolver {
        private WrappingList<Person> people;
        public Puzzle13(string input) : base(input) { Name = "Knights of the Dinner Table"; }

        public override void Setup() {
            List<string> items = Tools.GetLines(Input);
            people = new WrappingList<Person>();

            Person person = null;
            for (int i = 0; i < items.Count; i++) {
                string item = items[i];
                int index1 = item.IndexOf(" would ");
                string name = item.Substring(0, index1);

                if (person == null || person.Name != name) {
                    person = new Person(name);
                    people.AddBefore(person);
                }

                int index2 = item.IndexOf(" happiness ", index1);
                int amount = Tools.ParseInt(item, index1 + 12, index2 - index1 - 12);
                if (item[index1 + 7] == 'l') { amount = -amount; }

                index1 = item.IndexOf(" next to ", index2);
                string otherName = item.Substring(index1 + 9, item.Length - index1 - 10);

                person.Happiness.Add(otherName, amount);
            }
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