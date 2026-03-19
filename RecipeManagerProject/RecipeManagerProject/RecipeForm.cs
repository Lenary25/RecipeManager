using System;
using System.Text;
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
            this.Size = new System.Drawing.Size(550, 450);
            this.StartPosition = FormStartPosition.CenterParent;

            InitializeComponent(recipe);
        }

        private void InitializeComponent(Recipe recipe)
        {
            // Заголовок
            Label titleLabel = new Label
            {
                Text = $"Информация о рецепте \"{recipe.Name}\"",
                Location = new System.Drawing.Point(20, 15),
                Font = new System.Drawing.Font("Arial", 12, System.Drawing.FontStyle.Bold),
                AutoSize = true
            };

            // Текстовое поле с информацией
            infoTextBox = new TextBox
            {
                Location = new System.Drawing.Point(20, 50),
                Size = new System.Drawing.Size(490, 320),
                Multiline = true,
                ReadOnly = true,
                ScrollBars = ScrollBars.Vertical,
                Font = new System.Drawing.Font("Consolas", 10),
                BackColor = System.Drawing.Color.White
            };

            // Формируем текст рецепта
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"НАЗВАНИЕ: {recipe.Name}");
            sb.AppendLine(new string('-', 50));
            sb.AppendLine($"ОПИСАНИЕ: {recipe.Description}");
            sb.AppendLine($"КАЛОРИЙНОСТЬ: {recipe.Calories} ккал");
            sb.AppendLine(new string('-', 50));
            sb.AppendLine("ИНГРЕДИЕНТЫ:");
            foreach (var ing in recipe.Ingredients)
                sb.AppendLine($"  • {ing}");
            sb.AppendLine(new string('-', 50));
            sb.AppendLine("ИНСТРУКЦИИ:");
            foreach (var inst in recipe.Instructions)
                sb.AppendLine($"  • {inst}");

            infoTextBox.Text = sb.ToString();

            // Кнопка закрытия
            closeButton = new Button
            {
                Text = "Закрыть",
                Location = new System.Drawing.Point(220, 380),
                Size = new System.Drawing.Size(100, 30)
            };
            closeButton.Click += (s, e) => this.Close();

            this.Controls.Add(titleLabel);
            this.Controls.Add(infoTextBox);
            this.Controls.Add(closeButton);
        }
    }
}