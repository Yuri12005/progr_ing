using System.ComponentModel.DataAnnotations;

namespace BrainBurst.WebUI.Models;

public class ResetPasswordViewModel
{
    [Required]
    public string Email { get; set; }

    [Required]
    public string Code { get; set; }

    [Required(ErrorMessage = "Введіть новий пароль")]
    [DataType(DataType.Password)]
    public string Password { get; set; }

    [DataType(DataType.Password)]
    [Compare("Password", ErrorMessage = "Паролі не співпадають")]
    public string ConfirmPassword { get; set; }
}