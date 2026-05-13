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
    }
}