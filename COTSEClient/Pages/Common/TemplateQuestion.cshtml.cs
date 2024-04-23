using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace COTSEClient.Pages.Common
{
    public class TemplateQuestionModel : PageModel
    {
        public IActionResult OnGet()
        {
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Template/TemplateQuestion.xlsx");

            if (!System.IO.File.Exists(filePath))
            {
                return NotFound();
            }
            return PhysicalFile(filePath, "application/xlsx", "TemplateQuestion.xlsx");
        }
    }
}
