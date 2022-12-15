using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace RazorPages.Models.Classes.UI;

#nullable disable
public class LoginModelPOST
{

    [EmailAddress]
    [Required]
    [DisplayName("Email")]
    public string Email { get; set; }

    [DataType(DataType.Password)]
    [Required]
    [DisplayName("Password")]
    public string Password { get; set; }

    public bool RememberMe { get; set; } = true;
}