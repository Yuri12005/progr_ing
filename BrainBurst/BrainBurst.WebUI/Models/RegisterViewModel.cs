using System.ComponentModel.DataAnnotations;

namespace BrainBurst.WebUI.Models;

public class RegisterViewModel
{
    [Required(ErrorMessage = "Введіть Email")]
    [EmailAddress(ErrorMessage = "Некоректний формат Email")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Введіть повне ім'я")]
    public string FullName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Введіть пароль")]
    [DataType(DataType.Password)]
    [StringLength(100, ErrorMessage = "Пароль має містити мінімум {2} символів.", MinimumLength = 6)]
    public string Password { get; set; } = string.Empty;

    [DataType(DataType.Password)]
    [Display(Name = "Підтвердіть пароль")]
    [Compare("Password", ErrorMessage = "Паролі не співпадають")]
    public string ConfirmPassword { get; set; } = string.Empty;
}