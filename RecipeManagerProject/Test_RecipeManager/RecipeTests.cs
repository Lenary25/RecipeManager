using Microsoft.VisualStudio.TestTools.UnitTesting;
using RecipeMealPlanner;
using System.Collections.Generic;

namespace RecipeMealPlanner.Tests
{
    [TestClass]
    public class RecipeTests
    {
        [TestMethod]
        public void Constructor_SetsNameCorrectly()
        {
            // Arrange
            string expectedName = "Борщ";

            // Act
            var recipe = new Recipe(expectedName, "Описание", new List<string>(), new List<string>(), 100);

            // Assert
            Assert.AreEqual(expectedName, recipe.Name);
        }

        [TestMethod]
        public void Constructor_SetsDescriptionCorrectly()
        {
            // Arrange
            string expectedDescription = "Традиционный суп";

            // Act
            var recipe = new Recipe("Борщ", expectedDescription, new List<string>(), new List<string>(), 100);

            // Assert
            Assert.AreEqual(expectedDescription, recipe.Description);
        }

        [TestMethod]
        public void Constructor_SetsCaloriesCorrectly()
        {
            // Arrange
            int expectedCalories = 350;

            // Act
            var recipe = new Recipe("Борщ", "Описание", new List<string>(), new List<string>(), expectedCalories);

            // Assert
            Assert.AreEqual(expectedCalories, recipe.Calories);
        }

        [TestMethod]
        public void Constructor_WithIngredients_SetsIngredientsCorrectly()
        {
            // Arrange
            var ingredients = new List<string> { "Свекла", "Капуста", "Мясо" };

            // Act
            var recipe = new Recipe("Борщ", "Описание", ingredients, new List<string>(), 100);

            // Assert
            Assert.AreEqual(ingredients, recipe.Ingredients);
            Assert.AreEqual(3, recipe.Ingredients.Count);
            Assert.AreEqual("Свекла", recipe.Ingredients[0]);
            Assert.AreEqual("Капуста", recipe.Ingredients[1]);
            Assert.AreEqual("Мясо", recipe.Ingredients[2]);
        }

        [TestMethod]
        public void Constructor_WithInstructions_SetsInstructionsCorrectly()
        {
            // Arrange
            var instructions = new List<string> { "Сварить бульон", "Добавить овощи" };

            // Act
            var recipe = new Recipe("Борщ", "Описание", new List<string>(), instructions, 100);

            // Assert
            Assert.AreEqual(instructions, recipe.Instructions);
            Assert.AreEqual(2, recipe.Instructions.Count);
            Assert.AreEqual("Сварить бульон", recipe.Instructions[0]);
            Assert.AreEqual("Добавить овощи", recipe.Instructions[1]);
        }

        [TestMethod]
        public void Constructor_NullIngredients_CreatesEmptyList()
        {
            // Act
            var recipe = new Recipe("Тест", "Описание", null, new List<string>(), 100);

            // Assert
            Assert.IsNotNull(recipe.Ingredients);
            Assert.AreEqual(0, recipe.Ingredients.Count);
        }

        [TestMethod]
        public void Constructor_NullInstructions_CreatesEmptyList()
        {
            // Act
            var recipe = new Recipe("Тест", "Описание", new List<string>(), null, 100);

            // Assert
            Assert.IsNotNull(recipe.Instructions);
            Assert.AreEqual(0, recipe.Instructions.Count);
        }

        [TestMethod]
        public void Constructor_EmptyIngredients_CreatesEmptyList()
        {
            // Act
            var recipe = new Recipe("Тест", "Описание", new List<string>(), new List<string>(), 100);

            // Assert
            Assert.AreEqual(0, recipe.Ingredients.Count);
        }

        [TestMethod]
        public void Constructor_EmptyInstructions_CreatesEmptyList()
        {
            // Act
            var recipe = new Recipe("Тест", "Описание", new List<string>(), new List<string>(), 100);

            // Assert
            Assert.AreEqual(0, recipe.Instructions.Count);
        }

        [TestMethod]
        public void ToString_ReturnsName()
        {
            // Arrange
            var recipe = new Recipe("Борщ", "Описание", new List<string>(), new List<string>(), 100);

            // Act
            string result = recipe.ToString();

            // Assert
            Assert.AreEqual("Борщ", result);
        }

        [TestMethod]
        public void Property_Name_CanBeChanged()
        {
            // Arrange
            var recipe = new Recipe("Старое", "Описание", new List<string>(), new List<string>(), 100);

            // Act
            recipe.Name = "Новое";

            // Assert
            Assert.AreEqual("Новое", recipe.Name);
        }

        [TestMethod]
        public void Property_Description_CanBeChanged()
        {
            // Arrange
            var recipe = new Recipe("Тест", "Старое", new List<string>(), new List<string>(), 100);

            // Act
            recipe.Description = "Новое";

            // Assert
            Assert.AreEqual("Новое", recipe.Description);
        }

        [TestMethod]
        public void Property_Calories_CanBeChanged()
        {
            // Arrange
            var recipe = new Recipe("Тест", "Описание", new List<string>(), new List<string>(), 100);

            // Act
            recipe.Calories = 500;

            // Assert
            Assert.AreEqual(500, recipe.Calories);
        }

        [TestMethod]
        public void Ingredients_CanBeModifiedAfterCreation()
        {
            // Arrange
            var recipe = new Recipe("Тест", "Описание", new List<string>(), new List<string>(), 100);

            // Act
            recipe.Ingredients.Add("Новый ингредиент");

            // Assert
            Assert.AreEqual(1, recipe.Ingredients.Count);
            Assert.AreEqual("Новый ингредиент", recipe.Ingredients[0]);
        }

        [TestMethod]
        public void Instructions_CanBeModifiedAfterCreation()
        {
            // Arrange
            var recipe = new Recipe("Тест", "Описание", new List<string>(), new List<string>(), 100);

            // Act
            recipe.Instructions.Add("Новая инструкция");

            // Assert
            Assert.AreEqual(1, recipe.Instructions.Count);
            Assert.AreEqual("Новая инструкция", recipe.Instructions[0]);
        }

        [TestMethod]
        public void Recipe_WithZeroCalories_CreatesSuccessfully()
        {
            // Act
            var recipe = new Recipe("Вода", "Напиток", new List<string>(), new List<string>(), 0);

            // Assert
            Assert.AreEqual(0, recipe.Calories);
        }

        [TestMethod]
        public void Recipe_WithEmptyName_CreatesSuccessfully()
        {
            // Act
            var recipe = new Recipe("", "Описание", new List<string>(), new List<string>(), 100);

            // Assert
            Assert.AreEqual("", recipe.Name);
        }

        [TestMethod]
        public void Recipe_WithEmptyDescription_CreatesSuccessfully()
        {
            // Act
            var recipe = new Recipe("Тест", "", new List<string>(), new List<string>(), 100);

            // Assert
            Assert.AreEqual("", recipe.Description);
        }

        [TestMethod]
        public void TwoRecipes_WithSameData_AreDifferentObjects()
        {
            // Arrange
            var recipe1 = new Recipe("Борщ", "Описание", new List<string>(), new List<string>(), 100);
            var recipe2 = new Recipe("Борщ", "Описание", new List<string>(), new List<string>(), 100);

            // Assert
            Assert.AreNotSame(recipe1, recipe2);
        }
    }
}