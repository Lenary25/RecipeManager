using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace RecipeMealPlanner
{
    /// <summary>
    /// Форма для отображения и редактирования списка покупок с группировкой по категориям
    /// </summary>
    public class ShoppingListForm : Form
    {
        private ListBox shoppingListBox;
        private Button exportTxtButton;
        private Button closeButton;
        private Button addItemButton;
        private Button removeItemButton;
        private Button editItemButton;
        private ComboBox categoryFilterCombo;

        private Dictionary<string, int> shoppingList;
        private bool isUpdatingFilter = false;

        public ShoppingListForm(Dictionary<string, int> shoppingList)
        {
            this.shoppingList = new Dictionary<string, int>(shoppingList);
            this.Text = "Список покупок";
            this.Size = new Size(605, 650);
            this.MinimumSize = new Size(605, 350);
            this.MaximumSize = new Size(605, 850);
            this.StartPosition = FormStartPosition.CenterParent;

            InitializeComponent();
            LoadCategoriesToFilter();
            RefreshShoppingList();
        }

        private void InitializeComponent()
        {
            Label filterLabel = new Label
            {
                Text = "Фильтр по категории:",
                Location = new Point(20, 15),
                AutoSize = true,
                Font = new Font("Arial", 9, FontStyle.Bold)
            };

            categoryFilterCombo = new ComboBox
            {
                Location = new Point(160, 12),
                Width = 200,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Arial", 9)
            };
            categoryFilterCombo.SelectedIndexChanged += CategoryFilterCombo_SelectedIndexChanged;

            shoppingListBox = new ListBox
            {
                Location = new Point(20, 45),
                Size = new Size(550, 480),
                Font = new Font("Consolas", 11),
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
                BorderStyle = BorderStyle.Fixed3D,
                DrawMode = DrawMode.OwnerDrawFixed,
                ItemHeight = 25
            };
            shoppingListBox.DrawItem += ShoppingListBox_DrawItem;

            addItemButton = new Button
            {
                Text = "➕ Добавить",
                Size = new Size(100, 40),
                Location = new Point(20, 540),
                BackColor = Color.LightGreen,
                FlatStyle = FlatStyle.Flat,
                Anchor = AnchorStyles.Bottom | AnchorStyles.Left
            };
            addItemButton.Click += AddItemButton_Click;

            removeItemButton = new Button
            {
                Text = "✖ Удалить",
                Size = new Size(100, 40),
                Location = new Point(130, 540),
                BackColor = Color.LightCoral,
                FlatStyle = FlatStyle.Flat,
                Anchor = AnchorStyles.Bottom | AnchorStyles.Left
            };
            removeItemButton.Click += RemoveItemButton_Click;

            editItemButton = new Button
            {
                Text = "✎ Редактировать",
                Size = new Size(110, 40),
                Location = new Point(240, 540),
                BackColor = Color.LightBlue,
                FlatStyle = FlatStyle.Flat,
                Anchor = AnchorStyles.Bottom | AnchorStyles.Left
            };
            editItemButton.Click += EditItemButton_Click;

            exportTxtButton = new Button
            {
                Text = "📄 Экспорт TXT",
                Size = new Size(110, 40),
                Location = new Point(360, 540),
                BackColor = Color.LightYellow,
                FlatStyle = FlatStyle.Flat,
                Anchor = AnchorStyles.Bottom | AnchorStyles.Left
            };
            exportTxtButton.Click += ExportTxtButton_Click;

            closeButton = new Button
            {
                Text = "Закрыть",
                Size = new Size(90, 40),
                Location = new Point(480, 540),
                BackColor = Color.LightGray,
                FlatStyle = FlatStyle.Flat,
                Anchor = AnchorStyles.Bottom | AnchorStyles.Left
            };
            closeButton.Click += (s, e) => this.Close();

            this.Controls.Add(filterLabel);
            this.Controls.Add(categoryFilterCombo);
            this.Controls.Add(shoppingListBox);
            this.Controls.Add(addItemButton);
            this.Controls.Add(removeItemButton);
            this.Controls.Add(editItemButton);
            this.Controls.Add(exportTxtButton);
            this.Controls.Add(closeButton);
        }

        /// <summary>
        /// Загружает категории в выпадающий список фильтра
        /// </summary>
        private void LoadCategoriesToFilter()
        {
            categoryFilterCombo.Items.Clear();
            categoryFilterCombo.Items.Add("Все категории");

            var groupedItems = new Dictionary<ShoppingListGenerator.ProductCategory, List<KeyValuePair<string, int>>>();

            foreach (var item in shoppingList)
            {
                var category = ShoppingListGenerator.GetCategory(item.Key);
                if (!groupedItems.ContainsKey(category))
                    groupedItems[category] = new List<KeyValuePair<string, int>>();
                groupedItems[category].Add(item);
            }

            var categoryOrder = new[]
            {
                ShoppingListGenerator.ProductCategory.Овощи,
                ShoppingListGenerator.ProductCategory.Фрукты,
                ShoppingListGenerator.ProductCategory.Мясо,
                ShoppingListGenerator.ProductCategory.Рыба,
                ShoppingListGenerator.ProductCategory.МолочныеПродукты,
                ShoppingListGenerator.ProductCategory.Бакалея,
                ShoppingListGenerator.ProductCategory.Напитки,
                ShoppingListGenerator.ProductCategory.Специи,
                ShoppingListGenerator.ProductCategory.Прочее
            };

            foreach (var category in categoryOrder)
            {
                if (groupedItems.ContainsKey(category) && groupedItems[category].Count > 0)
                {
                    categoryFilterCombo.Items.Add(GetCategoryDisplayName(category));
                }
            }

            categoryFilterCombo.SelectedIndex = 0;
        }

        /// <summary>
        /// Обновляет отображение списка покупок
        /// </summary>
        private void RefreshShoppingList()
        {
            shoppingListBox.Items.Clear();

            // Группируем по категориям
            var groupedItems = new Dictionary<ShoppingListGenerator.ProductCategory, List<KeyValuePair<string, int>>>();

            foreach (var item in shoppingList)
            {
                var category = ShoppingListGenerator.GetCategory(item.Key);
                if (!groupedItems.ContainsKey(category))
                    groupedItems[category] = new List<KeyValuePair<string, int>>();
                groupedItems[category].Add(item);
            }

            var categoryOrder = new[]
            {
                ShoppingListGenerator.ProductCategory.Овощи,
                ShoppingListGenerator.ProductCategory.Фрукты,
                ShoppingListGenerator.ProductCategory.Мясо,
                ShoppingListGenerator.ProductCategory.Рыба,
                ShoppingListGenerator.ProductCategory.МолочныеПродукты,
                ShoppingListGenerator.ProductCategory.Бакалея,
                ShoppingListGenerator.ProductCategory.Напитки,
                ShoppingListGenerator.ProductCategory.Специи,
                ShoppingListGenerator.ProductCategory.Прочее
            };

            // Получаем выбранный фильтр
            string selectedFilter = categoryFilterCombo.SelectedItem?.ToString();
            bool filterActive = selectedFilter != null && selectedFilter != "Все категории";
            ShoppingListGenerator.ProductCategory? filterCat = null;

            if (filterActive)
            {
                foreach (var category in categoryOrder)
                {
                    if (GetCategoryDisplayName(category) == selectedFilter)
                    {
                        filterCat = category;
                        break;
                    }
                }
            }

            int totalItems = 0;

            foreach (var category in categoryOrder)
            {
                if (!groupedItems.ContainsKey(category))
                    continue;

                // Если фильтр активен и не совпадает с текущей категорией - пропускаем
                if (filterActive && filterCat.HasValue && filterCat.Value != category)
                    continue;

                // Добавляем заголовок категории
                shoppingListBox.Items.Add(new CategoryHeaderItem
                {
                    Category = category,
                    IsHeader = true,
                    DisplayText = GetCategoryDisplayName(category)
                });

                // Добавляем ингредиенты категории
                foreach (var ingredient in groupedItems[category].OrderBy(x => x.Key))
                {
                    shoppingListBox.Items.Add(new ShoppingItem
                    {
                        Name = ingredient.Key,
                        Quantity = ingredient.Value,
                        Category = category
                    });
                    totalItems++;
                }

                // Добавляем разделитель (кроме последней категории)
                shoppingListBox.Items.Add(new SeparatorItem());
            }

            this.Text = $"Список покупок ({totalItems} позиций)";
        }

        private void ShoppingListBox_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index < 0)
            {
                e.DrawBackground();
                return;
            }

            e.DrawBackground();

            object item = shoppingListBox.Items[e.Index];

            if (item is CategoryHeaderItem header)
            {
                Font font = new Font("Arial", 11, FontStyle.Bold);
                Brush brush = Brushes.White;
                e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(70, 130, 200)), e.Bounds);
                e.Graphics.DrawString(header.DisplayText, font, brush, e.Bounds.X + 10, e.Bounds.Y + 5);
                font.Dispose();
            }
            else if (item is ShoppingItem shoppingItem)
            {
                Font font = new Font("Consolas", 10);
                Brush brush = Brushes.Black;
                string displayText = $"    • {shoppingItem.Name} — {shoppingItem.Quantity} порц.";
                e.Graphics.DrawString(displayText, font, brush, e.Bounds.X + 20, e.Bounds.Y + 5);
                font.Dispose();
            }
            else if (item is SeparatorItem)
            {
                e.Graphics.DrawLine(Pens.LightGray, e.Bounds.X + 15, e.Bounds.Y + 12, e.Bounds.Right - 15, e.Bounds.Y + 12);
            }

            e.DrawFocusRectangle();
        }

        private void CategoryFilterCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (isUpdatingFilter) return;

            isUpdatingFilter = true;
            RefreshShoppingList();
            isUpdatingFilter = false;
        }

        private void AddItemButton_Click(object sender, EventArgs e)
        {
            using (var dialog = new AddEditIngredientForm())
            {
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    string newIngredient = dialog.IngredientName;
                    int quantity = dialog.Quantity;

                    if (shoppingList.ContainsKey(newIngredient))
                    {
                        shoppingList[newIngredient] += quantity;
                    }
                    else
                    {
                        shoppingList[newIngredient] = quantity;
                    }

                    // Обновляем фильтр (могли появиться новые категории)
                    LoadCategoriesToFilter();
                    RefreshShoppingList();

                    MessageBox.Show($"Ингредиент '{newIngredient}' добавлен.", "Успех",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private void RemoveItemButton_Click(object sender, EventArgs e)
        {
            if (shoppingListBox.SelectedIndex >= 0)
            {
                object selected = shoppingListBox.SelectedItem;

                if (selected is ShoppingItem item)
                {
                    DialogResult result = MessageBox.Show($"Удалить ингредиент '{item.Name}'?",
                        "Подтверждение", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                    if (result == DialogResult.Yes)
                    {
                        shoppingList.Remove(item.Name);

                        // Обновляем фильтр (категория могла стать пустой)
                        LoadCategoriesToFilter();
                        RefreshShoppingList();
                    }
                }
                else
                {
                    MessageBox.Show("Выберите ингредиент для удаления (не категорию).", "Внимание",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            else
            {
                MessageBox.Show("Выберите ингредиент для удаления.", "Внимание",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void EditItemButton_Click(object sender, EventArgs e)
        {
            if (shoppingListBox.SelectedIndex >= 0)
            {
                object selected = shoppingListBox.SelectedItem;

                if (selected is ShoppingItem item)
                {
                    using (var dialog = new AddEditIngredientForm(item.Name, item.Quantity))
                    {
                        if (dialog.ShowDialog() == DialogResult.OK)
                        {
                            string newIngredient = dialog.IngredientName;
                            int newQuantity = dialog.Quantity;

                            shoppingList.Remove(item.Name);

                            if (shoppingList.ContainsKey(newIngredient))
                            {
                                shoppingList[newIngredient] += newQuantity;
                            }
                            else
                            {
                                shoppingList[newIngredient] = newQuantity;
                            }

                            // Обновляем фильтр
                            LoadCategoriesToFilter();
                            RefreshShoppingList();

                            MessageBox.Show($"Ингредиент обновлён.", "Успех",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Выберите ингредиент для редактирования (не категорию).", "Внимание",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            else
            {
                MessageBox.Show("Выберите ингредиент для редактирования.", "Внимание",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void ExportTxtButton_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog saveDialog = new SaveFileDialog())
            {
                saveDialog.Filter = "Текстовые файлы (*.txt)|*.txt|Все файлы (*.*)|*.*";
                saveDialog.DefaultExt = "txt";
                saveDialog.FileName = $"ShoppingList_{DateTime.Now:yyyyMMdd_HHmmss}.txt";

                if (saveDialog.ShowDialog() == DialogResult.OK)
                {
                    string content = ShoppingListGenerator.FormatShoppingListWithCategories(shoppingList);
                    File.WriteAllText(saveDialog.FileName, content, Encoding.UTF8);
                    MessageBox.Show("Список покупок сохранён в TXT файл.", "Успех",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private string GetCategoryDisplayName(ShoppingListGenerator.ProductCategory category)
        {
            switch (category)
            {
                case ShoppingListGenerator.ProductCategory.Овощи: return "🥕 ОВОЩИ";
                case ShoppingListGenerator.ProductCategory.Фрукты: return "🍎 ФРУКТЫ";
                case ShoppingListGenerator.ProductCategory.Мясо: return "🍗 МЯСО";
                case ShoppingListGenerator.ProductCategory.Рыба: return "🐟 РЫБА";
                case ShoppingListGenerator.ProductCategory.МолочныеПродукты: return "🥛 МОЛОЧНЫЕ ПРОДУКТЫ";
                case ShoppingListGenerator.ProductCategory.Бакалея: return "🍚 БАКАЛЕЯ";
                case ShoppingListGenerator.ProductCategory.Напитки: return "🥤 НАПИТКИ";
                case ShoppingListGenerator.ProductCategory.Специи: return "🌿 СПЕЦИИ";
                default: return "📦 ПРОЧЕЕ";
            }
        }
    }

    /// <summary>
    /// Вспомогательный класс для заголовка категории
    /// </summary>
    public class CategoryHeaderItem
    {
        public ShoppingListGenerator.ProductCategory Category { get; set; }
        public bool IsHeader { get; set; }
        public string DisplayText { get; set; }
    }

    /// <summary>
    /// Вспомогательный класс для элемента списка покупок
    /// </summary>
    public class ShoppingItem
    {
        public string Name { get; set; }
        public int Quantity { get; set; }
        public ShoppingListGenerator.ProductCategory Category { get; set; }
    }

    /// <summary>
    /// Вспомогательный класс для разделителя
    /// </summary>
    public class SeparatorItem { }
}