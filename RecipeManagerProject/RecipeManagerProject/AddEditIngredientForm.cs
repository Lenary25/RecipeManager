using System;
using System.Drawing;
using System.Windows.Forms;

namespace RecipeMealPlanner
{
    /// <summary>
    /// Форма для добавления или редактирования ингредиента
    /// </summary>
    public class AddEditIngredientForm : Form
    {
        private TextBox nameTextBox;
        private NumericUpDown quantityNumeric;
        private Button okButton;
        private Button cancelButton;
        private Label categoryPreviewLabel;

        public string IngredientName { get; private set; }
        public int Quantity { get; private set; }

        public AddEditIngredientForm(string currentName = "", int currentQuantity = 1)
        {
            this.Text = string.IsNullOrEmpty(currentName) ? "Добавление ингредиента" : "Редактирование ингредиента";
            this.Size = new Size(400, 220);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            InitializeComponent(currentName, currentQuantity);
        }

        private void InitializeComponent(string currentName, int currentQuantity)
        {
            Label nameLabel = new Label
            {
                Text = "Название ингредиента:",
                Location = new Point(20, 20),
                AutoSize = true,
                Font = new Font("Arial", 10)
            };

            nameTextBox = new TextBox
            {
                Location = new Point(20, 45),
                Width = 340,
                Font = new Font("Arial", 10),
                Text = currentName
            };
            nameTextBox.TextChanged += NameTextBox_TextChanged;

            Label quantityLabel = new Label
            {
                Text = "Количество (порций):",
                Location = new Point(20, 75),
                AutoSize = true,
                Font = new Font("Arial", 10)
            };

            quantityNumeric = new NumericUpDown
            {
                Location = new Point(20, 100),
                Width = 100,
                Minimum = 1,
                Maximum = 999,
                Value = currentQuantity > 0 ? currentQuantity : 1,
                Font = new Font("Arial", 10)
            };

            categoryPreviewLabel = new Label
            {
                Location = new Point(140, 100),
                AutoSize = true,
                Font = new Font("Arial", 9),
                ForeColor = Color.Gray
            };

            okButton = new Button
            {
                Text = "OK",
                Location = new Point(100, 140),
                Size = new Size(100, 35),
                BackColor = Color.LightGreen,
                FlatStyle = FlatStyle.Flat,
                DialogResult = DialogResult.OK
            };
            okButton.Click += OkButton_Click;

            cancelButton = new Button
            {
                Text = "Отмена",
                Location = new Point(210, 140),
                Size = new Size(100, 35),
                BackColor = Color.LightCoral,
                FlatStyle = FlatStyle.Flat,
                DialogResult = DialogResult.Cancel
            };

            this.Controls.Add(nameLabel);
            this.Controls.Add(nameTextBox);
            this.Controls.Add(quantityLabel);
            this.Controls.Add(quantityNumeric);
            this.Controls.Add(categoryPreviewLabel);
            this.Controls.Add(okButton);
            this.Controls.Add(cancelButton);

            UpdateCategoryPreview();
        }

        private void NameTextBox_TextChanged(object sender, EventArgs e)
        {
            UpdateCategoryPreview();
        }

        private void UpdateCategoryPreview()
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(nameTextBox.Text))
                {
                    var category = ShoppingListGenerator.GetCategory(nameTextBox.Text);
                    string categoryName = GetCategoryDisplayName(category);
                    categoryPreviewLabel.Text = $"Категория: {categoryName}";
                    categoryPreviewLabel.Visible = true;
                }
                else
                {
                    categoryPreviewLabel.Visible = false;
                }
            }
            catch (Exception ex)
            {
                categoryPreviewLabel.Text = "Категория: неизвестно";
                categoryPreviewLabel.Visible = true;
            }
        }

        private string GetCategoryDisplayName(ShoppingListGenerator.ProductCategory category)
        {
            switch (category)
            {
                case ShoppingListGenerator.ProductCategory.Овощи: return "🥕 Овощи";
                case ShoppingListGenerator.ProductCategory.Фрукты: return "🍎 Фрукты";
                case ShoppingListGenerator.ProductCategory.Мясо: return "🍗 Мясо";
                case ShoppingListGenerator.ProductCategory.Рыба: return "🐟 Рыба";
                case ShoppingListGenerator.ProductCategory.МолочныеПродукты: return "🥛 Молочные продукты";
                case ShoppingListGenerator.ProductCategory.Бакалея: return "🍚 Бакалея";
                case ShoppingListGenerator.ProductCategory.Напитки: return "🥤 Напитки";
                case ShoppingListGenerator.ProductCategory.Специи: return "🌿 Специи";
                default: return "📦 Прочее";
            }
        }

        private void OkButton_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(nameTextBox.Text))
            {
                MessageBox.Show("Введите название ингредиента.", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                this.DialogResult = DialogResult.None;
                return;
            }

            IngredientName = nameTextBox.Text.Trim();
            Quantity = (int)quantityNumeric.Value;
        }
    }
}