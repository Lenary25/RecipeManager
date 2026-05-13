using Microsoft.VisualStudio.TestTools.UnitTesting;
using RecipeMealPlanner;
using System.Collections.Generic;

namespace RecipeMealPlanner.Tests
{
    [TestClass]
    public class RecipeTests
    {
        [TestMethod]
        public void Constructor_WithValidParameters_CreatesRecipeSuccessfully()
        {
            // Arrange
            string name = "Тестовый рецепт";
            string description = "Тестовое описание";
            var ingredients = new List<string> { "Ингредиент 1", "Ингредиент 2" };
            var instructions = new List<string> { "Инструкция 1", "Инструкция 2" };
            int calories = 350;

            // Act
            var recipe = new Recipe(name, description, ingredients, instructions, calories);

            // Assert
            Assert.AreEqual(name, recipe.Name);
            Assert.AreEqual(description, recipe.Description);
            Assert.AreEqual(ingredients.Count, recipe.Ingredients.Count);
            Assert.AreEqual(instructions.Count, recipe.Instructions.Count);
            Assert.AreEqual(calories, recipe.Calories);
        }

        [TestMethod]
        public void Constructor_WithNullIngredients_CreatesEmptyIngredientsList()
        {
            // Act
            var recipe = new Recipe("Тест", "Описание", null, new List<string>(), 100);

            // Assert
            Assert.IsNotNull(recipe.Ingredients);
            Assert.AreEqual(0, recipe.Ingredients.Count);
        }

        [TestMethod]
        public void Constructor_WithNullInstructions_CreatesEmptyInstructionsList()
        {
            // Act
            var recipe = new Recipe("Тест", "Описание", new List<string>(), null, 100);

            // Assert
            Assert.IsNotNull(recipe.Instructions);
            Assert.AreEqual(0, recipe.Instructions.Count);
        }

        [TestMethod]
        public void ToString_ReturnsRecipeName()
        {
            // Arrange
            var recipe = new Recipe("Борщ", "Описание", new List<string>(), new List<string>(), 250);

            // Act
            var result = recipe.ToString();

            // Assert
            Assert.AreEqual("Борщ", result);
        }

        [TestMethod]
        public void Properties_CanBeSetAndGet()
        {
            // Arrange
            var recipe = new Recipe("Старое имя", "Старое описание", new List<string>(), new List<string>(), 100);

            // Act
            recipe.Name = "Новое имя";
            recipe.Description = "Новое описание";
            recipe.Calories = 500;
            recipe.Ingredients.Add("Новый ингредиент");
            recipe.Instructions.Add("Новая инструкция");

            // Assert
            Assert.AreEqual("Новое имя", recipe.Name);
            Assert.AreEqual("Новое описание", recipe.Description);
            Assert.AreEqual(500, recipe.Calories);
            Assert.AreEqual(1, recipe.Ingredients.Count);
            Assert.AreEqual(1, recipe.Instructions.Count);
        }

        [TestMethod]
        public void SameNameRecipes_AreDifferentObjects()
        {
            // Arrange
            var recipe1 = new Recipe("Борщ", "Описание1", new List<string>(), new List<string>(), 100);
            var recipe2 = new Recipe("Борщ", "Описание2", new List<string>(), new List<string>(), 200);

            // Act & Assert
            Assert.AreNotSame(recipe1, recipe2);
            Assert.AreEqual(recipe1.Name, recipe2.Name);
            Assert.AreNotEqual(recipe1.Calories, recipe2.Calories);
        }

        [TestMethod]
        public void Recipe_CanBeUsedInDictionary()
        {
            // Arrange
            var recipe1 = new Recipe("Борщ", "Описание", new List<string>(), new List<string>(), 100);
            var recipe2 = new Recipe("Суп", "Описание", new List<string>(), new List<string>(), 150);
            var dict = new Dictionary<string, Recipe>();

            // Act
            dict.Add(recipe1.Name, recipe1);
            dict.Add(recipe2.Name, recipe2);

            // Assert
            Assert.IsTrue(dict.ContainsKey("Борщ"));
            Assert.IsTrue(dict.ContainsKey("Суп"));
            Assert.AreEqual(recipe1, dict["Борщ"]);
            Assert.AreEqual(recipe2, dict["Суп"]);
        }

        [TestMethod]
        public void Recipe_WithEmptyIngredients_ToStringWorks()
        {
            // Arrange
            var recipe = new Recipe("Пустой рецепт", "Описание", new List<string>(), new List<string>(), 100);

            // Act
            var result = recipe.ToString();

            // Assert
            Assert.AreEqual("Пустой рецепт", result);
        }

        [TestMethod]
        public void Recipe_WithManyIngredients_StoresAllIngredients()
        {
            // Arrange
            var ingredients = new List<string> { "Ингр1", "Ингр2", "Ингр3", "Ингр4", "Ингр5" };
            var recipe = new Recipe("Сложный рецепт", "Описание", ingredients, new List<string>(), 500);

            // Act & Assert
            Assert.AreEqual(5, recipe.Ingredients.Count);
            Assert.IsTrue(recipe.Ingredients.Contains("Ингр3"));
        }

        [TestMethod]
        public void Recipe_WithManyInstructions_StoresAllInstructions()
        {
            // Arrange
            var instructions = new List<string> { "Шаг1", "Шаг2", "Шаг3", "Шаг4", "Шаг5" };
            var recipe = new Recipe("Сложный рецепт", "Описание", new List<string>(), instructions, 500);

            // Act & Assert
            Assert.AreEqual(5, recipe.Instructions.Count);
            Assert.IsTrue(recipe.Instructions.Contains("Шаг3"));
        }
    }
}