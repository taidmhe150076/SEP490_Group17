using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace COTSEClient.Pages.Common
{
   //[Authorize(Roles = "User")]
   public class HomeModel : PageModel
    {

        public void OnGet()
        {
        }
    }
}
