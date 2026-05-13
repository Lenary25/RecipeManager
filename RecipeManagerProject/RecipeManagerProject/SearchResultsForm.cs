using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace RecipeMealPlanner
{
    public class SearchResultsForm : Form
    {
        private ListView resultsListView;
        private Button viewButton;
        private Button closeButton;

        public Recipe SelectedRecipe { get; private set; }
        public bool BackToResultsRequested { get; private set; }

        public SearchResultsForm(List<Recipe> recipes, string searchQuery)
        {
            this.Text = $"Результаты поиска: '{searchQuery}'";
            this.Size = new Size(500, 400);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            InitializeComponent(recipes);
        }

        private void InitializeComponent(List<Recipe> recipes)
        {
            resultsListView = new ListView
            {
                Location = new Point(20, 20),
                Size = new Size(440, 280),
                View = View.Details,
                FullRowSelect = true,
                GridLines = true,
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right
            };
            resultsListView.Columns.Add("Название", 200);
            resultsListView.Columns.Add("Описание", 230);

            foreach (var recipe in recipes)
            {
                ListViewItem item = new ListViewItem(recipe.Name);
                item.SubItems.Add(recipe.Description.Length > 30 ? recipe.Description.Substring(0, 30) + "..." : recipe.Description);
                item.Tag = recipe;
                resultsListView.Items.Add(item);
            }

            resultsListView.DoubleClick += (s, e) => ViewButton_Click(s, e);

            viewButton = new Button
            {
                Text = "Просмотреть рецепт",
                Location = new Point(20, 310),
                Size = new Size(140, 35),
                BackColor = Color.LightBlue,
                FlatStyle = FlatStyle.Flat,
                Anchor = AnchorStyles.Bottom | AnchorStyles.Left
            };
            viewButton.Click += ViewButton_Click;

            closeButton = new Button
            {
                Text = "Закрыть",
                Location = new Point(170, 310),
                Size = new Size(140, 35),
                BackColor = Color.LightCoral,
                FlatStyle = FlatStyle.Flat,
                Anchor = AnchorStyles.Bottom | AnchorStyles.Left
            };
            closeButton.Click += (s, e) => this.Close();

            this.Controls.Add(resultsListView);
            this.Controls.Add(viewButton);
            this.Controls.Add(closeButton);

            if (recipes.Count == 0)
            {
                Label noResultsLabel = new Label
                {
                    Text = "Ничего не найдено.",
                    AutoSize = true,
                    Location = new Point(150, 150),
                    Font = new Font("Arial", 12, FontStyle.Bold)
                };
                this.Controls.Add(noResultsLabel);
                viewButton.Enabled = false;
            }
        }

        private void ViewButton_Click(object sender, EventArgs e)
        {
            if (resultsListView.SelectedItems.Count > 0)
            {
                SelectedRecipe = resultsListView.SelectedItems[0].Tag as Recipe;
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else
            {
                MessageBox.Show("Выберите рецепт из списка.", "Внимание",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
    }
}