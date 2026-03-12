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
            this.Width = 500;
            this.Height = 400;
            this.StartPosition = FormStartPosition.CenterScreen;

            InitializeComponent();
            mealPlan = new MealPlan(listView);
        }

        private void InitializeComponent()
        {
            // Метка для даты
            Label dateLabel = new Label
            {
                Text = "Дата:",
                Location = new System.Drawing.Point(10, 15),
                AutoSize = true
            };

            // Выбор даты
            datePicker = new DateTimePicker
            {
                Location = new System.Drawing.Point(60, 12),
                Width = 150,
                Format = DateTimePickerFormat.Short
            };

            // Список для отображения плана
            listView = new ListView
            {
                Location = new System.Drawing.Point(10, 40),
                Size = new System.Drawing.Size(460, 250),
                View = View.Details,
                FullRowSelect = true,
                MultiSelect = false
            };
            listView.Columns.Add("Дата", 100);
            listView.Columns.Add("Блюдо", 350);

            // Кнопка добавления (из варианта 26)
            addRecipeButton = new Button
            {
                Location = new System.Drawing.Point(10, 300),
                Text = "Добавить рецепт",
                Size = new System.Drawing.Size(100, 25)
            };
            addRecipeButton.Click += AddRecipeButton_Click;

            // Кнопка удаления (из варианта 26)
            removeRecipeButton = new Button
            {
                Location = new System.Drawing.Point(120, 300),
                Text = "Удалить рецепт",
                Size = new System.Drawing.Size(100, 25)
            };
            removeRecipeButton.Click += RemoveRecipeButton_Click;

            // Кнопка поиска (из варианта 26)
            searchRecipeButton = new Button
            {
                Location = new System.Drawing.Point(230, 300),
                Text = "Поиск рецепта",
                Size = new System.Drawing.Size(100, 25)
            };
            searchRecipeButton.Click += SearchRecipeButton_Click;

            // Добавляем контролы
            this.Controls.Add(dateLabel);
            this.Controls.Add(datePicker);
            this.Controls.Add(listView);
            this.Controls.Add(addRecipeButton);
            this.Controls.Add(removeRecipeButton);
            this.Controls.Add(searchRecipeButton);
        }

        private void AddRecipeButton_Click(object sender, EventArgs e)
        {
            // Вызов формы добавления рецепта
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
                MessageBox.Show("Сначала выберите рецепт для удаления.");
                return;
            }

            DateTime selectedDate = (DateTime)listView.SelectedItems[0].Tag;
            mealPlan.RemoveRecipeFromPlan(selectedDate);
        }

        private void SearchRecipeButton_Click(object sender, EventArgs e)
        {
            // Вызов формы поиска
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
                    MessageBox.Show("Рецепт не найден.");
                }
            }
        }
    }
}