using BusinessLogic.IRepository;
using DataAccess.Models;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;


namespace COTSEClient.Pages.Department
{
    public class ListAllImageModel : PageModel
    {
        private readonly IConfiguration _configuration;
       
        private readonly IRepositorySlideWorkshop _repositorySlideWorkshop;
        private readonly IRepositoryWorkshops _repositoryWorkshops;

        private readonly IWebHostEnvironment _webHostEnvironment;

        [BindProperty]
        public List<string> ImageUrls { get; set; }
        [BindProperty]
        public int WorkShopId { get; set; }
        [BindProperty]
        public string WorkshopName { get; set; }
        [BindProperty]
        public bool Flag { get; set; } = false;
        [BindProperty]
        public Workshop WorkShop { get; set; } = new Workshop();
        [BindProperty]
        public string Url { get; set; }


        public ListAllImageModel(IRepositorySlideWorkshop repositorySlideWorkshop , IWebHostEnvironment webHostEnvironment , IRepositoryWorkshops repositoryWorkshops, IConfiguration configuration)
        {
            _repositorySlideWorkshop = repositorySlideWorkshop;
            _webHostEnvironment = webHostEnvironment;
            _repositoryWorkshops = repositoryWorkshops;
            _configuration = configuration;
            Url = _configuration["BaseURL"];
        }

        public IActionResult OnGet(int workShopId)
        {

            if (workShopId == 0)
            {
                return BadRequest();
            }
            WorkShopId = workShopId;
            var slideWorkshops = _repositorySlideWorkshop.GetAllSlideWorkshop(WorkShopId);
            WorkShop = _repositoryWorkshops.GetWorkshopByWorkshopId(WorkShopId);
            WorkshopName = WorkShop.WorkshopName;
            ImageUrls = new List<string>();


            foreach (var workshop in slideWorkshops)
            {

                if (workshop.Image != null)
                {
                    ImageUrls.Add(workshop.Image.Image1);
                }

            }
            return Page();
        }
        public IActionResult OnPostSaveToPdf()
        {
            var slideWorkshops = _repositorySlideWorkshop.GetAllSlideWorkshop(WorkShopId);
            WorkShop = _repositoryWorkshops.GetWorkshopByWorkshopId(WorkShopId);
         
            ImageUrls = new List<string>();
            var workshopName = Helper.HelperMethods.RemoveSpecialCharacters(WorkShop.WorkshopName);
            foreach (var workshop in slideWorkshops)
            {
                if (workshop.Image != null)
                {
                    ImageUrls.Add(workshop.Image.Image1);
                }
            }

            string fileName = Path.Combine(_webHostEnvironment.WebRootPath, $@"PDF\{workshopName}.pdf");


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
                         pdfImage.ScaleToFit(doc.PageSize.Width ,doc.PageSize.Height);
                        doc.Add(pdfImage);
                    }
                    doc.Close();
                }
            }
            Flag = true;
            Url += $@"PDF/{workshopName}.pdf";
            return Page();
        }
    }
}
