using iTextSharp.text.pdf;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace COTSEClient.Pages.Department
{
    public class DownloadPdfModel : PageModel
    {

        public string FilePath { get; set; }

        public IActionResult OnGet(string filePath , int workshopId)
        {
            try
            {
                PdfReader reader = new PdfReader(filePath);
                FilePath = filePath;

                if (!System.IO.File.Exists(filePath))
                {
                    return NotFound(); 
                }
                var stream = new FileStream(filePath, FileMode.Open);
                
                return File(stream, "application/pdf", "test.pdf");
            }
            catch (IOException ex)
            {
                Console.WriteLine("Error reading PDF file: " + ex.Message);
                
            }

            return Page();
        }
    }
}
