using Microsoft.AspNetCore.Mvc;

namespace BrainBurst.WebUI.Controllers
{
    public class AccountController : Controller
    {
        // Відповідає за сторінку реєстрації
        public IActionResult Register()
        {
            return View();
        }

        // ВІДПОВІДАЄ ЗА СТОРІНКУ ВХОДУ (нове)
        public IActionResult Login()
        {
            return View();
        }

        public IActionResult Logout()
        {
            // ПІЗНІШЕ ТУТ БУДЕ ЛОГІКА:
            // Наприклад, щось типу await HttpContext.SignOutAsync();
            // щоб система забула користувача.

            // Після "виходу" перенаправляємо людину на сторінку Входу
            return RedirectToAction("Login");
        }
    }
}