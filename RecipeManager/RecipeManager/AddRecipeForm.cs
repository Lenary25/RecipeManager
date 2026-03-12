using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace RecipeMealPlanner
{
    public class AddRecipeForm : Form
    {
        private ComboBox existingRecipesCombo;
        private TextBox nameTextBox;
        private TextBox descriptionTextBox;
        private TextBox ingredientsTextBox;
        private TextBox instructionsTextBox;
        private NumericUpDown caloriesNumeric;
        private Button okButton;
        private Button cancelButton;
        private RadioButton existingRadio;
        private RadioButton newRadio;
        private GroupBox existingGroup;
        private GroupBox newGroup;

        private List<Recipe> allRecipes;
        public Recipe SelectedRecipe { get; private set; }

        public AddRecipeForm(List<Recipe> recipes)
        {
            allRecipes = recipes;
            this.Text = "Добавить рецепт в план";
            this.Size = new System.Drawing.Size(450, 450);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;

            InitializeComponent();
        }

        private void InitializeComponent()
        {
            // Радио-кнопки выбора режима
            existingRadio = new RadioButton
            {
                Text = "Выбрать существующий",
                Location = new System.Drawing.Point(10, 10),
                Checked = true
            };
            existingRadio.CheckedChanged += Radio_CheckedChanged;

            newRadio = new RadioButton
            {
                Text = "Создать новый",
                Location = new System.Drawing.Point(200, 10)
            };

            // Группа существующих рецептов
            existingGroup = new GroupBox
            {
                Text = "Существующие рецепты",
                Location = new System.Drawing.Point(10, 40),
                Size = new System.Drawing.Size(410, 80)
            };

            existingRecipesCombo = new ComboBox
            {
                Location = new System.Drawing.Point(10, 25),
                Width = 380,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            existingRecipesCombo.Items.AddRange(allRecipes.ToArray());
            if (existingRecipesCombo.Items.Count > 0)
                existingRecipesCombo.SelectedIndex = 0;
            existingGroup.Controls.Add(existingRecipesCombo);

            // Группа нового рецепта
            newGroup = new GroupBox
            {
                Text = "Новый рецепт",
                Location = new System.Drawing.Point(10, 40),
                Size = new System.Drawing.Size(410, 280),
                Enabled = false
            };

            Label nameLabel = new Label { Text = "Название:", Location = new System.Drawing.Point(10, 20), AutoSize = true };
            nameTextBox = new TextBox { Location = new System.Drawing.Point(10, 40), Width = 380 };

            Label descLabel = new Label { Text = "Описание:", Location = new System.Drawing.Point(10, 70), AutoSize = true };
            descriptionTextBox = new TextBox { Location = new System.Drawing.Point(10, 90), Width = 380, Height = 50, Multiline = true };

            Label ingLabel = new Label { Text = "Ингредиенты (через запятую):", Location = new System.Drawing.Point(10, 150), AutoSize = true };
            ingredientsTextBox = new TextBox { Location = new System.Drawing.Point(10, 170), Width = 380 };

            Label instrLabel = new Label { Text = "Инструкции (через запятую):", Location = new System.Drawing.Point(10, 200), AutoSize = true };
            instructionsTextBox = new TextBox { Location = new System.Drawing.Point(10, 220), Width = 380 };

            Label calLabel = new Label { Text = "Калории:", Location = new System.Drawing.Point(10, 250), AutoSize = true };
            caloriesNumeric = new NumericUpDown { Location = new System.Drawing.Point(10, 270), Width = 100, Maximum = 5000 };

            newGroup.Controls.AddRange(new Control[] {
                nameLabel, nameTextBox, descLabel, descriptionTextBox,
                ingLabel, ingredientsTextBox, instrLabel, instructionsTextBox,
                calLabel, caloriesNumeric
            });

            // Кнопки
            okButton = new Button
            {
                Text = "Добавить в план",
                Location = new System.Drawing.Point(10, 350),
                Size = new System.Drawing.Size(150, 30)
            };
            okButton.Click += OkButton_Click;

            cancelButton = new Button
            {
                Text = "Отмена",
                Location = new System.Drawing.Point(170, 350),
                Size = new System.Drawing.Size(100, 30)
            };
            cancelButton.Click += (s, e) => this.DialogResult = DialogResult.Cancel;

            // Добавляем на форму
            this.Controls.Add(existingRadio);
            this.Controls.Add(newRadio);
            this.Controls.Add(existingGroup);
            this.Controls.Add(newGroup);
            this.Controls.Add(okButton);
            this.Controls.Add(cancelButton);
        }

        private void Radio_CheckedChanged(object sender, EventArgs e)
        {
            existingGroup.Enabled = existingRadio.Checked;
            newGroup.Enabled = newRadio.Checked;
        }

        private void OkButton_Click(object sender, EventArgs e)
        {
            if (existingRadio.Checked)
            {
                SelectedRecipe = existingRecipesCombo.SelectedItem as Recipe;
                if (SelectedRecipe != null)
                {
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Выберите рецепт из списка.");
                }
            }
            else
            {
                // Валидация
                if (string.IsNullOrWhiteSpace(nameTextBox.Text) ||
                    string.IsNullOrWhiteSpace(descriptionTextBox.Text) ||
                    string.IsNullOrWhiteSpace(ingredientsTextBox.Text) ||
                    string.IsNullOrWhiteSpace(instructionsTextBox.Text))
                {
                    MessageBox.Show("Заполните все поля!");
                    return;
                }

                SelectedRecipe = new Recipe(
                    nameTextBox.Text.Trim(),
                    descriptionTextBox.Text.Trim(),
                    ingredientsTextBox.Text.Split(',').Select(s => s.Trim()).ToList(),
                    instructionsTextBox.Text.Split(',').Select(s => s.Trim()).ToList(),
                    (int)caloriesNumeric.Value
                );

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }
    }
}