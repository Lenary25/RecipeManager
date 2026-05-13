using System;
using System.Collections.Generic;
using System.Drawing;
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
        private Button generateShoppingListButton;
        private DateTimePicker datePicker;

        public MealPlanForm()
        {
            this.Text = "Планирование меню";
            this.Size = new Size(565, 600);
            this.MinimumSize = new Size(565, 450);
            this.MaximumSize = new Size(565, 800);
            this.StartPosition = FormStartPosition.CenterScreen;

            InitializeComponent();
            mealPlan = new MealPlan(listView);
        }

        private void InitializeComponent()
        {
            Label dateLabel = new Label
            {
                Text = "Выберите дату:",
                Location = new Point(20, 20),
                Font = new Font("Arial", 10),
                AutoSize = true,
                Anchor = AnchorStyles.Top | AnchorStyles.Left
            };

            datePicker = new DateTimePicker
            {
                Location = new Point(140, 18),
                Width = 150,
                Format = DateTimePickerFormat.Short,
                Anchor = AnchorStyles.Top | AnchorStyles.Left,
                Name = "datePicker"
            };

            Label listCaptionLabel = new Label
            {
                Text = "Список рецептов по датам",
                Location = new Point(20, 45),
                Font = new Font("Arial", 10, FontStyle.Bold),
                AutoSize = true,
                Anchor = AnchorStyles.Top | AnchorStyles.Left
            };

            listView = new ListView
            {
                Location = new Point(20, 75),
                Size = new Size(510, 435),
                MaximumSize = new Size(510, 800),
                View = View.Details,
                FullRowSelect = true,
                GridLines = true,
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
                Name = "listView"  // ДОБАВЛЕНО!
            };
            listView.Columns.Add("Дата", 150);
            listView.Columns.Add("Блюдо", 356);
            listView.MouseDoubleClick += ListView_MouseDoubleClick;

            addRecipeButton = new Button
            {
                Location = new Point(20, 520),
                Text = "Добавить рецепт",
                Size = new Size(120, 35),
                BackColor = Color.LightGreen,
                FlatStyle = FlatStyle.Flat,
                Anchor = AnchorStyles.Bottom | AnchorStyles.Left,
                Name = "addRecipeButton"
            };
            addRecipeButton.Click += AddRecipeButton_Click;

            removeRecipeButton = new Button
            {
                Location = new Point(150, 520),
                Text = "Удалить рецепт",
                Size = new Size(120, 35),
                BackColor = Color.LightCoral,
                FlatStyle = FlatStyle.Flat,
                Anchor = AnchorStyles.Bottom | AnchorStyles.Left,
                Name = "removeRecipeButton"
            };
            removeRecipeButton.Click += RemoveRecipeButton_Click;

            searchRecipeButton = new Button
            {
                Location = new Point(280, 520),
                Text = "Поиск рецепта",
                Size = new Size(120, 35),
                BackColor = Color.LightBlue,
                FlatStyle = FlatStyle.Flat,
                Anchor = AnchorStyles.Bottom | AnchorStyles.Left,
                Name = "searchRecipeButton"
            };
            searchRecipeButton.Click += SearchRecipeButton_Click;

            generateShoppingListButton = new Button
            {
                Location = new Point(410, 520),
                Text = "Список покупок",
                Size = new Size(120, 35),
                BackColor = Color.LightYellow,
                FlatStyle = FlatStyle.Flat,
                Anchor = AnchorStyles.Bottom | AnchorStyles.Left,
                Name = "generateShoppingListButton"
            };
            generateShoppingListButton.Click += GenerateShoppingListButton_Click;

            this.Controls.Add(dateLabel);
            this.Controls.Add(datePicker);
            this.Controls.Add(listCaptionLabel);
            this.Controls.Add(listView);
            this.Controls.Add(addRecipeButton);
            this.Controls.Add(removeRecipeButton);
            this.Controls.Add(searchRecipeButton);
            this.Controls.Add(generateShoppingListButton);
        }

        private void ListView_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (listView.SelectedItems.Count == 0) return;

            string dateString = listView.SelectedItems[0].Text;
            if (DateTime.TryParse(dateString, out DateTime selectedDate))
            {
                Recipe recipe = mealPlan.GetRecipeByDate(selectedDate);
                if (recipe != null)
                {
                    RecipeForm recipeForm = new RecipeForm(recipe, false);
                    recipeForm.ShowDialog();
                }
            }
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
                string query = searchForm.SearchQuery;
                List<Recipe> foundRecipes = mealPlan.SearchRecipeByName(query);

                if (foundRecipes.Count == 0)
                {
                    MessageBox.Show($"Рецепты по запросу '{query}' не найдены.", "Результат поиска",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else if (foundRecipes.Count == 1)
                {
                    RecipeForm recipeForm = new RecipeForm(foundRecipes[0], false);
                    recipeForm.ShowDialog();
                }
                else
                {
                    bool continueBrowsing = true;
                    while (continueBrowsing && foundRecipes.Count > 0)
                    {
                        SearchResultsForm resultsForm = new SearchResultsForm(foundRecipes, query);
                        DialogResult resultsResult = resultsForm.ShowDialog();

                        if (resultsResult == DialogResult.OK)
                        {
                            Recipe selectedRecipe = resultsForm.SelectedRecipe;
                            if (selectedRecipe != null)
                            {
                                RecipeForm recipeForm = new RecipeForm(selectedRecipe, true);
                                DialogResult recipeResult = recipeForm.ShowDialog();

                                if (recipeResult == DialogResult.Abort)
                                {
                                    continueBrowsing = true;
                                }
                                else
                                {
                                    continueBrowsing = false;
                                }
                            }
                            else
                            {
                                continueBrowsing = false;
                            }
                        }
                        else
                        {
                            continueBrowsing = false;
                        }
                    }
                }
            }
        }

        private void GenerateShoppingListButton_Click(object sender, EventArgs e)
        {
            var plan = mealPlan.GetPlan();

            if (plan.Count == 0)
            {
                MessageBox.Show("В плане меню нет рецептов. Добавьте рецепты перед созданием списка покупок.",
                    "Нет данных", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            using (var periodForm = new SelectPeriodForm())
            {
                if (periodForm.ShowDialog() == DialogResult.OK)
                {
                    var shoppingList = ShoppingListGenerator.GenerateShoppingList(
                        plan,
                        periodForm.StartDate,
                        periodForm.EndDate
                    );

                    if (shoppingList.Count == 0)
                    {
                        MessageBox.Show("На выбранный период нет запланированных рецептов.",
                            "Нет данных", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        ShoppingListForm listForm = new ShoppingListForm(shoppingList);
                        listForm.ShowDialog();
                    }
                }
            }
        }
    }
}