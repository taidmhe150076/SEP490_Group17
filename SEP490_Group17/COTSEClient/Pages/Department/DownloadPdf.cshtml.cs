using iTextSharp.text.pdf;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace COTSEClient.Pages.Department
{
    public class DownloadPdfModel : PageModel
    {

        public string FilePath { get; set; }

        public IActionResult OnGet(string filePath)
        {
            try
            {
                PdfReader reader = new PdfReader(filePath);
                FilePath = filePath;

                if (!System.IO.File.Exists(filePath))
                {
                    return NotFound(); // Trả về 404 Not Found nếu không tìm thấy tệp
                }

                // Đọc tệp và trả về dưới dạng phản hồi
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
