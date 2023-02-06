using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.SqlTypes;

namespace RazorPages.Identity.Classes
{
    public class User : IdentityUser
    {
        [Required]
        public string DisplayName { get; set; }

        public int CoinBalance { get; set; }
    }
}
