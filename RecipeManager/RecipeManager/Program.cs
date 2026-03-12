using System;
using System.Windows.Forms;

namespace RecipeMealPlanner
{
    internal static class Program
    {
        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Создаем и запускаем главную форму
            MealPlanForm mainForm = new MealPlanForm();
            Application.Run(mainForm);
        }
    }
}