using System.ComponentModel.DataAnnotations;

namespace BrainBurst.WebUI.Models;

public class ForgotPasswordViewModel
{
    [Required(ErrorMessage = "Введіть Email")]
    [EmailAddress(ErrorMessage = "Некоректний формат Email")]
    public string Email { get; set; }
}