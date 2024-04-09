using BusinessLogic.IRepository;
using DataAccess.Models;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Drawing;
using System.Drawing.Imaging;


namespace COTSEClient.Pages.Department
{
    public class ListAllImageModel : PageModel
    {

        private readonly IRepositorySlideWorkshop _repositorySlideWorkshop;
        private readonly IWebHostEnvironment _webHostEnvironment;

        [BindProperty]
        public List<string> ImageUrls { get; set; }



        public ListAllImageModel(IRepositorySlideWorkshop repositorySlideWorkshop , IWebHostEnvironment webHostEnvironment )
        {
            _repositorySlideWorkshop = repositorySlideWorkshop;
            _webHostEnvironment = webHostEnvironment;
      
        }

        public void OnGet(int workShopId = 1214)
        {

            var slideWorkshops = _repositorySlideWorkshop.GetAllSlideWorkshop(workShopId);

            ImageUrls = new List<string>();


            foreach (var workshop in slideWorkshops)
            {

                if (workshop.Image != null)
                {
                    ImageUrls.Add(workshop.Image.Image1);
                }

            }
        }
        public IActionResult OnPostSaveToPdf(int workShopId = 1214)
        {
            var slideWorkshops = _repositorySlideWorkshop.GetAllSlideWorkshop(workShopId);
            ImageUrls = new List<string>();

            foreach (var workshop in slideWorkshops)
            {
                if (workshop.Image != null)
                {
                    ImageUrls.Add(workshop.Image.Image1);
                }
            }

            string fileName = Path.Combine(_webHostEnvironment.WebRootPath, "test.pdf");

            using (FileStream fileStream = new FileStream(fileName, FileMode.Create))
            {
                using (Document doc = new Document())
                {
                    PdfWriter.GetInstance(doc, fileStream);
                    doc.Open();

                    foreach (var imageUrl in ImageUrls)
                    {
                        byte[] imageBytes = Convert.FromBase64String(imageUrl);
                        iTextSharp.text.Image pdfImage = iTextSharp.text.Image.GetInstance(imageBytes);
                        doc.Add(pdfImage);
                    }
                    doc.Close();
                }
            }

            return RedirectToPage("./DownloadPdf", new { filePath = fileName });

        }
    }
}
