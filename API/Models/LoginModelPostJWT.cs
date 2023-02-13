using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace API.Models;

#nullable disable
/// <summary>
/// Модель, в которую биндятся данные с формы логина пользователя
/// </summary>
public class LoginModelPostJWT
{

    [EmailAddress]
    [Required]
    [DisplayName("Email")]
    public string Email { get; set; }

    [DataType(DataType.Password)]
    [Required]
    [DisplayName("Password")]
    public string Password { get; set; }
}