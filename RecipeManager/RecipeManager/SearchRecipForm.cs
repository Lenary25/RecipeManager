using System;
using System.Windows.Forms;

namespace RecipeMealPlanner
{
    public class SearchRecipeForm : Form
    {
        private TextBox searchTextBox;
        private Button okButton;
        private Button cancelButton;

        public string SearchQuery { get; private set; }

        public SearchRecipeForm()
        {
            this.Text = "Поиск рецепта";
            this.Size = new System.Drawing.Size(300, 120);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;

            InitializeComponent();
        }

        private void InitializeComponent()
        {
            Label label = new Label
            {
                Text = "Введите название рецепта:",
                Location = new System.Drawing.Point(10, 10),
                AutoSize = true
            };

            searchTextBox = new TextBox
            {
                Location = new System.Drawing.Point(10, 30),
                Width = 260
            };

            okButton = new Button
            {
                Text = "Найти",
                Location = new System.Drawing.Point(10, 60),
                Size = new System.Drawing.Size(80, 25)
            };
            okButton.Click += OkButton_Click;

            cancelButton = new Button
            {
                Text = "Отмена",
                Location = new System.Drawing.Point(100, 60),
                Size = new System.Drawing.Size(80, 25)
            };
            cancelButton.Click += (s, e) => this.DialogResult = DialogResult.Cancel;

            this.Controls.Add(label);
            this.Controls.Add(searchTextBox);
            this.Controls.Add(okButton);
            this.Controls.Add(cancelButton);
        }

        private void OkButton_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(searchTextBox.Text))
            {
                SearchQuery = searchTextBox.Text.Trim();
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else
            {
                MessageBox.Show("Введите поисковый запрос.");
            }
        }
    }
}