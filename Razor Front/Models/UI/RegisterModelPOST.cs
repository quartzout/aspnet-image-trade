﻿using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace RazorPages.Models.Classes.UI;

/// <summary>
/// Модель, в которую биндятся данные с формы регистрации пользователя
/// </summary>
#nullable disable
public class RegisterModelPOST
{
    [DisplayName("Email")]
    [Required]
    [EmailAddress]
    public string Email { get; set; }

    [DisplayName("Name")]
    [Required]
    [StringLength(maximumLength: 20, ErrorMessage = "Name should be less than 20 characters long")]
    public string DisplayName { get; set; }

    [DisplayName("Password")]
    [Required]
    [DataType(DataType.Password)]
    [StringLength(maximumLength: 100, MinimumLength = 8, ErrorMessage="Password should take at least 8 characters")]
    public string Password { get; set; }

    [DisplayName("Confirm Password")]
    [Required]
    [DataType(DataType.Password)]
    [Compare(nameof(Password))]
    public string ConfirmPassword { get; set; }

    public bool RememberMe { get; set; }
}