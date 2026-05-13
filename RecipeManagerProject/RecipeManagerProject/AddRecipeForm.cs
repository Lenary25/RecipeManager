using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
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
        private Panel scrollPanel;
        private List<Recipe> allRecipes;

        public Recipe SelectedRecipe { get; private set; }

        public AddRecipeForm(List<Recipe> recipes)
        {
            allRecipes = recipes;
            this.Text = "Добавление рецепта";
            this.Size = new Size(600, 550);
            this.MinimumSize = new Size(500, 400);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            InitializeComponent();
        }

        private void InitializeComponent()
        {
            Label titleLabel = new Label
            {
                Text = "Добавьте рецепт в план",
                Location = new Point(20, 10),
                Font = new Font("Arial", 12, FontStyle.Bold),
                AutoSize = true,
                Anchor = AnchorStyles.Top | AnchorStyles.Left,
                Name = "titleLabel"
            };

            existingRadio = new RadioButton
            {
                Text = "Выбрать существующий рецепт",
                Location = new Point(20, 40),
                Checked = true,
                Anchor = AnchorStyles.Top | AnchorStyles.Left,
                Name = "existingRadio"
            };
            existingRadio.CheckedChanged += Radio_CheckedChanged;

            newRadio = new RadioButton
            {
                Text = "Создать новый рецепт",
                Location = new Point(250, 40),
                Anchor = AnchorStyles.Top | AnchorStyles.Left,
                Name = "newRadio"
            };
            newRadio.CheckedChanged += Radio_CheckedChanged;

            existingGroup = new GroupBox
            {
                Text = "Существующие рецепты",
                Location = new Point(20, 70),
                Size = new Size(540, 80),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                Name = "existingGroup"
            };

            existingRecipesCombo = new ComboBox
            {
                Location = new Point(10, 30),
                Width = 520,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                Name = "existingRecipesCombo"
            };
            existingRecipesCombo.Items.AddRange(allRecipes.ToArray());
            if (existingRecipesCombo.Items.Count > 0)
                existingRecipesCombo.SelectedIndex = 0;
            existingGroup.Controls.Add(existingRecipesCombo);

            scrollPanel = new Panel
            {
                Location = new Point(20, 70),
                Size = new Size(540, 380),
                AutoScroll = true,
                BorderStyle = BorderStyle.Fixed3D,
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
                Name = "scrollPanel"
            };

            newGroup = new GroupBox
            {
                Text = "Новый рецепт",
                Location = new Point(0, 0),
                Size = new Size(510, 450),
                Enabled = false,
                Name = "newGroup"
            };

            int yPos = 25;

            Label nameLabel = new Label
            {
                Text = "Название:",
                Location = new Point(10, yPos),
                AutoSize = true,
                Name = "nameLabel"
            };

            nameTextBox = new TextBox
            {
                Location = new Point(10, yPos + 20),
                Width = 480,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                Name = "nameTextBox"
            };
            yPos += 60;

            Label descLabel = new Label
            {
                Text = "Описание:",
                Location = new Point(10, yPos),
                AutoSize = true,
                Name = "descLabel"
            };

            descriptionTextBox = new TextBox
            {
                Location = new Point(10, yPos + 20),
                Width = 480,
                Height = 80,
                Multiline = true,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                Name = "descriptionTextBox"
            };
            yPos += 110;

            Label ingLabel = new Label
            {
                Text = "Ингредиенты (через запятую):",
                Location = new Point(10, yPos),
                AutoSize = true,
                Name = "ingLabel"
            };

            ingredientsTextBox = new TextBox
            {
                Location = new Point(10, yPos + 20),
                Width = 480,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                Name = "ingredientsTextBox"
            };
            yPos += 50;

            Label instrLabel = new Label
            {
                Text = "Инструкции (через запятую):",
                Location = new Point(10, yPos),
                AutoSize = true,
                Name = "instrLabel"
            };

            instructionsTextBox = new TextBox
            {
                Location = new Point(10, yPos + 20),
                Width = 480,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                Name = "instructionsTextBox"
            };
            yPos += 50;

            Label calLabel = new Label
            {
                Text = "Калорийность:",
                Location = new Point(10, yPos),
                AutoSize = true,
                Name = "calLabel"
            };

            caloriesNumeric = new NumericUpDown
            {
                Location = new Point(10, yPos + 20),
                Width = 100,
                Maximum = 5000,
                Name = "caloriesNumeric"
            };
            yPos += 50;

            newGroup.Controls.AddRange(new Control[] {
                nameLabel, nameTextBox, descLabel, descriptionTextBox,
                ingLabel, ingredientsTextBox, instrLabel, instructionsTextBox,
                calLabel, caloriesNumeric
            });

            newGroup.Height = yPos + 30;
            scrollPanel.Controls.Add(newGroup);

            okButton = new Button
            {
                Text = "Добавить",
                Location = new Point(150, 460),
                Size = new Size(100, 35),
                BackColor = Color.LightGreen,
                FlatStyle = FlatStyle.Flat,
                Anchor = AnchorStyles.Bottom | AnchorStyles.Left,
                Name = "okButton"
            };
            okButton.Click += OkButton_Click;

            cancelButton = new Button
            {
                Text = "Отмена",
                Location = new Point(280, 460),
                Size = new Size(100, 35),
                BackColor = Color.LightCoral,
                FlatStyle = FlatStyle.Flat,
                Anchor = AnchorStyles.Bottom | AnchorStyles.Left,
                Name = "cancelButton"
            };
            cancelButton.Click += (s, e) => this.DialogResult = DialogResult.Cancel;

            this.Controls.Add(titleLabel);
            this.Controls.Add(existingRadio);
            this.Controls.Add(newRadio);
            this.Controls.Add(existingGroup);
            this.Controls.Add(scrollPanel);
            this.Controls.Add(okButton);
            this.Controls.Add(cancelButton);
        }

        private void Radio_CheckedChanged(object sender, EventArgs e)
        {
            existingGroup.Enabled = existingRadio.Checked;
            newGroup.Enabled = newRadio.Checked;
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