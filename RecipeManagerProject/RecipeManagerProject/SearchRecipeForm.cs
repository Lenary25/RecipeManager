using System;
using System.Drawing;
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
            this.Size = new Size(350, 150);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            InitializeComponent();
        }

        private void InitializeComponent()
        {
            Label label = new Label
            {
                Text = "Введите название рецепта:",
                Location = new Point(20, 20),
                Font = new Font("Arial", 10),
                AutoSize = true
            };

            searchTextBox = new TextBox
            {
                Location = new Point(20, 45),
                Width = 290,
                Font = new Font("Arial", 10)
            };

            okButton = new Button
            {
                Text = "Найти",
                Location = new Point(70, 80),
                Size = new Size(90, 30),
                BackColor = Color.LightBlue
            };
            okButton.Click += OkButton_Click;

            cancelButton = new Button
            {
                Text = "Отмена",
                Location = new Point(170, 80),
                Size = new Size(90, 30)
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
                MessageBox.Show("Введите название для поиска.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
    }
}