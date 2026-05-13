using Microsoft.VisualStudio.TestTools.UnitTesting;
using RecipeMealPlanner;
using System;
using System.Collections.Generic;

namespace RecipeMealPlanner.Tests
{
    [TestClass]
    public class ShoppingListGeneratorTests
    {
        #region Тесты для GenerateShoppingList

        [TestMethod]
        public void GenerateShoppingList_EmptyPlan_ReturnsEmptyDictionary()
        {
            // Arrange
            var emptyPlan = new Dictionary<DateTime, Recipe>();
            var startDate = new DateTime(2025, 5, 1);
            var endDate = new DateTime(2025, 5, 7);

            // Act
            var result = ShoppingListGenerator.GenerateShoppingList(emptyPlan, startDate, endDate);

            // Assert
            Assert.AreEqual(0, result.Count);
        }

        [TestMethod]
        public void GenerateShoppingList_SingleRecipe_ReturnsCorrectIngredients()
        {
            // Arrange
            var recipe = new Recipe("Тестовый рецепт", "Описание",
                new List<string> { "Мука", "Яйца", "Молоко" },
                new List<string> { "Шаг 1" }, 300);

            var plan = new Dictionary<DateTime, Recipe>
            {
                { new DateTime(2025, 5, 1), recipe }
            };
            var startDate = new DateTime(2025, 5, 1);
            var endDate = new DateTime(2025, 5, 1);

            // Act
            var result = ShoppingListGenerator.GenerateShoppingList(plan, startDate, endDate);

            // Assert
            Assert.AreEqual(3, result.Count);
            Assert.IsTrue(result.ContainsKey("Мука"));
            Assert.IsTrue(result.ContainsKey("Яйца"));
            Assert.IsTrue(result.ContainsKey("Молоко"));
            Assert.AreEqual(1, result["Мука"]);
        }

        [TestMethod]
        public void GenerateShoppingList_MultipleRecipesSameIngredient_AggregatesQuantities()
        {
            // Arrange
            var recipe1 = new Recipe("Рецепт 1", "Описание",
                new List<string> { "Мука", "Яйца" },
                new List<string> { "Шаг 1" }, 300);

            var recipe2 = new Recipe("Рецепт 2", "Описание",
                new List<string> { "Мука", "Сахар" },
                new List<string> { "Шаг 1" }, 200);

            var plan = new Dictionary<DateTime, Recipe>
            {
                { new DateTime(2025, 5, 1), recipe1 },
                { new DateTime(2025, 5, 2), recipe2 }
            };
            var startDate = new DateTime(2025, 5, 1);
            var endDate = new DateTime(2025, 5, 7);

            // Act
            var result = ShoppingListGenerator.GenerateShoppingList(plan, startDate, endDate);

            // Assert
            Assert.AreEqual(3, result.Count);
            Assert.AreEqual(2, result["Мука"]);
            Assert.AreEqual(1, result["Яйца"]);
            Assert.AreEqual(1, result["Сахар"]);
        }

        [TestMethod]
        public void GenerateShoppingList_DateOutsidePeriod_ExcludesRecipe()
        {
            // Arrange
            var recipe = new Recipe("Тестовый рецепт", "Описание",
                new List<string> { "Мука" },
                new List<string> { "Шаг 1" }, 300);

            var plan = new Dictionary<DateTime, Recipe>
            {
                { new DateTime(2025, 5, 10), recipe }
            };
            var startDate = new DateTime(2025, 5, 1);
            var endDate = new DateTime(2025, 5, 7);

            // Act
            var result = ShoppingListGenerator.GenerateShoppingList(plan, startDate, endDate);

            // Assert
            Assert.AreEqual(0, result.Count);
        }

        [TestMethod]
        public void GenerateShoppingList_DateOnBorder_IncludesRecipe()
        {
            // Arrange
            var recipe = new Recipe("Тестовый рецепт", "Описание",
                new List<string> { "Мука" },
                new List<string> { "Шаг 1" }, 300);

            var plan = new Dictionary<DateTime, Recipe>
            {
                { new DateTime(2025, 5, 1), recipe }
            };
            var startDate = new DateTime(2025, 5, 1);
            var endDate = new DateTime(2025, 5, 7);

            // Act
            var result = ShoppingListGenerator.GenerateShoppingList(plan, startDate, endDate);

            // Assert
            Assert.AreEqual(1, result.Count);
        }

