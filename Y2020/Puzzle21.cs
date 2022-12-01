using AdventOfCode.Core;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
namespace AdventOfCode.Y2020 {
    [Description("Allergen Assessment")]
    public class Puzzle21 : ASolver {
        private List<Food> foods;
        private Dictionary<string, HashSet<string>> allergenToIngredients;

        public override void Setup() {
            List<string> items = Tools.GetLines(Input);
            foods = new List<Food>();

            for (int i = 0; i < items.Count; i++) {
                string item = items[i];

                int index = item.IndexOf('(');
                string[] ingredients = item.Substring(0, index - 1).Split(' ');
                string[] allergens = item.Substring(index + 10, item.Length - index - 11).Split(", ");

                Food food = new Food();
                for (int j = 0; j < ingredients.Length; j++) {
                    food.Ingredients.Add(ingredients[j]);
                }
                for (int j = 0; j < allergens.Length; j++) {
                    food.Allergens.Add(allergens[j]);
                }
                foods.Add(food);
            }

            allergenToIngredients = new Dictionary<string, HashSet<string>>();
            for (int i = 0; i < foods.Count; i++) {
                Food food = foods[i];

                foreach (string allergen in food.Allergens) {
                    HashSet<string> ingredients;
                    if (!allergenToIngredients.TryGetValue(allergen, out ingredients)) {
                        ingredients = new HashSet<string>();

                        foreach (string ingredient in food.Ingredients) {
                            ingredients.Add(ingredient);
                        }
                        allergenToIngredients.Add(allergen, ingredients);
                    }

                    ingredients.IntersectWith(food.Ingredients);
                }
            }
        }

        [Description("How many times do any of those ingredients appear?")]
        public override string SolvePart1() {
            HashSet<string> notSafeIngredients = new HashSet<string>();
            foreach (HashSet<string> ingredients in allergenToIngredients.Values) {
                notSafeIngredients.UnionWith(ingredients);
            }

            List<string> safeIngredients = new List<string>();
            for (int i = 0; i < foods.Count; i++) {
                Food food = foods[i];

                foreach (string ingredient in food.Ingredients) {
                    if (!notSafeIngredients.Contains(ingredient)) {
                        safeIngredients.Add(ingredient);
                    }
                }
            }

            return $"{safeIngredients.Count}";
        }

        [Description("What is your canonical dangerous ingredient list?")]
        public override string SolvePart2() {
            List<Allergen> foundAllergens = new List<Allergen>();
            while (allergenToIngredients.Count > 0) {
                foreach (KeyValuePair<string, HashSet<string>> pair in allergenToIngredients) {
                    HashSet<string> ingredients = pair.Value;
                    for (int i = 0; i < foundAllergens.Count; i++) {
                        ingredients.Remove(foundAllergens[i].Ingredient);
                    }
                    if (ingredients.Count == 1) {
                        string[] ingredient = new string[1];
                        pair.Value.CopyTo(ingredient);
                        foundAllergens.Add(new Allergen() { Name = pair.Key, Ingredient = ingredient[0] });
                    }
                }
                for (int i = 0; i < foundAllergens.Count; i++) {
                    allergenToIngredients.Remove(foundAllergens[i].Name);
                }
            }

            foundAllergens.Sort((left, right) => left.Name.CompareTo(right.Name));

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < foundAllergens.Count; i++) {
                sb.Append($"{foundAllergens[i].Ingredient},");
            }
            sb.Length--;
            return sb.ToString();
        }

        private class Food {
            public HashSet<string> Ingredients = new HashSet<string>();
            public HashSet<string> Allergens = new HashSet<string>();

            public override string ToString() {
                StringBuilder sb = new StringBuilder();
                foreach (string ingredient in Ingredients) {
                    sb.Append($"{ingredient}, ");
                }
                if (sb.Length > 0) { sb.Length -= 2; }

                sb.Append(" [");
                foreach (string allergen in Allergens) {
                    sb.Append($"{allergen}, ");
                }
                sb.Length -= 2;
                sb.Append(']');

                return sb.ToString();
            }
        }
        private class Allergen {
            public string Name;
            public string Ingredient;

            public override string ToString() {
                return $"{Name} {Ingredient}";
            }
        }
    }
}