using FlaUI.Core;
using FlaUI.Core.AutomationElements;
using FlaUI.Core.Definitions;
using FlaUI.UIA3;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace AutoTest
{
    [TestClass]
    [DoNotParallelize]
    public class RecipeManagerUITests
    {
        private Application _app;
        private UIA3Automation _automation;
        private Window _mainWindow;
        private const string ExePath = @"D:\Содержимое\3 курс\Предметы\Тестирование\Коды\RecipeManager-main\RecipeManagerProject\RecipeManagerProject\bin\Debug\net8.0-windows\RecipeManagerProject.exe";

        #region Тестовые данные

        private static readonly Dictionary<string, List<string>> TestData = new Dictionary<string, List<string>>
        {
            ["TC_001"] = new List<string> { "Тестовое блюдо", "Тестовое описание", "Ингредиент 1, Ингредиент 2", "Инструкция 1, Инструкция 2", "100" },
            ["TC_003"] = new List<string> { "15.05.2025", "Курица с рисом", "", "", "" },
            ["TC_005"] = new List<string> { "Рецепт для удаления", "Тест", "Ингр", "Инстр", "100" },
            ["TC_007"] = new List<string> { "сыр", "", "", "", "" },
            ["TC_008"] = new List<string> { "Памагите", "", "", "", "" },
            ["TC_109"] = new List<string> { "01.05.2025", "07.05.2025", "", "", "" }
        };

        #endregion

        [TestInitialize]
        public void TestInitialize()
        {
            foreach (var p in Process.GetProcessesByName("RecipeManagerProject"))
            {
                try { p.Kill(); p.WaitForExit(1000); } catch { }
            }
            Thread.Sleep(500);

            _app = Application.Launch(ExePath);
            _automation = new UIA3Automation();

            for (int i = 0; i < 30; i++)
            {
                Thread.Sleep(500);
                try
                {
                    _mainWindow = _app.GetMainWindow(_automation);
                    if (_mainWindow != null && !string.IsNullOrEmpty(_mainWindow.Title))
                    {
                        break;
                    }
                }
                catch { }
            }
            Assert.IsNotNull(_mainWindow, "Главное окно не найдено");
            Thread.Sleep(500);
        }

        [TestCleanup]
        public void TestCleanup()
        {
            try { _app?.Kill(); } catch { }
            try { _app?.Close(); } catch { }
            if (_app != null) { _app.Dispose(); }
            if (_automation != null) { _automation.Dispose(); }
            foreach (var p in Process.GetProcessesByName("RecipeManagerProject"))
            {
                try { p.Kill(); } catch { }
            }
            Thread.Sleep(500);
        }

        #region Вспомогательные методы

        private AutomationElement FindControl(Window window, string identifier)
        {
            string target = identifier.Trim();
            string[] variants = { target, target + " ", target + "  " };

            foreach (var v in variants)
            {
                AutomationElement el = window.FindFirstDescendant(cf => cf.ByAutomationId(v));
                if (el != null) return el;
                el = window.FindFirstDescendant(cf => cf.ByName(v));
                if (el != null) return el;
            }

            var all = window.FindAllDescendants(cf =>
                cf.ByControlType(ControlType.Button)
                .Or(cf.ByControlType(ControlType.Edit))
                .Or(cf.ByControlType(ControlType.RadioButton))
                .Or(cf.ByControlType(ControlType.List))
                .Or(cf.ByControlType(ControlType.DataGrid))
                .Or(cf.ByControlType(ControlType.Table)));

            return all.FirstOrDefault(e =>
                (e.AutomationId != null && e.AutomationId.Trim() == target) ||
                (e.Name != null && e.Name.Trim() == target));
        }

        private void ClickButtonByName(string name)
        {
            AutomationElement el = FindControl(_mainWindow, name);
            Assert.IsNotNull(el, $"Кнопка '{name}' не найдена");
            el.AsButton().Click();
            Thread.Sleep(500);
        }

        private void ClickButtonByText(Window window, string text)
        {
            string target = text.Trim();
            AutomationElement btn = window.FindFirstDescendant(cf => cf.ByName(target))?.AsButton();
            if (btn == null) btn = window.FindFirstDescendant(cf => cf.ByText(target))?.AsButton();

            Assert.IsNotNull(btn, $"Кнопка с текстом '{text}' не найдена");
            btn.Click();
            Thread.Sleep(500);
        }

        private void SetTextBoxByName(Window window, string name, string value)
        {
            AutomationElement el = FindControl(window, name);
            Assert.IsNotNull(el, $"Поле '{name}' не найдено");
            el.AsTextBox().Text = value;
            Thread.Sleep(500);
        }

        private void SetNumericByName(Window window, string name, string value)
        {
            AutomationElement parent = FindControl(window, name);
            Assert.IsNotNull(parent, $"Control '{name}' не найден");
            AutomationElement edit = parent.FindFirstDescendant(cf => cf.ByControlType(ControlType.Edit));
            Assert.IsNotNull(edit, "Внутреннее поле NumericUpDown не найдено");
            edit.AsTextBox().Text = value;
            Thread.Sleep(500);
        }

        private void ClickRadioByName(Window window, string name)
        {
            AutomationElement el = FindControl(window, name);
            Assert.IsNotNull(el, $"Радиокнопка '{name}' не найдена");
            el.AsRadioButton().Click();
            Thread.Sleep(500);
        }

        /// <summary>
        /// Установка даты из строки
        /// </summary>
        private void SetDatePickerValue(string date)
        {
            string[] parts = date.Split('.');
            int day = int.Parse(parts[0]);
            int month = int.Parse(parts[1]);
            int year = int.Parse(parts[2]);
            SetDatePickerValue(day, month, year);
        }

        /// <summary>
        /// Установка даты через координаты
        /// </summary>
        private void SetDatePickerValue(int day, int month, int year)
        {
            Console.WriteLine($"Установка даты: {day:00}.{month:00}.{year}");

            AutomationElement datePickerElement = _mainWindow.FindFirstDescendant(cf => cf.ByAutomationId("datePicker"));
            if (datePickerElement == null)
            {
                datePickerElement = _mainWindow.FindFirstDescendant(cf => cf.ByName("datePicker"));
            }

            Assert.IsNotNull(datePickerElement, "DateTimePicker не найден");

            var rect = datePickerElement.BoundingRectangle;
            int x = (int)rect.Left;
            int y = (int)rect.Top;
            int width = (int)rect.Width;
            int height = (int)rect.Height;

            int dayOffset = width * 5 / 100;
            int monthOffset = width * 15 / 100;
            int yearOffset = width * 45 / 100;

            ClickAtPosition(x + dayOffset, y + height / 2);
            Thread.Sleep(200);
            System.Windows.Forms.SendKeys.SendWait(day.ToString("00"));
            Thread.Sleep(200);

            ClickAtPosition(x + monthOffset, y + height / 2);
            Thread.Sleep(200);
            System.Windows.Forms.SendKeys.SendWait(month.ToString("00"));
            Thread.Sleep(200);

            ClickAtPosition(x + yearOffset, y + height / 2);
            Thread.Sleep(200);
            System.Windows.Forms.SendKeys.SendWait(year.ToString());
            Thread.Sleep(200);

            System.Windows.Forms.SendKeys.SendWait("{ENTER}");
            Thread.Sleep(500);

            Console.WriteLine("Дата установлена");
        }

        /// <summary>
        /// Установка даты в DatePicker внутри указанной формы
        /// </summary>
        private void SetDatePickerValueInForm(Window window, string pickerName, string date)
        {
            Console.WriteLine($"Установка даты '{date}' в поле '{pickerName}'");

            string[] parts = date.Split('.');
            string day = parts[0];
            string month = parts[1];
            string year = parts[2];

            // Получаем координаты формы
            var windowRect = window.BoundingRectangle;
            int windowX = (int)windowRect.Left;
            int windowY = (int)windowRect.Top;

            Console.WriteLine($"Форма: X={windowX}, Y={windowY}");

            // Координаты полей ввода даты (относительно формы)
            // Из отладки: форма X=1104, Y=606
            // Начальная дата: примерно X=1104+130=1234, Y=606+25=631
            // Конечная дата: примерно X=1104+130=1234, Y=606+60=666
            int startDateX = windowX + 165;
            int startDateY = windowY + 55;
            int endDateX = windowX + 165;
            int endDateY = windowY + 95;

            int targetX, targetY;

            if (pickerName.Contains("start"))
            {
                targetX = startDateX;
                targetY = startDateY;
                Console.WriteLine($"Кликаем на начальную дату: X={targetX}, Y={targetY}");
            }
            else
            {
                targetX = endDateX;
                targetY = endDateY;
                Console.WriteLine($"Кликаем на конечную дату: X={targetX}, Y={targetY}");
            }

            // Кликаем на поле даты
            ClickAtPosition(targetX, targetY);
            Thread.Sleep(300);

            // Выделяем всё и удаляем
            System.Windows.Forms.SendKeys.SendWait("^a");
            Thread.Sleep(100);
            System.Windows.Forms.SendKeys.SendWait("{DELETE}");
            Thread.Sleep(100);

            // Вводим новую дату
            System.Windows.Forms.SendKeys.SendWait(date);
            Thread.Sleep(100);
            System.Windows.Forms.SendKeys.SendWait("{ENTER}");
            Thread.Sleep(500);

            Console.WriteLine($"Дата '{date}' установлена");
        }

        /// <summary>
        /// Клик по координатам
        /// </summary>
        private void ClickAtPosition(int x, int y)
        {
            var originalPos = System.Windows.Forms.Cursor.Position;
            System.Windows.Forms.Cursor.Position = new System.Drawing.Point(x, y);
            Thread.Sleep(100);

            const int MOUSEEVENTF_LEFTDOWN = 0x02;
            const int MOUSEEVENTF_LEFTUP = 0x04;

            mouse_event(MOUSEEVENTF_LEFTDOWN, 0, 0, 0, 0);
            Thread.Sleep(50);
            mouse_event(MOUSEEVENTF_LEFTUP, 0, 0, 0, 0);
            Thread.Sleep(200);

            System.Windows.Forms.Cursor.Position = originalPos;
        }

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern void mouse_event(int dwFlags, int dx, int dy, int cButtons, int dwExtraInfo);

        private Window WaitForModal(string titleContains, int timeoutMs = 12000)
        {
            int waited = 0;
            while (waited < timeoutMs)
            {
                try
                {
                    Window[] modals = _mainWindow.ModalWindows;
                    foreach (Window m in modals)
                    {
                        if (m.Title != null && m.Title.Contains(titleContains))
                        {
                            return m;
                        }
                    }
                }
                catch { }
                Thread.Sleep(500);
                waited += 500;
            }
            return null;
        }

        private void CloseModalByTitle(string titleContains)
        {
            Console.WriteLine($"Поиск окна: '{titleContains}'");

            for (int attempt = 0; attempt < 10; attempt++)
            {
                try
                {
                    Window[] modalWindows = _mainWindow.ModalWindows;
                    if (modalWindows != null && modalWindows.Length > 0)
                    {
                        foreach (Window modal in modalWindows)
                        {
                            if (modal.Title != null && modal.Title.Contains(titleContains))
                            {
                                AutomationElement okButton = modal.FindFirstDescendant(cf => cf.ByName("OK"))?.AsButton();
                                if (okButton == null) okButton = modal.FindFirstDescendant(cf => cf.ByText("OK"))?.AsButton();
                                if (okButton == null) okButton = modal.FindFirstDescendant(cf => cf.ByControlType(ControlType.Button))?.AsButton();

                                if (okButton != null)
                                {
                                    okButton.Click();
                                    Thread.Sleep(500);
                                    return;
                                }
                            }
                        }
                    }
                }
                catch { }
                Thread.Sleep(500);
            }

            for (int attempt = 0; attempt < 10; attempt++)
            {
                try
                {
                    AutomationElement desktop = _automation.GetDesktop();
                    AutomationElement[] windows = desktop.FindAllChildren(cf => cf.ByControlType(ControlType.Window));
                    foreach (AutomationElement window in windows)
                    {
                        Window w = window.AsWindow();
                        if (w.Title != null && w.Title.Contains(titleContains))
                        {
                            AutomationElement okButton = w.FindFirstDescendant(cf => cf.ByName("OK"))?.AsButton();
                            if (okButton == null) okButton = w.FindFirstDescendant(cf => cf.ByText("OK"))?.AsButton();
                            if (okButton == null) okButton = w.FindFirstDescendant(cf => cf.ByControlType(ControlType.Button))?.AsButton();

                            if (okButton != null)
                            {
                                okButton.Click();
                                Thread.Sleep(500);
                                return;
                            }
                            w.Close();
                            Thread.Sleep(500);
                            return;
                        }
                    }
                }
                catch { }
                Thread.Sleep(500);
            }
        }

        private AutomationElement GetMainListView()
        {
            for (int i = 0; i < 10; i++)
            {
                AutomationElement el = _mainWindow.FindFirstDescendant(cf => cf.ByAutomationId("listView"));
                if (el == null) el = _mainWindow.FindFirstDescendant(cf => cf.ByName("listView"));
                if (el != null) return el;
                Thread.Sleep(500);
            }
            Assert.Fail("ListView не найден");
            return null;
        }

        private int GetListItemsCount()
        {
            AutomationElement lv = GetMainListView();
            return lv.FindAllDescendants(cf => cf.ByControlType(ControlType.ListItem)).Length;
        }

        private AutomationElement FindListItemByText(string text)
        {
            AutomationElement lv = GetMainListView();
            AutomationElement[] items = lv.FindAllDescendants(cf => cf.ByControlType(ControlType.ListItem));
            foreach (AutomationElement item in items)
            {
                AutomationElement[] texts = item.FindAllChildren(cf => cf.ByControlType(ControlType.Text));
                foreach (AutomationElement t in texts)
                {
                    if (t.Name != null && t.Name.Contains(text)) return item;
                }
                if (item.Name != null && item.Name.Contains(text)) return item;
            }
            return null;
        }

        private void DoubleClickItem(AutomationElement item)
        {
            item.DoubleClick();
            Thread.Sleep(500);
        }

        private void SetEditByIndex(Window window, int index, string value)
        {
            AutomationElement[] edits = window.FindAllDescendants(cf => cf.ByControlType(ControlType.Edit)).ToArray();
            Assert.IsTrue(index < edits.Length, $"Edit с индексом {index} не найден (всего {edits.Length})");
            edits[index].AsTextBox().Text = value;
            Thread.Sleep(500);
        }

        private void ClearAllRecipes()
        {
            Console.WriteLine("Очистка всех рецептов...");

            var listView = GetMainListView();
            var items = listView.FindAllDescendants(cf => cf.ByControlType(ControlType.ListItem));

            while (items.Length > 0)
            {
                items[0].Click();
                Thread.Sleep(300);

                ClickButtonByName("removeRecipeButton");
                Thread.Sleep(500);

                CloseModalByTitle("Успех");
                Thread.Sleep(500);

                items = listView.FindAllDescendants(cf => cf.ByControlType(ControlType.ListItem));
            }

            Console.WriteLine("Все рецепты удалены");
        }

        private void EnsureRecipeInPlan(string name, string desc, string ing, string instr, string cal)
        {
            if (FindListItemByText(name) != null) return;

            ClickButtonByName("addRecipeButton");
            Thread.Sleep(500);
            Window addForm = WaitForModal("Добавление рецепта");
            Assert.IsNotNull(addForm, "Форма добавления не открылась");

            ClickRadioByName(addForm, "newRadio");
            Thread.Sleep(500);

            SetTextBoxByName(addForm, "nameTextBox", name);
            SetTextBoxByName(addForm, "descriptionTextBox", desc);
            SetTextBoxByName(addForm, "ingredientsTextBox", ing);
            SetTextBoxByName(addForm, "instructionsTextBox", instr);
            SetNumericByName(addForm, "caloriesNumeric", cal);

            ClickButtonByText(addForm, "Добавить");
            Thread.Sleep(500);
            CloseModalByTitle("Успех");
            Thread.Sleep(500);
        }

              
        /// <summary>
        /// Добавление нового рецепта на указанную дату
        /// </summary>
        private void AddNewRecipeToPlanOnDate(string date, string name, string desc, string ing, string instr, string cal)
        {
            Console.WriteLine($"Добавление нового рецепта '{name}' на дату {date}");

            SetDatePickerValue(date);
            Thread.Sleep(500);

            ClickButtonByName("addRecipeButton");
            Thread.Sleep(500);

            Window addForm = WaitForModal("Добавление рецепта");
            Assert.IsNotNull(addForm, "Форма добавления не открылась");

            ClickRadioByName(addForm, "newRadio");
            Thread.Sleep(500);

            SetTextBoxByName(addForm, "nameTextBox", name);
            SetTextBoxByName(addForm, "descriptionTextBox", desc);
            SetTextBoxByName(addForm, "ingredientsTextBox", ing);
            SetTextBoxByName(addForm, "instructionsTextBox", instr);
            SetNumericByName(addForm, "caloriesNumeric", cal);

            ClickButtonByText(addForm, "Добавить");
            Thread.Sleep(500);

            CloseModalByTitle("Успех");
            Thread.Sleep(500);
        }

        #endregion

        #region TC_001: Добавление рецепта в список рецептов

        [TestMethod]
        public void TC_001_AddNewRecipe_ShouldAppearInList()
        {
            Console.WriteLine("=== TC_001: Добавление рецепта ===");

            List<string> data = TestData["TC_001"];
            int initialCount = GetListItemsCount();

            ClickButtonByName("addRecipeButton");
            Thread.Sleep(500);

            Window addForm = WaitForModal("Добавление рецепта");
            Assert.IsNotNull(addForm, "Форма добавления не открылась");

            ClickRadioByName(addForm, "newRadio");
            Thread.Sleep(500);

            SetTextBoxByName(addForm, "nameTextBox", data[0]);
            SetTextBoxByName(addForm, "descriptionTextBox", data[1]);
            SetTextBoxByName(addForm, "ingredientsTextBox", data[2]);
            SetTextBoxByName(addForm, "instructionsTextBox", data[3]);
            SetNumericByName(addForm, "caloriesNumeric", data[4]);

            ClickButtonByText(addForm, "Добавить");
            Thread.Sleep(500);
            CloseModalByTitle("Успех");
            Thread.Sleep(500);

            int newCount = GetListItemsCount();
            Assert.IsTrue(newCount > initialCount, "Рецепт не добавлен в список");
            Assert.IsNotNull(FindListItemByText(data[0]), $"Рецепт '{data[0]}' не найден в списке");

            Console.WriteLine("=== TC_001 УСПЕШНО ЗАВЕРШЁН ===");
        }

        #endregion

        #region TC_002: Проверка хранения информации о рецепте

        [TestMethod]
        public void TC_002_ViewRecipeDetails_ShouldShowCorrectData()
        {
            Console.WriteLine("=== TC_002: Проверка информации о рецепте ===");

            List<string> data = new List<string> { "Тестовый рецепт для TC002", "Описание TC002", "Ингр TC002", "Инстр TC002", "200" };
            EnsureRecipeInPlan(data[0], data[1], data[2], data[3], data[4]);

            AutomationElement item = FindListItemByText(data[0]);
            Assert.IsNotNull(item, $"Рецепт '{data[0]}' не найден");

            DoubleClickItem(item);
            Thread.Sleep(500);

            Window recipeForm = WaitForModal("Рецепт:", 500);
            Assert.IsNotNull(recipeForm, "Окно рецепта не открылось");

            AutomationElement infoBox = recipeForm.FindFirstDescendant(cf => cf.ByControlType(ControlType.Edit));
            Assert.IsNotNull(infoBox, "infoTextBox не найден");

            string infoText = infoBox.AsTextBox().Text;
            StringAssert.Contains(infoText, data[0], "Название рецепта не найдено");
            StringAssert.Contains(infoText, data[4], "Калорийность не найдена");

            ClickButtonByText(recipeForm, "Закрыть");
            Thread.Sleep(500);

            Console.WriteLine("=== TC_002 УСПЕШНО ЗАВЕРШЁН ===");
        }

        #endregion

        #region TC_003: Добавление рецепта на определённую дату

        [TestMethod]
        public void TC_003_AddRecipeToSpecificDate_ShouldSucceed()
        {
            Console.WriteLine("=== TC_003: Добавление рецепта на определённую дату ===");

            List<string> data = TestData["TC_003"];
            string targetDate = data[0];
            string recipeName = data[1];

            AddNewRecipeToPlanOnDate(targetDate, recipeName, "Тестовое описание", "Ингредиенты", "Инструкции", "100");

            AutomationElement item = FindListItemByText(targetDate);
            if (item == null)
            {
                item = FindListItemByText(recipeName);
            }
            Assert.IsNotNull(item, $"Рецепт на дату {targetDate} не найден");

            Console.WriteLine("=== TC_003 УСПЕШНО ЗАВЕРШЁН ===");
        }

        #endregion

        #region TC_004: Ошибка при добавлении рецепта на повторную дату

        [TestMethod]
        public void TC_004_DuplicateDateError_ShouldShowErrorMessage()
        {
            Console.WriteLine("=== TC_004: Ошибка при добавлении на повторную дату ===");

            string todayDate = DateTime.Today.ToString("dd.MM.yyyy");

            AddNewRecipeToPlanOnDate(todayDate, "Макароны с сыром", "Описание", "Ингредиенты", "Инструкции", "100");
            Thread.Sleep(500);

            int itemsBefore = GetListItemsCount();

            AddNewRecipeToPlanOnDate(todayDate, "Курица с рисом", "Описание", "Ингредиенты", "Инструкции", "200");
            Thread.Sleep(500);

            Window errorWindow = WaitForModal("Ошибка", 500);
            Assert.IsNotNull(errorWindow, "Окно с ошибкой не появилось");

            CloseModalByTitle("Ошибка");

            int itemsAfter = GetListItemsCount();
            Assert.AreEqual(itemsBefore, itemsAfter, "Количество рецептов изменилось");

            Console.WriteLine("=== TC_004 УСПЕШНО ЗАВЕРШЁН ===");
        }

        #endregion

        #region TC_005: Удаление существующего рецепта

        [TestMethod]
        public void TC_005_DeleteExistingRecipe_ShouldSucceed()
        {
            Console.WriteLine("=== TC_005: Удаление рецепта ===");

            List<string> data = TestData["TC_005"];
            EnsureRecipeInPlan(data[0], data[1], data[2], data[3], data[4]);
            Thread.Sleep(500);

            AutomationElement item = FindListItemByText(data[0]);
            Assert.IsNotNull(item, "Рецепт для удаления не найден");

            item.Click();
            Thread.Sleep(500);

            ClickButtonByName("removeRecipeButton");
            Thread.Sleep(500);
            CloseModalByTitle("Успех");
            Thread.Sleep(500);

            Assert.IsNull(FindListItemByText(data[0]), "Рецепт не удалён из плана");

            Console.WriteLine("=== TC_005 УСПЕШНО ЗАВЕРШЁН ===");
        }

        #endregion

        #region TC_006: Ошибка при удалении без выбора

        [TestMethod]
        public void TC_006_DeleteWithoutSelection_ShouldShowError()
        {
            Console.WriteLine("=== TC_006: Ошибка при удалении без выбора ===");

            AutomationElement listView = GetMainListView();
            listView.Click();
            Thread.Sleep(500);

            ClickButtonByName("removeRecipeButton");
            Thread.Sleep(500);

            Window errorWindow = WaitForModal("Внимание", 500);
            Assert.IsNotNull(errorWindow, "Сообщение об ошибке не появилось");

            CloseModalByTitle("Внимание");

            Console.WriteLine("=== TC_006 УСПЕШНО ЗАВЕРШЁН ===");
        }

        #endregion

        #region TC_007: Поиск рецепта по названию

        [TestMethod]
        public void TC_007_SearchRecipeByName_ShouldFindAndDisplay()
        {
            Console.WriteLine("=== TC_007: Поиск рецепта по названию ===");

            ClearAllRecipes();

            // Создаём уникальный рецепт с простым названием для поиска
            string uniqueName = "ТестовыйРецепт";
            AddNewRecipeToPlanOnDate(DateTime.Today.ToString("dd.MM.yyyy"), uniqueName, "Описание", "Ингредиенты", "Инструкции", "100");
            Thread.Sleep(500);

            // Поисковый запрос (часть названия)
            string searchQuery = "тест";

            ClickButtonByName("searchRecipeButton");
            Thread.Sleep(500);

            Window searchForm = WaitForModal("Поиск рецепта");
            Assert.IsNotNull(searchForm, "Форма поиска не открылась");

            SetEditByIndex(searchForm, 0, searchQuery);
            Thread.Sleep(500);

            ClickButtonByText(searchForm, "Найти");
            Thread.Sleep(500);

            // Ждём открытия формы результата (может быть список результатов или сразу рецепт)
            Window resultForm = WaitForModal("Результаты поиска", 4000);
            if (resultForm != null)
            {
                // Если открылся список результатов, выбираем первый
                var resultsList = resultForm.FindFirstDescendant(cf => cf.ByControlType(ControlType.List));
                if (resultsList != null)
                {
                    var items = resultsList.FindAllDescendants(cf => cf.ByControlType(ControlType.ListItem));
                    if (items.Length > 0)
                    {
                        items[0].Click();
                        Thread.Sleep(500);

                        var viewButton = resultForm.FindFirstDescendant(cf => cf.ByText("Просмотреть рецепт"))?.AsButton();
                        if (viewButton != null)
                        {
                            viewButton.Click();
                            Thread.Sleep(500);
                        }
                    }
                }
            }
            else
            {
                // Если сразу открылся рецепт
                resultForm = WaitForModal("Рецепт:", 4000);
            }

            Assert.IsNotNull(resultForm, "Форма с результатом поиска не открылась");

            // Проверяем, что открылся правильный рецепт (содержит название)
            StringAssert.Contains(resultForm.Title, uniqueName, $"Открылся не тот рецепт. Ожидалось: {uniqueName}, Фактически: {resultForm.Title}");

            // Закрываем форму
            ClickButtonByText(resultForm, "Закрыть");
            Thread.Sleep(500);

            Console.WriteLine("=== TC_007 УСПЕШНО ЗАВЕРШЁН ===");
        }

        #endregion

        #region TC_008: Ошибка при поиске несуществующего рецепта

        [TestMethod]
        public void TC_008_SearchNonexistentRecipe_ShouldShowError()
        {
            Console.WriteLine("=== TC_008: Ошибка при поиске несуществующего рецепта ===");

            List<string> data = TestData["TC_008"];
            string query = data[0];

            ClickButtonByName("searchRecipeButton");
            Thread.Sleep(500);

            Window searchForm = WaitForModal("Поиск рецепта");
            Assert.IsNotNull(searchForm, "Форма поиска не открылась");

            SetEditByIndex(searchForm, 0, query);
            Thread.Sleep(500);

            ClickButtonByText(searchForm, "Найти");
            Thread.Sleep(500);

            Window errorWindow = WaitForModal("Результат поиска", 2000);
            Assert.IsNotNull(errorWindow, "Сообщение о ненайденном рецепте не появилось");

            CloseModalByTitle("Результат поиска");

            AutomationElement cancelButton = searchForm.FindFirstDescendant(cf => cf.ByText("Отмена"))?.AsButton();
            if (cancelButton != null) cancelButton.Click();
            Thread.Sleep(500);

            Console.WriteLine("=== TC_008 УСПЕШНО ЗАВЕРШЁН ===");
        }

        #endregion

        #region TC_109: Создание списка покупок с группировкой

        [TestMethod]
        public void TC_109_GenerateShoppingListWithGrouping_ShouldSucceed()
        {
            Console.WriteLine("=== TC_109: Создание списка покупок с группировкой ===");

            ClearAllRecipes();

            AddNewRecipeToPlanOnDate("01.05.2025", "Макароны с сыром", "Классическое блюдо", "Макароны, Сыр, Масло", "Сварить макароны, Натереть сыр", "450");
            AddNewRecipeToPlanOnDate("03.05.2025", "Курица с рисом", "Сытный ужин", "Курица, Рис, Морковь, Лук", "Обжарить курицу, Добавить рис", "650");
            AddNewRecipeToPlanOnDate("05.05.2025", "Яблочная шарлотка", "Вкусный десерт", "Яблоки, Мука, Яйца, Сахар", "Взбить, Замесить, Испечь", "350");
            AddNewRecipeToPlanOnDate("07.05.2025", "Запеченные яблоки", "Полезный десерт", "Яблоки, Творог, Мед", "Смешать, Запечь", "200");
            Thread.Sleep(1000);

            ClickButtonByName("generateShoppingListButton");
            Thread.Sleep(1000);

            Window periodForm = WaitForModal("Выбор периода", 2000);
            Assert.IsNotNull(periodForm, "Форма периода не открылась");
            Console.WriteLine("Форма периода открылась");

            Thread.Sleep(500);

            // Устанавливаем начальную дату
            SetDatePickerValueInForm(periodForm, "startDatePicker", "01.05.2025");
            Thread.Sleep(500);

            // Устанавливаем конечную дату
            SetDatePickerValueInForm(periodForm, "endDatePicker", "07.05.2025");
            Thread.Sleep(500);

            // Нажимаем кнопку "Создать"
            AutomationElement createButton = periodForm.FindFirstDescendant(cf => cf.ByText("Создать"))?.AsButton();
            Assert.IsNotNull(createButton, "Кнопка 'Создать' не найдена");
            createButton.Click();
            Console.WriteLine("Кнопка 'Создать' нажата");
            Thread.Sleep(2000);

            // Ждём открытия окна списка покупок
            Window shoppingForm = null;
            for (int i = 0; i < 20; i++)
            {
                shoppingForm = WaitForModal("Список покупок", 500);
                if (shoppingForm != null) break;
                Console.WriteLine($"Ожидание окна списка покупок... попытка {i + 1}");
                Thread.Sleep(500);
            }

            Assert.IsNotNull(shoppingForm, "Окно списка покупок не открылось");
            Console.WriteLine("Окно списка покупок открылось");
            Assert.IsTrue(shoppingForm.Title.Contains("позиций"), "Список покупок пуст");

            ClickButtonByText(shoppingForm, "Закрыть");
            Thread.Sleep(500);

            Console.WriteLine("=== TC_109 УСПЕШНО ЗАВЕРШЁН ===");
        }

        #endregion

        #region TC_110: Экспорт списка покупок в TXT

        [TestMethod]
        public void TC_110_ExportShoppingListToTxt_ShouldSucceed()
        {
            Console.WriteLine("=== TC_110: Экспорт списка покупок в TXT ===");

            ClearAllRecipes();

            AddNewRecipeToPlanOnDate(DateTime.Today.ToString("dd.MM.yyyy"), "Макароны с сыром", "Классическое блюдо", "Макароны, Сыр, Масло", "Сварить макароны, Натереть сыр", "450");
            AddNewRecipeToPlanOnDate(DateTime.Today.AddDays(1).ToString("dd.MM.yyyy"), "Яблочная шарлотка", "Вкусный десерт", "Яблоки, Мука, Яйца, Сахар", "Взбить, Замесить, Испечь", "350");
            Thread.Sleep(500);

            ClickButtonByName("generateShoppingListButton");
            Thread.Sleep(500);

            Window periodForm = WaitForModal("Выбор периода");
            Assert.IsNotNull(periodForm, "Форма периода не открылась");

            SetDatePickerValueInForm(periodForm, "startDatePicker", DateTime.Today.ToString("dd.MM.yyyy"));
            SetDatePickerValueInForm(periodForm, "endDatePicker", DateTime.Today.AddDays(7).ToString("dd.MM.yyyy"));

            ClickButtonByText(periodForm, "Создать");
            Thread.Sleep(1000);

            Window shoppingForm = WaitForModal("Список покупок", 5000);
            Assert.IsNotNull(shoppingForm, "Окно списка покупок не открылось");

            AutomationElement exportButton = shoppingForm.FindFirstDescendant(cf => cf.ByName("exportTxtButton"))?.AsButton();
            if (exportButton == null) exportButton = shoppingForm.FindFirstDescendant(cf => cf.ByText("📄 Экспорт TXT"))?.AsButton();

            Assert.IsNotNull(exportButton, "Кнопка экспорта не найдена");
            exportButton.Click();
            Thread.Sleep(500);

            Window saveDialog = WaitForModal("Сохранить", 1000);
            if (saveDialog != null)
            {
                AutomationElement cancelButton = saveDialog.FindFirstDescendant(cf => cf.ByText("Отмена"))?.AsButton();
                if (cancelButton != null) cancelButton.Click();
                Thread.Sleep(500);
            }

            ClickButtonByText(shoppingForm, "Закрыть");
            Thread.Sleep(500);

            Console.WriteLine("=== TC_110 УСПЕШНО ЗАВЕРШЁН ===");
        }

        #endregion

        #region TC_111: Редактирование списка покупок

        [TestMethod]
        public void TC_111_EditShoppingList_ShouldSucceed()
        {
            Console.WriteLine("=== TC_111: Редактирование списка покупок ===");

            ClearAllRecipes();

            AddNewRecipeToPlanOnDate(DateTime.Today.ToString("dd.MM.yyyy"), "Яблочная шарлотка", "Вкусный десерт", "Яблоки, Мука, Яйца, Сахар", "Взбить, Замесить, Испечь", "350");
            Thread.Sleep(500);

            ClickButtonByName("generateShoppingListButton");
            Thread.Sleep(1000);

            Window periodForm = WaitForModal("Выбор периода", 3000);
            Assert.IsNotNull(periodForm, "Форма периода не открылась");

            SetDatePickerValueInForm(periodForm, "startDatePicker", DateTime.Today.ToString("dd.MM.yyyy"));
            SetDatePickerValueInForm(periodForm, "endDatePicker", DateTime.Today.ToString("dd.MM.yyyy"));

            ClickButtonByText(periodForm, "Создать");
            Thread.Sleep(2000);

            Window shoppingForm = WaitForModal("Список покупок", 12000);
            Assert.IsNotNull(shoppingForm, "Окно списка покупок не открылось");
            Console.WriteLine("Окно списка покупок открылось");

            // ---- Добавление ингредиента ----
            AutomationElement addButton = shoppingForm.FindFirstDescendant(cf => cf.ByName("addItemButton"))?.AsButton();
            if (addButton == null) addButton = shoppingForm.FindFirstDescendant(cf => cf.ByText("➕ Добавить"))?.AsButton();
            Assert.IsNotNull(addButton, "Кнопка 'Добавить' не найдена");
            addButton.Click();
            Thread.Sleep(500);

            Window addIngredientForm = WaitForModal("Добавление ингредиента", 2000);
            Assert.IsNotNull(addIngredientForm, "Окно добавления ингредиента не открылось");

            // Вводим название ингредиента
            var nameTextBox = addIngredientForm.FindFirstDescendant(cf => cf.ByName("nameTextBox"))?.AsTextBox();
            if (nameTextBox == null)
            {
                var allEdits = addIngredientForm.FindAllDescendants(cf => cf.ByControlType(ControlType.Edit));
                if (allEdits.Length > 0) nameTextBox = allEdits[0].AsTextBox();
            }
            Assert.IsNotNull(nameTextBox, "Поле 'Название ингредиента' не найдено");
            nameTextBox.Text = "Тестовый ингредиент";
            Thread.Sleep(500);

            // Вводим количество
            AutomationElement quantityNumeric = addIngredientForm.FindFirstDescendant(cf => cf.ByControlType(ControlType.Spinner));
            if (quantityNumeric != null)
            {
                var editField = quantityNumeric.FindFirstDescendant(cf => cf.ByControlType(ControlType.Edit));
                if (editField != null)
                {
                    editField.AsTextBox().Text = "3";
                }
                else
                {
                    quantityNumeric.AsTextBox().Text = "3";
                }
            }
            else
            {
                // Альтернативный поиск поля количества
                var allEdits = addIngredientForm.FindAllDescendants(cf => cf.ByControlType(ControlType.Edit));
                if (allEdits.Length > 1)
                {
                    allEdits[1].AsTextBox().Text = "3";
                }
            }
            Thread.Sleep(500);

            // Нажимаем OK
            AutomationElement okButton = addIngredientForm.FindFirstDescendant(cf => cf.ByName("okButton"))?.AsButton();
            if (okButton == null) okButton = addIngredientForm.FindFirstDescendant(cf => cf.ByText("OK"))?.AsButton();
            Assert.IsNotNull(okButton, "Кнопка 'OK' не найдена");
            okButton.Click();
            Thread.Sleep(500);
            CloseModalByTitle("Успех");
            Thread.Sleep(500);

            // Проверяем, что ингредиент появился в списке
            AutomationElement shoppingListBox = shoppingForm.FindFirstDescendant(cf => cf.ByName("shoppingListBox"));
            if (shoppingListBox == null)
            {
                shoppingListBox = shoppingForm.FindFirstDescendant(cf => cf.ByControlType(ControlType.List));
            }
            Assert.IsNotNull(shoppingListBox, "Список покупок не найден");

            // ---- Удаление ингредиента ----
            // Находим и выбираем добавленный ингредиент
            var items = shoppingListBox.FindAllDescendants(cf => cf.ByControlType(ControlType.ListItem));
            AutomationElement targetItem = null;
            foreach (AutomationElement item in items)
            {
                AutomationElement text = item.FindFirstDescendant(cf => cf.ByControlType(ControlType.Text));
                if (text != null && text.Name != null && text.Name.Contains("Тестовый ингредиент"))
                {
                    targetItem = item;
                    break;
                }
            }

            if (targetItem != null)
            {
                targetItem.Click();
                Thread.Sleep(500);

                AutomationElement removeButton = shoppingForm.FindFirstDescendant(cf => cf.ByName("removeItemButton"))?.AsButton();
                if (removeButton == null) removeButton = shoppingForm.FindFirstDescendant(cf => cf.ByText("✖ Удалить"))?.AsButton();
                if (removeButton != null)
                {
                    removeButton.Click();
                    Thread.Sleep(500);

                    Window confirmDialog = WaitForModal("Подтверждение", 3000);
                    if (confirmDialog != null)
                    {
                        AutomationElement yesButton = confirmDialog.FindFirstDescendant(cf => cf.ByText("Да"))?.AsButton();
                        if (yesButton != null) yesButton.Click();
                        Thread.Sleep(500);
                    }
                }
            }

            // ---- Редактирование ингредиента ----
            // Находим и выбираем ингредиент "Яблоки"
            AutomationElement appleItem = null;
            foreach (AutomationElement item in items)
            {
                AutomationElement text = item.FindFirstDescendant(cf => cf.ByControlType(ControlType.Text));
                if (text != null && text.Name != null && text.Name.Contains("Яблоки"))
                {
                    appleItem = item;
                    break;
                }
            }

            if (appleItem != null)
            {
                appleItem.Click();
                Thread.Sleep(500);

                AutomationElement editButton = shoppingForm.FindFirstDescendant(cf => cf.ByName("editItemButton"))?.AsButton();
                if (editButton == null) editButton = shoppingForm.FindFirstDescendant(cf => cf.ByText("✎ Редактировать"))?.AsButton();
                if (editButton != null)
                {
                    editButton.Click();
                    Thread.Sleep(500);

                    Window editForm = WaitForModal("Редактирование ингредиента", 1000);
                    if (editForm != null)
                    {
                        // Меняем количество
                        AutomationElement quantityEdit = editForm.FindFirstDescendant(cf => cf.ByControlType(ControlType.Spinner));
                        if (quantityEdit != null)
                        {
                            AutomationElement editField = quantityEdit.FindFirstDescendant(cf => cf.ByControlType(ControlType.Edit));
                            if (editField != null)
                            {
                                editField.AsTextBox().Text = "5";
                            }
                        }
                        else
                        {
                            var allEdits = editForm.FindAllDescendants(cf => cf.ByControlType(ControlType.Edit));
                            if (allEdits.Length > 0)
                            {
                                allEdits[0].AsTextBox().Text = "5";
                            }
                        }
                        Thread.Sleep(500);

                        AutomationElement editOkButton = editForm.FindFirstDescendant(cf => cf.ByName("okButton"))?.AsButton();
                        if (editOkButton == null) editOkButton = editForm.FindFirstDescendant(cf => cf.ByText("OK"))?.AsButton();
                        if (editOkButton != null)
                        {
                            editOkButton.Click();
                            Thread.Sleep(500);
                            CloseModalByTitle("Успех");
                        }
                    }
                }
            }

            // Закрываем окно списка покупок
            AutomationElement closeButton = shoppingForm.FindFirstDescendant(cf => cf.ByName("closeButton"))?.AsButton();
            if (closeButton == null) closeButton = shoppingForm.FindFirstDescendant(cf => cf.ByText("Закрыть"))?.AsButton();
            if (closeButton != null) closeButton.Click();
            Thread.Sleep(500);

            Console.WriteLine("=== TC_111 УСПЕШНО ЗАВЕРШЁН ===");
        }

        #endregion
    }
}