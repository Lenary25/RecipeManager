using System;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

namespace RecipeMealPlanner
{
    public class RecipeForm : Form
    {
        private TextBox infoTextBox;
        private Button closeButton;
        private Button backToResultsButton;
        private bool showBackButton;

        public RecipeForm(Recipe recipe, bool showBackButton = false)
        {
            this.Text = $"Рецепт: {recipe.Name}";
            this.Size = new Size(700, 550);
            this.MinimumSize = new Size(500, 380);
            this.StartPosition = FormStartPosition.CenterParent;
            this.showBackButton = showBackButton;

            InitializeComponent(recipe);
        }

        private void InitializeComponent(Recipe recipe)
        {
            Label titleLabel = new Label
            {
                Text = $"Информация о рецепте \"{recipe.Name}\"",
                Location = new Point(20, 15),
                Font = new Font("Arial", 12, FontStyle.Bold),
                AutoSize = true,
                Anchor = AnchorStyles.Top | AnchorStyles.Left
            };

            infoTextBox = new TextBox
            {
                Location = new Point(20, 50),
                Size = new Size(640, 380),
                Multiline = true,
                ReadOnly = true,
                ScrollBars = ScrollBars.Vertical,
                Font = new Font("Consolas", 10),
                BackColor = Color.White,
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right
            };

            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"НАЗВАНИЕ: {recipe.Name}");
            sb.AppendLine(new string('-', 40));
            sb.AppendLine($"ОПИСАНИЕ: {recipe.Description}");
            sb.AppendLine($"КАЛОРИЙНОСТЬ: {recipe.Calories} ккал");
            sb.AppendLine(new string('-', 40));
            sb.AppendLine("ИНГРЕДИЕНТЫ:");
            foreach (var ing in recipe.Ingredients)
                sb.AppendLine($"  • {ing}");
            sb.AppendLine(new string('-', 40));
            sb.AppendLine("ИНСТРУКЦИЯ:");
            foreach (var inst in recipe.Instructions)
                sb.AppendLine($"  • {inst}");

            infoTextBox.Text = sb.ToString();

            // Кнопка закрытия
            closeButton = new Button
            {
                Text = "Закрыть",
                Size = new Size(120, 35),
                Anchor = AnchorStyles.Top | AnchorStyles.Left,
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.LightCoral
            };
            closeButton.Click += (s, e) => this.Close();

            // Кнопка "Назад к результатам" (только если есть результаты поиска)
            if (showBackButton)
            {
                backToResultsButton = new Button
                {
                    Text = "← Назад к результатам",
                    Size = new Size(140, 35),
                    Anchor = AnchorStyles.Top | AnchorStyles.Left,
                    FlatStyle = FlatStyle.Flat,
                    BackColor = Color.LightBlue
                };
                backToResultsButton.Click += BackToResultsButton_Click;
                this.Controls.Add(backToResultsButton);
            }

            CenterButtons();
            this.Resize += (s, e) => CenterButtons();

            this.Controls.Add(titleLabel);
            this.Controls.Add(infoTextBox);
            this.Controls.Add(closeButton);
        }

        private void CenterButtons()
        {
            int totalWidth = showBackButton ? 280 : 120;
            int buttonY = this.ClientSize.Height - 55;
            int startX = (this.ClientSize.Width - totalWidth) / 2;

            if (showBackButton)
            {
                backToResultsButton.Location = new Point(startX, buttonY);
                closeButton.Location = new Point(startX + 150, buttonY);
            }
            else
            {
                closeButton.Location = new Point(startX, buttonY);
            }
        }

        private void BackToResultsButton_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Abort; // Специальный код для возврата к результатам
            this.Close();
        }
    }
}