using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace RecipeMealPlanner
{
    public class MealPlan
    {
        private Dictionary<DateTime, Recipe> plan = new Dictionary<DateTime, Recipe>();
        private List<Recipe> allRecipes = new List<Recipe>();
        private ListView listView;

        public MealPlan(ListView listView)
        {
            this.listView = listView;
            LoadPlan();
            LoadSampleRecipes(); // Для демонстрации
        }

        private void LoadPlan()
        {
            listView.Items.Clear();
            foreach (var entry in plan)
            {
                ListViewItem item = new ListViewItem(entry.Key.ToString("dd.MM.yyyy"));
                item.SubItems.Add(entry.Value.Name);
                item.Tag = entry.Key; // Сохраняем дату в Tag
                listView.Items.Add(item);
            }
        }

        // Добавляем примеры рецептов для тестирования
        private void LoadSampleRecipes()
        {
            if (allRecipes.Count == 0)
            {
                var ingredients1 = new List<string> { "Макароны", "Сыр", "Масло" };
                var instructions1 = new List<string> { "Сварить макароны", "Добавить сыр" };
                allRecipes.Add(new Recipe("Макароны с сыром", "Простое блюдо",
                    ingredients1, instructions1, 450));

                var ingredients2 = new List<string> { "Курица", "Рис", "Специи" };
                var instructions2 = new List<string> { "Сварить рис", "Пожарить курицу" };
                allRecipes.Add(new Recipe("Курица с рисом", "Сытный обед",
                    ingredients2, instructions2, 650));
            }
        }

        public void AddRecipeToPlan(DateTime date, Recipe recipe)
        {
            if (plan.ContainsKey(date))
            {
                MessageBox.Show("На эту дату рецепт уже добавлен.", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {
                if (!allRecipes.Contains(recipe))
                {
                    allRecipes.Add(recipe);
                }
                plan.Add(date, recipe);
                LoadPlan();
            }
        }

        public void RemoveRecipeFromPlan(DateTime date)
        {
            if (plan.ContainsKey(date))
            {
                plan.Remove(date);
                LoadPlan();
            }
        }

        public Recipe SearchRecipeByName(string name)
        {
            return allRecipes.FirstOrDefault(r =>
                r.Name.IndexOf(name, StringComparison.OrdinalIgnoreCase) >= 0);
        }

        public List<Recipe> GetAllRecipes()
        {
            return new List<Recipe>(allRecipes);
        }

        public Recipe GetRecipeAtDate(DateTime date)
        {
            plan.TryGetValue(date, out Recipe recipe);
            return recipe;
        }
    }
}