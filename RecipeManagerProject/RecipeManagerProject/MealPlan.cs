using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace RecipeMealPlanner
{
    /// <summary>
    /// Класс управления планом меню
    /// Хранит список всех рецептов и план на даты
    /// </summary>
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
                allRecipes.Add(new Recipe("Макароны с сыром", "Классическое итальянское блюдо", ingredients1, instructions1, 450));

                var ingredients2 = new List<string> { "Куриное филе", "Рис", "Морковь", "Лук", "Специи" };
                var instructions2 = new List<string> { "Сварить рис", "Обжарить курицу с овощами", "Смешать" };
                allRecipes.Add(new Recipe("Курица с рисом", "Сытный ужин", ingredients2, instructions2, 650));

                var ingredients3 = new List<string> { "Яйца", "Молоко", "Мука", "Сахар", "Яблоки" };
                var instructions3 = new List<string> { "Взбить яйца с сахаром", "Добавить муку", "Добавить яблоки", "Испечь" };
                allRecipes.Add(new Recipe("Яблочная шарлотка", "Вкусный десерт", ingredients3, instructions3, 350));

                var ingredients4 = new List<string> { "Яблоки", "Творог", "Мед" };
                var instructions4 = new List<string> { "Смешать", "Запечь" };
                allRecipes.Add(new Recipe("Запеченные яблоки", "Десерт из яблок", ingredients4, instructions4, 200));
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
                MessageBox.Show("Рецепт добавлен в план!", "Успех",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        public void RemoveRecipeFromPlan(DateTime date)
        {
            if (plan.ContainsKey(date))
            {
                plan.Remove(date);
                LoadPlan();
                MessageBox.Show("Рецепт удален из плана.", "Успех",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        public List<Recipe> SearchRecipeByName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return new List<Recipe>();

            return allRecipes.Where(r =>
                r.Name.IndexOf(name, StringComparison.OrdinalIgnoreCase) >= 0 ||
                r.Description.IndexOf(name, StringComparison.OrdinalIgnoreCase) >= 0 ||
                r.Ingredients.Any(i => i.IndexOf(name, StringComparison.OrdinalIgnoreCase) >= 0)
            ).ToList();
        }

        public List<Recipe> GetAllRecipes()
        {
            return allRecipes;
        }

        public Recipe GetRecipeByDate(DateTime date)
        {
            if (plan.ContainsKey(date))
            {
                return plan[date];
            }
            return null;
        }

        /// <summary>
        /// НОВЫЙ МЕТОД для варианта 26: Возвращает копию плана меню
        /// Нужен для генерации списка покупок без нарушения инкапсуляции
        /// </summary>
        public Dictionary<DateTime, Recipe> GetPlan()
        {
            return new Dictionary<DateTime, Recipe>(plan);
        }
    }
}