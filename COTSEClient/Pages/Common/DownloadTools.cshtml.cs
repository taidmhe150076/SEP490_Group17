using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace COTSEClient.Pages.Common
{
    public class DownloadToolsModel : PageModel
    {
        public IActionResult OnGet()
        {
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Tools/SEP490_Group17.zip");

            if (!System.IO.File.Exists(filePath))
            {
                return NotFound();
            }
            return PhysicalFile(filePath, "application/zip", "SEP490_Group17.zip");
        }
    }
}
