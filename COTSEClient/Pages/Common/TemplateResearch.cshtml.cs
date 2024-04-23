using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace COTSEClient.Pages.Common
{
    public class TemplateResearchModel : PageModel
    {
        public IActionResult OnGet()
        {
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Template/Template.xlsx");

            if (!System.IO.File.Exists(filePath))
            {
                return NotFound();
            }
            return PhysicalFile(filePath, "application/xlsx", "Template.xlsx");
        }
    }
}
