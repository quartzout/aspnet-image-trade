using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using System.ComponentModel.DataAnnotations;
using System.Web;

namespace RazorPages.Pages.Identity
{
    public class EmailConfirmationLinkModel : PageModel
    {
        [DataType(DataType.Url)]
        public string DisplayLink { get; set; }

        public void OnGet(string encodedLink)
        {
            DisplayLink = HttpUtility.UrlDecode(encodedLink);
        }

    }
}
