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
            LoadSampleRecipes();
            LoadPlan();
        }

        private void LoadPlan()
        {
            if (listView == null) return;

            listView.Items.Clear();
            foreach (var entry in plan)
            {
                ListViewItem item = new ListViewItem(entry.Key.ToString("dd.MM.yyyy"));
                item.SubItems.Add(entry.Value.Name);
                listView.Items.Add(item);
            }
        }

        private void LoadSampleRecipes()
        {
            if (allRecipes.Count == 0)
            {
                var ingredients1 = new List<string> { "Макароны", "Сыр", "Масло", "Соль" };
                var instructions1 = new List<string> { "Сварить макароны", "Натереть сыр", "Смешать с маслом" };
                allRecipes.Add(new Recipe("Макароны с сыром", "Классическое итальянское блюдо",
                    ingredients1, instructions1, 450));

                var ingredients2 = new List<string> { "Куриное филе", "Рис", "Морковь", "Лук", "Специи" };
                var instructions2 = new List<string> { "Сварить рис", "Обжарить курицу с овощами", "Смешать" };
                allRecipes.Add(new Recipe("Курица с рисом", "Сытный ужин",
                    ingredients2, instructions2, 650));

                var ingredients3 = new List<string> { "Яйца", "Молоко", "Мука", "Сахар", "Яблоки" };
                var instructions3 = new List<string> { "Взбить яйца с сахаром", "Добавить муку", "Добавить яблоки", "Испечь" };
                allRecipes.Add(new Recipe("Яблочная шарлотка", "Вкусный десерт",
                    ingredients3, instructions3, 350));
            }
        }

        public void AddRecipeToPlan(DateTime date, Recipe recipe)
        {
            if (recipe == null) return;

            if (plan.ContainsKey(date))
            {
                if (listView != null)
                {
                    MessageBox.Show("На эту дату рецепт уже добавлен.", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            else
            {
                if (!allRecipes.Contains(recipe))
                {
                    allRecipes.Add(recipe);
                }
                plan.Add(date, recipe);
                LoadPlan();

                if (listView != null)
                {
                    MessageBox.Show("Рецепт добавлен в план!", "Успех",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        public void RemoveRecipeFromPlan(DateTime date)
        {
            if (plan.ContainsKey(date))
            {
                plan.Remove(date);
                LoadPlan();

                if (listView != null)
                {
                    MessageBox.Show("Рецепт удален из плана.", "Успех",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        public Recipe SearchRecipeByName(string name)
        {
            // ИСПРАВЛЕНИЕ: проверка на null
            if (string.IsNullOrEmpty(name))
            {
                return null;
            }

            return allRecipes.FirstOrDefault(r =>
                r.Name.IndexOf(name, StringComparison.OrdinalIgnoreCase) >= 0);
        }

        public List<Recipe> GetAllRecipes()
        {
            return new List<Recipe>(allRecipes);
        }
    }
}