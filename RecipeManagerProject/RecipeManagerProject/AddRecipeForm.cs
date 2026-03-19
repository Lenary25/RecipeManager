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
        private Panel scrollPanel; // Добавляем панель с прокруткой

        private List<Recipe> allRecipes;
        public Recipe SelectedRecipe { get; private set; }

        public AddRecipeForm(List<Recipe> recipes)
        {
            allRecipes = recipes;
            this.Text = "Добавление рецепта";
            this.Size = new System.Drawing.Size(550, 500);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            InitializeComponent();
        }

        private void InitializeComponent()
        {
            // Заголовок
            Label titleLabel = new Label
            {
                Text = "Добавьте рецепт в план",
                Location = new System.Drawing.Point(20, 10),
                Font = new System.Drawing.Font("Arial", 12, System.Drawing.FontStyle.Bold),
                AutoSize = true
            };

            // Радио-кнопки выбора режима
            existingRadio = new RadioButton
            {
                Text = "Выбрать существующий рецепт",
                Location = new System.Drawing.Point(20, 40),
                Checked = true
            };
            existingRadio.CheckedChanged += Radio_CheckedChanged;

            newRadio = new RadioButton
            {
                Text = "Создать новый рецепт",
                Location = new System.Drawing.Point(250, 40)
            };

            // Группа существующих рецептов
            existingGroup = new GroupBox
            {
                Text = "Существующие рецепты",
                Location = new System.Drawing.Point(20, 70),
                Size = new System.Drawing.Size(490, 80)
            };

            existingRecipesCombo = new ComboBox
            {
                Location = new System.Drawing.Point(10, 30),
                Width = 460,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            existingRecipesCombo.Items.AddRange(allRecipes.ToArray());
            if (existingRecipesCombo.Items.Count > 0)
                existingRecipesCombo.SelectedIndex = 0;
            existingGroup.Controls.Add(existingRecipesCombo);

            // СОЗДАЕМ ПАНЕЛЬ С ПРОКРУТКОЙ ДЛЯ НОВОГО РЕЦЕПТА
            scrollPanel = new Panel
            {
                Location = new System.Drawing.Point(20, 70),
                Size = new System.Drawing.Size(490, 330),
                AutoScroll = true,
                BorderStyle = BorderStyle.Fixed3D
            };

            // Группа нового рецепта (помещаем внутрь панели)
            newGroup = new GroupBox
            {
                Text = "Новый рецепт",
                Location = new System.Drawing.Point(0, 0),
                Size = new System.Drawing.Size(460, 400), // Увеличиваем высоту
                Enabled = false
            };

            int yPos = 25;

            // Название
            Label nameLabel = new Label { Text = "Название:", Location = new System.Drawing.Point(10, yPos), AutoSize = true };
            nameTextBox = new TextBox { Location = new System.Drawing.Point(10, yPos + 20), Width = 430 };
            yPos += 60;

            // Описание (многострочное)
            Label descLabel = new Label { Text = "Описание:", Location = new System.Drawing.Point(10, yPos), AutoSize = true };
            descriptionTextBox = new TextBox { Location = new System.Drawing.Point(10, yPos + 20), Width = 430, Height = 80, Multiline = true };
            yPos += 110;

            // Ингредиенты
            Label ingLabel = new Label { Text = "Ингредиенты (через запятую):", Location = new System.Drawing.Point(10, yPos), AutoSize = true };
            ingredientsTextBox = new TextBox { Location = new System.Drawing.Point(10, yPos + 20), Width = 430 };
            yPos += 50;

            // Инструкции
            Label instrLabel = new Label { Text = "Инструкции (через запятую):", Location = new System.Drawing.Point(10, yPos), AutoSize = true };
            instructionsTextBox = new TextBox { Location = new System.Drawing.Point(10, yPos + 20), Width = 430 };
            yPos += 50;

            // Калории
            Label calLabel = new Label { Text = "Калорийность:", Location = new System.Drawing.Point(10, yPos), AutoSize = true };
            caloriesNumeric = new NumericUpDown { Location = new System.Drawing.Point(10, yPos + 20), Width = 100, Maximum = 5000 };
            yPos += 50;

            // Добавляем все контролы в группу
            newGroup.Controls.AddRange(new Control[] {
                nameLabel, nameTextBox, descLabel, descriptionTextBox,
                ingLabel, ingredientsTextBox, instrLabel, instructionsTextBox,
                calLabel, caloriesNumeric
            });

            // Устанавливаем высоту группы по содержимому
            newGroup.Height = yPos + 30;

            // Добавляем группу в панель с прокруткой
            scrollPanel.Controls.Add(newGroup);

            // Кнопки (внизу формы, вне панели прокрутки)
            okButton = new Button
            {
                Text = "Добавить",
                Location = new System.Drawing.Point(150, 410),
                Size = new System.Drawing.Size(100, 35),
                BackColor = System.Drawing.Color.LightGreen,
                FlatStyle = FlatStyle.Flat
            };
            okButton.Click += OkButton_Click;

            cancelButton = new Button
            {
                Text = "Отмена",
                Location = new System.Drawing.Point(280, 410),
                Size = new System.Drawing.Size(100, 35),
                BackColor = System.Drawing.Color.LightCoral,
                FlatStyle = FlatStyle.Flat
            };
            cancelButton.Click += (s, e) => this.DialogResult = DialogResult.Cancel;

            // Добавляем все на форму
            this.Controls.Add(titleLabel);
            this.Controls.Add(existingRadio);
            this.Controls.Add(newRadio);
            this.Controls.Add(existingGroup);
            this.Controls.Add(scrollPanel); // Добавляем панель вместо newGroup
            this.Controls.Add(okButton);
            this.Controls.Add(cancelButton);
        }

        private void Radio_CheckedChanged(object sender, EventArgs e)
        {
            existingGroup.Enabled = existingRadio.Checked;
            newGroup.Enabled = newRadio.Checked;

            // Показываем/скрываем соответствующие элементы
            existingGroup.Visible = existingRadio.Checked;
            scrollPanel.Visible = newRadio.Checked;
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
                    MessageBox.Show("Выберите рецепт из списка.", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            else
            {
                if (string.IsNullOrWhiteSpace(nameTextBox.Text) ||
                    string.IsNullOrWhiteSpace(descriptionTextBox.Text) ||
                    string.IsNullOrWhiteSpace(ingredientsTextBox.Text) ||
                    string.IsNullOrWhiteSpace(instructionsTextBox.Text))
                {
                    MessageBox.Show("Заполните все поля!", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
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