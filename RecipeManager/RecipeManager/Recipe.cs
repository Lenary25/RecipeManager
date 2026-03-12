using System;
using System.Collections.Generic;

namespace RecipeMealPlanner
{
    public class Recipe
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public List<string> Ingredients { get; set; }
        public List<string> Instructions { get; set; }
        public int Calories { get; set; }

        public Recipe(string name, string description, List<string> ingredients,
                      List<string> instructions, int calories)
        {
            Name = name;
            Description = description;
            Ingredients = ingredients ?? new List<string>();
            Instructions = instructions ?? new List<string>();
            Calories = calories;
        }

        public override string ToString()
        {
            return Name;
        }

        // Для отображения полной информации
        public string GetFullInfo()
        {
            string result = $"Название: {Name}\n";
            result += $"Описание: {Description}\n";
            result += $"Калории: {Calories}\n\n";
            result += "Ингредиенты:\n";
            foreach (var ing in Ingredients)
                result += $"  • {ing}\n";
            result += "\nИнструкции:\n";
            foreach (var inst in Instructions)
                result += $"  • {inst}\n";
            return result;
        }
    }
}