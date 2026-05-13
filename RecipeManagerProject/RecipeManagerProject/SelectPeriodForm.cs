using System;
using System.Drawing;
using System.Windows.Forms;

namespace RecipeMealPlanner
{
    /// <summary>
    /// Форма для выбора периода для генерации списка покупок
    /// НОВАЯ ФУНКЦИЯ для варианта 26
    /// </summary>
    public class SelectPeriodForm : Form
    {
        private DateTimePicker startDatePicker;
        private DateTimePicker endDatePicker;
        private Button okButton;
        private Button cancelButton;

        public DateTime StartDate { get; private set; }
        public DateTime EndDate { get; private set; }

        public SelectPeriodForm()
        {
            this.Text = "Выбор периода для списка покупок";
            this.Size = new Size(350, 180);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            InitializeComponent();
        }

        private void InitializeComponent()
        {
            Label startLabel = new Label
            {
                Text = "Начальная дата:",
                Location = new Point(20, 20),
                AutoSize = true
            };

            startDatePicker = new DateTimePicker
            {
                Location = new Point(150, 18),
                Width = 150,
                Format = DateTimePickerFormat.Short,
                Name = "startDatePicker"  // ДОБАВЬТЕ ЭТУ СТРОКУ
            };

            Label endLabel = new Label
            {
                Text = "Конечная дата:",
                Location = new Point(20, 55),
                AutoSize = true
            };

            endDatePicker = new DateTimePicker
            {
                Location = new Point(150, 53),
                Width = 150,
                Format = DateTimePickerFormat.Short,
                Name = "endDatePicker"  // ДОБАВЬТЕ ЭТУ СТРОКУ
            };

            okButton = new Button
            {
                Text = "Создать",
                Location = new Point(50, 100),
                Size = new Size(100, 30),
                BackColor = Color.LightGreen
            };
            okButton.Click += OkButton_Click;

            cancelButton = new Button
            {
                Text = "Отмена",
                Location = new Point(170, 100),
                Size = new Size(100, 30),
                BackColor = Color.LightCoral
            };
            cancelButton.Click += (s, e) => this.DialogResult = DialogResult.Cancel;

            this.Controls.Add(startLabel);
            this.Controls.Add(startDatePicker);
            this.Controls.Add(endLabel);
            this.Controls.Add(endDatePicker);
            this.Controls.Add(okButton);
            this.Controls.Add(cancelButton);
        }

        private void OkButton_Click(object sender, EventArgs e)
        {
            StartDate = startDatePicker.Value.Date;
            EndDate = endDatePicker.Value.Date;

            if (StartDate > EndDate)
            {
                MessageBox.Show("Начальная дата не может быть позже конечной.",
                    "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}