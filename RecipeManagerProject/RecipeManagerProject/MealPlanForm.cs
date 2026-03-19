using System;
using System.Windows.Forms;

namespace RecipeMealPlanner
{
    public class MealPlanForm : Form
    {
        private MealPlan mealPlan;
        private ListView listView;
        private Button addRecipeButton;
        private Button removeRecipeButton;
        private Button searchRecipeButton;
        private DateTimePicker datePicker;

        public MealPlanForm()
        {
            this.Text = "Планирование меню";
            this.Size = new System.Drawing.Size(600, 450);
            this.StartPosition = FormStartPosition.CenterScreen;

            InitializeComponent();
            mealPlan = new MealPlan(listView);
        }

        private void InitializeComponent()
        {
            // Метка для даты
            Label dateLabel = new Label
            {
                Text = "Выберите дату:",
                Location = new System.Drawing.Point(20, 20),
                Font = new System.Drawing.Font("Arial", 10),
                AutoSize = true
            };

            // Выбор даты
            datePicker = new DateTimePicker
            {
                Location = new System.Drawing.Point(140, 18),
                Width = 150,
                Format = DateTimePickerFormat.Short
            };

            // Список для отображения плана
            listView = new ListView
            {
                Location = new System.Drawing.Point(20, 60),
                Size = new System.Drawing.Size(540, 280),
                View = View.Details,
                FullRowSelect = true,
                GridLines = true
            };
            listView.Columns.Add("Дата", 150);
            listView.Columns.Add("Блюдо", 380);

            // Кнопка добавления
            addRecipeButton = new Button
            {
                Location = new System.Drawing.Point(20, 350),
                Text = "Добавить рецепт",
                Size = new System.Drawing.Size(120, 35),
                BackColor = System.Drawing.Color.LightGreen,
                FlatStyle = FlatStyle.Flat
            };
            addRecipeButton.Click += AddRecipeButton_Click;

            // Кнопка удаления
            removeRecipeButton = new Button
            {
                Location = new System.Drawing.Point(150, 350),
                Text = "Удалить рецепт",
                Size = new System.Drawing.Size(120, 35),
                BackColor = System.Drawing.Color.LightCoral,
                FlatStyle = FlatStyle.Flat
            };
            removeRecipeButton.Click += RemoveRecipeButton_Click;

            // Кнопка поиска
            searchRecipeButton = new Button
            {
                Location = new System.Drawing.Point(280, 350),
                Text = "Поиск рецепта",
                Size = new System.Drawing.Size(120, 35),
                BackColor = System.Drawing.Color.LightBlue,
                FlatStyle = FlatStyle.Flat
            };
            searchRecipeButton.Click += SearchRecipeButton_Click;

            // Добавляем контролы на форму
            this.Controls.Add(dateLabel);
            this.Controls.Add(datePicker);
            this.Controls.Add(listView);
            this.Controls.Add(addRecipeButton);
            this.Controls.Add(removeRecipeButton);
            this.Controls.Add(searchRecipeButton);
        }

        private void AddRecipeButton_Click(object sender, EventArgs e)
        {
            AddRecipeForm addForm = new AddRecipeForm(mealPlan.GetAllRecipes());
            if (addForm.ShowDialog() == DialogResult.OK)
            {
                mealPlan.AddRecipeToPlan(datePicker.Value.Date, addForm.SelectedRecipe);
            }
        }

        private void RemoveRecipeButton_Click(object sender, EventArgs e)
        {
            if (listView.SelectedItems.Count == 0)
            {
                MessageBox.Show("Сначала выберите рецепт для удаления.", "Внимание",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string dateString = listView.SelectedItems[0].Text;
            if (DateTime.TryParse(dateString, out DateTime selectedDate))
            {
                mealPlan.RemoveRecipeFromPlan(selectedDate);
            }
        }

        private void SearchRecipeButton_Click(object sender, EventArgs e)
        {
            SearchRecipeForm searchForm = new SearchRecipeForm();
            if (searchForm.ShowDialog() == DialogResult.OK)
            {
                Recipe foundRecipe = mealPlan.SearchRecipeByName(searchForm.SearchQuery);
                if (foundRecipe != null)
                {
                    RecipeForm recipeForm = new RecipeForm(foundRecipe);
                    recipeForm.ShowDialog();
                }
                else
                {
                    MessageBox.Show($"Рецепт '{searchForm.SearchQuery}' не найден.",
                        "Результат поиска", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }
    }
}