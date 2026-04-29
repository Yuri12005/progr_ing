using Microsoft.AspNetCore.Mvc;

namespace BrainBurst.WebUI.Controllers
{
    public class SettingsController : Controller
    {
        // Відкриває головну сторінку налаштувань
        public IActionResult Index()
        {
            ViewBag.Username = "Твій_Нікнейм";
            return View();
        }

        // НОВЕ: Відкриває сторінку зміни пароля
        [HttpGet]
        public IActionResult ChangePassword()
        {
            return View();
        }

        // НОВЕ: Приймає дані після натискання "Зберегти"
        [HttpPost]
        public IActionResult ChangePassword(string oldPassword, string newPassword, string confirmPassword)
        {
            // ПІЗНІШЕ: тут буде логіка перевірки старого пароля 
            // та збереження нового в базу даних

            // Повертаємо користувача назад в налаштування
            return RedirectToAction("Index");
        }

        // НОВЕ: Відкриває сторінку попередження про видалення
        [HttpGet]
        public IActionResult DeleteAccount()
        {
            return View();
        }

        // НОВЕ: Приймає запит на видалення після вводу пароля
        [HttpPost]
        public IActionResult DeleteAccount(string password)
        {
            // ПІЗНІШЕ ТУТ БУДЕ ЛОГІКА:
            // 1. Перевіряємо, чи введений пароль правильний.
            // 2. Якщо так — видаляємо користувача та всі його картки/тести з БД.

            // Після успішного видалення перекидаємо на сторінку реєстрації/входу
            return RedirectToAction("Register", "Account");
        }
    }
}