        [TestMethod]
        public void GenerateShoppingList_EmptyIngredients_IgnoresEmptyIngredients()
        {
            // Arrange
            var recipe = new Recipe("Тестовый рецепт", "Описание",
                new List<string> { "Мука", "", "   ", "Яйца" },
                new List<string> { "Шаг 1" }, 300);

            var plan = new Dictionary<DateTime, Recipe>
            {
                { new DateTime(2025, 5, 1), recipe }
            };
            var startDate = new DateTime(2025, 5, 1);
            var endDate = new DateTime(2025, 5, 1);

            // Act
            var result = ShoppingListGenerator.GenerateShoppingList(plan, startDate, endDate);

            // Assert
            Assert.AreEqual(2, result.Count);
            Assert.IsTrue(result.ContainsKey("Мука"));
            Assert.IsTrue(result.ContainsKey("Яйца"));
        }

        #endregion

        #region Тесты для FormatShoppingListWithCategories

        [TestMethod]
        public void FormatShoppingList_EmptyList_ReturnsNoRecipesMessage()
        {
            // Arrange
            var emptyList = new Dictionary<string, int>();

            // Act
            var result = ShoppingListGenerator.FormatShoppingListWithCategories(emptyList);

            // Assert
            Assert.IsTrue(result.Contains("Нет запланированных рецептов"));
        }

        [TestMethod]
        public void FormatShoppingList_NonEmptyList_ContainsCategoryHeaders()
        {
            // Arrange
            var shoppingList = new Dictionary<string, int>
            {
                { "Мука", 2 },
                { "Яйца", 3 },
                { "Молоко", 1 },
                { "Курица", 1 }
            };

            // Act
            var result = ShoppingListGenerator.FormatShoppingListWithCategories(shoppingList);

            // Assert
            Assert.IsTrue(result.Contains("СПИСОК ПОКУПОК"));
        }

        [TestMethod]
        public void FormatShoppingList_ContainsCorrectQuantities()
        {
            // Arrange
            var shoppingList = new Dictionary<string, int>
            {
                { "Мука", 2 },
                { "Сахар", 1 },
                { "Яйца", 3 }
            };

            // Act
            var result = ShoppingListGenerator.FormatShoppingListWithCategories(shoppingList);

            // Assert
            Assert.IsTrue(result.Contains("Мука — 2 порц."));
            Assert.IsTrue(result.Contains("Сахар — 1 порц."));
            Assert.IsTrue(result.Contains("Яйца — 3 порц."));
        }

        #endregion

        #region Тесты для GetCategory

        [TestMethod]
        public void GetCategory_Vegetable_ReturnsVegetableCategory()
        {
            // Arrange
            string[] vegetables = { "помидор", "морковь", "картофель", "лук", "капуста", "огурец" };

            foreach (var veg in vegetables)
            {
                // Act
                var category = ShoppingListGenerator.GetCategory(veg);

                // Assert
                Assert.AreEqual(ShoppingListGenerator.ProductCategory.Овощи, category);
            }
        }

        [TestMethod]
        public void GetCategory_Fruit_ReturnsFruitCategory()
        {
            // Arrange
            string[] fruits = { "яблоко", "банан", "апельсин", "груша", "клубника", "виноград" };

            foreach (var fruit in fruits)
            {
                // Act
                var category = ShoppingListGenerator.GetCategory(fruit);

                // Assert
                Assert.AreEqual(ShoppingListGenerator.ProductCategory.Фрукты, category);
            }
        }

        [TestMethod]
        public void GetCategory_Meat_ReturnsMeatCategory()
        {
            // Arrange
            string[] meats = { "курица", "говядина", "свинина", "фарш", "колбаса", "бекон" };

            foreach (var meat in meats)
            {
                // Act
                var category = ShoppingListGenerator.GetCategory(meat);

                // Assert
                Assert.AreEqual(ShoppingListGenerator.ProductCategory.Мясо, category);
            }
        }

        [TestMethod]
        public void GetCategory_Dairy_ReturnsDairyCategory()
        {
            // Arrange
            string[] dairies = { "молоко", "сыр", "творог", "сметана", "кефир", "йогурт" };

            foreach (var dairy in dairies)
            {
                // Act
                var category = ShoppingListGenerator.GetCategory(dairy);

                // Assert
                Assert.AreEqual(ShoppingListGenerator.ProductCategory.МолочныеПродукты, category);
            }
        }

        [TestMethod]
        public void GetCategory_Grocery_ReturnsGroceryCategory()
        {
            // Arrange
            string[] groceries = { "мука", "сахар", "соль", "рис", "макароны", "хлеб" };

            foreach (var grocery in groceries)
            {
                // Act
                var category = ShoppingListGenerator.GetCategory(grocery);

                // Assert
                Assert.AreEqual(ShoppingListGenerator.ProductCategory.Бакалея, category);
            }
        }

        [TestMethod]
        public void GetCategory_UnknownIngredient_ReturnsOtherCategory()
        {
            // Arrange
            string unknown = "неизвестный ингредиент";

            // Act
            var category = ShoppingListGenerator.GetCategory(unknown);

            // Assert
            Assert.AreEqual(ShoppingListGenerator.ProductCategory.Прочее, category);
        }

        #endregion
    }
}