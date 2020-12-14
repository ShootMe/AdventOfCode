using AdventOfCode.Core;
using System.Collections.Generic;
using System.ComponentModel;
namespace AdventOfCode.Y2015 {
    public class Puzzle15 : ASolver {
        private Ingredient[] ingredients;
        public Puzzle15(string input) : base(input) { Name = "Science for Hungry People"; }

        public override void Setup() {
            List<string> items = Tools.GetLines(Input);
            ingredients = new Ingredient[items.Count];

            for (int i = 0; i < items.Count; i++) {
                string item = items[i];
                int capacity = Tools.ParseInt(item, " capacity ", ", ");
                int durability = Tools.ParseInt(item, " durability ", ", ");
                int flavor = Tools.ParseInt(item, " flavor ", ", ");
                int texture = Tools.ParseInt(item, " texture ", ", ");
                int calories = Tools.ParseInt(item, " calories ");

                Ingredient ingredient = new Ingredient() { Calories = calories, Capacity = capacity, Durability = durability, Flavor = flavor, Texture = texture };
                ingredients[i] = ingredient;
            }
        }

        [Description("What is the total score of the highest-scoring cookie you can make?")]
        public override string SolvePart1() {
            return $"{FindBest(0)}";
        }

        [Description("What is the best score of the cookie you can make with a calorie total of 500?")]
        public override string SolvePart2() {
            return $"{FindBest(0, true)}";
        }

        private int FindBest(int level, bool limitCalories = false) {
            Ingredient ingredient = ingredients[level];

            if (level == ingredients.Length - 1) {
                Ingredient final = new Ingredient();
                int amount = 0;
                for (int i = ingredients.Length - 2; i >= 0; i--) {
                    Ingredient other = ingredients[i];
                    final.AddTo(other);
                    amount += other.Amount;
                }
                ingredient.Amount = 100 - amount;
                final.AddTo(ingredient);
                if (!limitCalories || final.Calories == 500) {
                    return final.Score();
                }
                return 0;
            }

            int score = 0;
            for (int i = 0; i < 100; i++) {
                ingredient.Amount = i;
                int result = FindBest(level + 1, limitCalories);
                if (result > score) {
                    score = result;
                }
            }
            return score;
        }

        private class Ingredient {
            public int Capacity;
            public int Durability;
            public int Flavor;
            public int Texture;
            public int Calories;
            public int Amount;

            public Ingredient() { }
            public Ingredient(Ingredient copy) {
                Capacity = copy.Capacity;
                Durability = copy.Durability;
                Flavor = copy.Flavor;
                Texture = copy.Texture;
                Calories = copy.Calories;
            }

            public void AddTo(Ingredient ingredient) {
                Capacity += ingredient.Capacity * ingredient.Amount;
                Durability += ingredient.Durability * ingredient.Amount;
                Flavor += ingredient.Flavor * ingredient.Amount;
                Texture += ingredient.Texture * ingredient.Amount;
                Calories += ingredient.Calories * ingredient.Amount;
            }
            public int Score() {
                return (Capacity < 0 ? 0 : Capacity) * (Durability < 0 ? 0 : Durability) * (Flavor < 0 ? 0 : Flavor) * (Texture < 0 ? 0 : Texture);
            }
            public override string ToString() {
                return $"{Capacity} {Durability} {Flavor} {Texture} {Calories} {Amount}";
            }
        }
    }
}