using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using BrainBurst.Domain.Entities;
using System.Threading.Tasks;

namespace BrainBurst.WebUI.Controllers
{
    [Authorize] // Налаштування доступні тільки залогіненим
    public class SettingsController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public SettingsController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        // Головна сторінка налаштувань
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            ViewBag.Username = string.IsNullOrEmpty(user?.FullName) ? user?.UserName : user?.FullName;
            return View();
        }

        // Сторінка зміни пароля
        [HttpGet]
        public IActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(string oldPassword, string newPassword, string confirmPassword)
        {
            if (newPassword != confirmPassword)
            {
                ModelState.AddModelError("", "Паролі не збігаються.");
                return View();
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");

            // Використовуємо вбудований метод Identity для зміни пароля
            var result = await _userManager.ChangePasswordAsync(user, oldPassword, newPassword);

            if (result.Succeeded)
            {
                // Оновлюємо кукі, щоб сесія не перервалася після зміни пароля
                await _signInManager.RefreshSignInAsync(user);
                return RedirectToAction("Index", new { message = "Пароль успішно змінено" });
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }

            return View();
        }

        // Видалення акаунта
        [HttpGet]
        public IActionResult DeleteAccount()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> DeleteAccount(string password)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");

            // Перевіряємо, чи пароль вірний перед видаленням
            var passwordValid = await _userManager.CheckPasswordAsync(user, password);
            if (!passwordValid)
            {
                ModelState.AddModelError("", "Неправильний пароль. Видалення неможливе.");
                return View();
            }

            // Видаляємо користувача (завдяки Cascade Delete в БД, його картки та тести видаляться автоматично)
            var result = await _userManager.DeleteAsync(user);
            if (result.Succeeded)
            {
                await _signInManager.SignOutAsync();
                return RedirectToAction("Register", "Account");
            }

            ModelState.AddModelError("", "Помилка при видаленні акаунта.");
            return View();
        }
    }
}