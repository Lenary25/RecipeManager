using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RecipeMealPlanner
{
    /// <summary>
    /// Класс для генерации и управления списком покупок
    /// НОВАЯ ФУНКЦИЯ для варианта 26
    /// </summary>
    public static class ShoppingListGenerator
    {
        /// <summary>
        /// Категории продуктов
        /// </summary>
        public enum ProductCategory
        {
            Овощи,
            Фрукты,
            Мясо,
            Рыба,
            МолочныеПродукты,
            Бакалея,
            Напитки,
            Специи,
            Прочее
        }

        /// <summary>
        /// Определяет категорию продукта по его названию
        /// </summary>
        public static ProductCategory GetCategory(string ingredientName)
        {
            if (string.IsNullOrWhiteSpace(ingredientName))
                return ProductCategory.Прочее;

            string lowerName = ingredientName.ToLower();

            // Овощи
            string[] vegetables = { "помидор", "томат", "огурец", "морковь", "картофель", "картошка",
                "лук", "чеснок", "капуста", "перец болгарский", "баклажан", "кабачок", "свекла", "редис",
                "зелень", "укроп", "петрушка", "салат", "брокколи", "цветная капуста" };
            foreach (var v in vegetables)
                if (lowerName.Contains(v)) return ProductCategory.Овощи;

            // Фрукты
            string[] fruits = { "яблоко", "груша", "банан", "апельсин", "мандарин", "лимон", "лайм",
                "виноград", "клубника", "малина", "черника", "персик", "абрикос", "слива", "киви",
                "ананас", "арбуз", "дыня" };
            foreach (var f in fruits)
                if (lowerName.Contains(f)) return ProductCategory.Фрукты;

            // Мясо
            string[] meat = { "курица", "куриное филе", "говядина", "свинина", "баранина", "индейка",
                "утка", "фарш", "колбаса", "сосиски", "ветчина", "бекон" };
            foreach (var m in meat)
                if (lowerName.Contains(m)) return ProductCategory.Мясо;

            // Рыба
            string[] fish = { "рыба", "лосось", "семга", "треска", "минтай", "тунец", "скумбрия",
                "сельдь", "креветка", "кальмар", "мидии" };
            foreach (var f in fish)
                if (lowerName.Contains(f)) return ProductCategory.Рыба;

            // Молочные продукты
            string[] dairy = { "молоко", "кефир", "йогурт", "сметана", "творог", "сыр", "масло сливочное",
                "сливки", "ряженка", "простокваша" };
            foreach (var d in dairy)
                if (lowerName.Contains(d)) return ProductCategory.МолочныеПродукты;

            // Бакалея
            string[] grocery = { "мука", "сахар", "соль", "рис", "гречка", "макароны", "спагетти",
                "крупа", "овсянка", "хлеб", "батон", "печенье", "конфеты", "шоколад", "масло растительное",
                "подсолнечное масло", "оливковое масло" };
            foreach (var g in grocery)
                if (lowerName.Contains(g)) return ProductCategory.Бакалея;

            // Напитки
            string[] drinks = { "вода", "сок", "чай", "кофе", "газировка", "лимонад" };
            foreach (var d in drinks)
                if (lowerName.Contains(d)) return ProductCategory.Напитки;

            // Специи
            string[] spices = { "перец чёрный", "перец красный", "паприка", "корица", "ваниль", "имбирь", "гвоздика",
                "лавровый лист", "базилик", "орегано", "тимьян", "розмарин", "куркума", "карри",
                "кориандр", "мускатный орех" };
            foreach (var s in spices)
                if (lowerName.Contains(s)) return ProductCategory.Специи;

            // Если слово "перец" отдельно - относим к специям
            if (lowerName == "перец") return ProductCategory.Специи;

            return ProductCategory.Прочее;
        }

        /// <summary>
        /// Генерирует список покупок на основе запланированных рецептов
        /// </summary>
        public static Dictionary<string, int> GenerateShoppingList(Dictionary<DateTime, Recipe> mealPlan, DateTime startDate, DateTime endDate)
        {
            var shoppingList = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

            foreach (var entry in mealPlan)
            {
                if (entry.Key.Date >= startDate.Date && entry.Key.Date <= endDate.Date)
                {
                    Recipe recipe = entry.Value;
                    foreach (string ingredient in recipe.Ingredients)
                    {
                        string normalizedIngredient = ingredient.Trim();
                        if (string.IsNullOrWhiteSpace(normalizedIngredient))
                            continue;

                        if (shoppingList.ContainsKey(normalizedIngredient))
                            shoppingList[normalizedIngredient]++;
                        else
                            shoppingList[normalizedIngredient] = 1;
                    }
                }
            }

            return shoppingList;
        }

        /// <summary>
        /// Форматирует список покупок с группировкой по категориям
        /// </summary>
        public static string FormatShoppingListWithCategories(Dictionary<string, int> shoppingList)
        {
            if (shoppingList == null || shoppingList.Count == 0)
                return "Нет запланированных рецептов на выбранный период.";

            var groupedItems = new Dictionary<ProductCategory, List<KeyValuePair<string, int>>>();

            foreach (var item in shoppingList)
            {
                ProductCategory category = GetCategory(item.Key);
                if (!groupedItems.ContainsKey(category))
                    groupedItems[category] = new List<KeyValuePair<string, int>>();
                groupedItems[category].Add(item);
            }

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("СПИСОК ПОКУПОК");
            sb.AppendLine(new string('=', 50));
            sb.AppendLine();

            // Добавляем дату
            sb.AppendLine($"Дата создания: {DateTime.Now:dd.MM.yyyy HH:mm}");
            sb.AppendLine();

            var categoryOrder = new[]
            {
                ProductCategory.Овощи,
                ProductCategory.Фрукты,
                ProductCategory.Мясо,
                ProductCategory.Рыба,
                ProductCategory.МолочныеПродукты,
                ProductCategory.Бакалея,
                ProductCategory.Напитки,
                ProductCategory.Специи,
                ProductCategory.Прочее
            };

            foreach (var category in categoryOrder)
            {
                if (groupedItems.ContainsKey(category) && groupedItems[category].Count > 0)
                {
                    string categoryName = GetCategoryDisplayName(category);
                    sb.AppendLine($"【 {categoryName} 】");
                    sb.AppendLine(new string('-', 40));

                    foreach (var item in groupedItems[category].OrderBy(x => x.Key))
                    {
                        sb.AppendLine($"  • {item.Key} — {item.Value} порц.");
                    }
                    sb.AppendLine();
                }
            }

            sb.AppendLine(new string('=', 50));
            sb.AppendLine($"Всего позиций: {shoppingList.Count}");

            return sb.ToString();
        }

        private static string GetCategoryDisplayName(ProductCategory category)
        {
            switch (category)
            {
                case ProductCategory.Овощи: return "🥕 ОВОЩИ";
                case ProductCategory.Фрукты: return "🍎 ФРУКТЫ";
                case ProductCategory.Мясо: return "🍗 МЯСО";
                case ProductCategory.Рыба: return "🐟 РЫБА";
                case ProductCategory.МолочныеПродукты: return "🥛 МОЛОЧНЫЕ ПРОДУКТЫ";
                case ProductCategory.Бакалея: return "🍚 БАКАЛЕЯ";
                case ProductCategory.Напитки: return "🥤 НАПИТКИ";
                case ProductCategory.Специи: return "🌿 СПЕЦИИ";
                default: return "📦 ПРОЧЕЕ";
            }
        }
    }
}