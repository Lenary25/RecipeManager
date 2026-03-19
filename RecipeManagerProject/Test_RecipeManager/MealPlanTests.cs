using Microsoft.VisualStudio.TestTools.UnitTesting;
using RecipeMealPlanner;
using System.Collections.Generic;
using System.Windows.Forms;

namespace RecipeMealPlanner.Tests
{
    [TestClass]
    public class MealPlanTests
    {
        [TestMethod]
        public void Constructor_WithListView_CreatesInstance()
        {
            // Arrange
            var listView = new ListView();

            // Act
            var mealPlan = new MealPlan(listView);

            // Assert
            Assert.IsNotNull(mealPlan);
        }

        [TestMethod]
        public void Constructor_WithNullListView_CreatesInstance()
        {
            // Act
            var mealPlan = new MealPlan(null);

            // Assert
            Assert.IsNotNull(mealPlan);
        }

        [TestMethod]
        public void Constructor_InitializesAllRecipes()
        {
            // Arrange
            var mealPlan = new MealPlan(null);

            // Act
            var recipes = mealPlan.GetAllRecipes();

            // Assert
            Assert.IsNotNull(recipes);
        }

        [TestMethod]
        public void GetAllRecipes_ReturnsList()
        {
            // Arrange
            var mealPlan = new MealPlan(null);

            // Act
            var result = mealPlan.GetAllRecipes();

            // Assert
            Assert.IsInstanceOfType(result, typeof(List<Recipe>));
        }

        [TestMethod]
        public void GetAllRecipes_MultipleCalls_ReturnDifferentLists()
        {
            // Arrange
            var mealPlan = new MealPlan(null);

            // Act
            var list1 = mealPlan.GetAllRecipes();
            var list2 = mealPlan.GetAllRecipes();

            // Assert
            Assert.AreNotSame(list1, list2);
        }

        [TestMethod]
        public void Constructor_LoadsSampleRecipes()
        {
            // Arrange & Act
            var mealPlan = new MealPlan(null);
            var recipes = mealPlan.GetAllRecipes();

            // Assert - проверяем что есть хотя бы 3 рецепта (из LoadSampleRecipes)
            Assert.IsTrue(recipes.Count >= 3);
        }

        [TestMethod]
        public void AddRecipeToPlan_SameDate_DifferentRecipes_BothAddedToCollection()
        {
            // Arrange
            var mealPlan = new MealPlan(null);
            var date = new System.DateTime(2026, 3, 20);

            var recipe1 = new Recipe("Борщ", "Традиционный суп",
                new List<string> { "Свекла", "Капуста", "Мясо" },
                new List<string> { "Сварить бульон", "Добавить овощи" }, 350);

            var recipe2 = new Recipe("Суп", "Легкий суп",
                new List<string> { "Курица", "Лапша", "Морковь" },
                new List<string> { "Сварить бульон", "Добавить лапшу" }, 250);

            // Act
            // Добавляем первый рецепт
            mealPlan.AddRecipeToPlan(date, recipe1);

            // Добавляем второй рецепт на ТУ ЖЕ дату
            mealPlan.AddRecipeToPlan(date, recipe2);

            // Assert
            // Проверяем, что оба рецепта есть в общей коллекции
            var allRecipes = mealPlan.GetAllRecipes();

            bool foundRecipe1 = false;
            bool foundRecipe2 = false;

            foreach (var recipe in allRecipes)
            {
                if (recipe.Name == "Борщ" && recipe.Calories == 350)
                    foundRecipe1 = true;
                if (recipe.Name == "Суп" && recipe.Calories == 250)
                    foundRecipe2 = true;
            }

            Assert.IsTrue(foundRecipe1, "Первый рецепт должен быть в коллекции");
            Assert.IsTrue(foundRecipe2, "Второй рецепт должен быть в коллекции");
        }

        [TestMethod]
        public void AddRecipeToPlan_DifferentDates_BothAddedToCollection()
        {
            // Arrange
            var mealPlan = new MealPlan(null);
            var date1 = new System.DateTime(2026, 3, 20);
            var date2 = new System.DateTime(2026, 3, 21);

            var recipe1 = new Recipe("Борщ", "Традиционный суп",
                new List<string> { "Свекла", "Капуста", "Мясо" },
                new List<string> { "Сварить бульон", "Добавить овощи" }, 350);

            var recipe2 = new Recipe("Суп", "Легкий суп",
                new List<string> { "Курица", "Лапша", "Морковь" },
                new List<string> { "Сварить бульон", "Добавить лапшу" }, 250);

            // Act
            mealPlan.AddRecipeToPlan(date1, recipe1);
            mealPlan.AddRecipeToPlan(date2, recipe2);

            // Assert
            var allRecipes = mealPlan.GetAllRecipes();

            bool foundRecipe1 = false;
            bool foundRecipe2 = false;

            foreach (var recipe in allRecipes)
            {
                if (recipe.Name == "Борщ" && recipe.Calories == 350)
                    foundRecipe1 = true;
                if (recipe.Name == "Суп" && recipe.Calories == 250)
                    foundRecipe2 = true;
            }

            Assert.IsTrue(foundRecipe1, "Первый рецепт должен быть в коллекции");
            Assert.IsTrue(foundRecipe2, "Второй рецепт должен быть в коллекции");
        }

        [TestMethod]
        public void AddRecipeToPlan_SameRecipeTwice_AddedOnce()
        {
            // Arrange
            var mealPlan = new MealPlan(null);
            var date1 = new System.DateTime(2026, 3, 20);
            var date2 = new System.DateTime(2026, 3, 21);

            var recipe = new Recipe("Борщ", "Традиционный суп",
                new List<string> { "Свекла", "Капуста", "Мясо" },
                new List<string> { "Сварить бульон", "Добавить овощи" }, 350);

            // Act
            mealPlan.AddRecipeToPlan(date1, recipe);
            mealPlan.AddRecipeToPlan(date2, recipe);

            // Assert
            var allRecipes = mealPlan.GetAllRecipes();

            int count = 0;
            foreach (var r in allRecipes)
            {
                if (r.Name == "Борщ" && r.Calories == 350)
                    count++;
            }

            Assert.AreEqual(1, count, "Один и тот же рецепт должен быть добавлен только один раз");
        }
    }
}