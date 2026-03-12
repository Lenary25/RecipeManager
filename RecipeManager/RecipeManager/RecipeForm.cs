using System;
using System.Windows.Forms;

namespace RecipeMealPlanner
{
    public class RecipeForm : Form
    {
        private TextBox infoTextBox;
        private Button closeButton;

        public RecipeForm(Recipe recipe)
        {
            this.Text = $"Рецепт: {recipe.Name}";
            this.Size = new System.Drawing.Size(500, 400);
            this.StartPosition = FormStartPosition.CenterParent;

            InitializeComponent(recipe);
        }

        private void InitializeComponent(Recipe recipe)
        {
            infoTextBox = new TextBox
            {
                Location = new System.Drawing.Point(10, 10),
                Size = new System.Drawing.Size(460, 300),
                Multiline = true,
                ReadOnly = true,
                ScrollBars = ScrollBars.Vertical,
                Text = recipe?.GetFullInfo() ?? "Информация отсутствует"
            };

            closeButton = new Button
            {
                Text = "Закрыть",
                Location = new System.Drawing.Point(200, 320),
                Size = new System.Drawing.Size(100, 30)
            };
            closeButton.Click += (s, e) => this.Close();

            this.Controls.Add(infoTextBox);
            this.Controls.Add(closeButton);
        }
    }
}