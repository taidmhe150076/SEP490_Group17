using BusinessLogic.IRepository;
using COTSEClient.Helper;
using DataAccess.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace COTSEClient.Pages.Department
{
    public class AllSeriesWorkshopModel : PageModel
    {

        private readonly IRepositoryWorkshopSeries _repositoryWorkshopSeries;

        [BindProperty(SupportsGet = true)]
        public string SearchInput { get; set; }
        [BindProperty(SupportsGet = true)]
        public DateTime? CurentDate { get; set; }

        [BindProperty]
        public  IFormFile imageFile { get;set; }

        [BindProperty]
        public WorkshopSeries WorkshopSeries { get; set; }

        [BindProperty]
        public int WorskhopSeriesId { get; set; }
        [BindProperty]
        public string Msg { get; set; }

        public PageList<WorkshopSeries> WorkshopSeriesPage { get; set; }
     
        public AllSeriesWorkshopModel(IRepositoryWorkshopSeries repositoryWorkshopSeries)
        {
            _repositoryWorkshopSeries = repositoryWorkshopSeries;
        }

        public void OnGet( int pageIndex = 1, int pageSize = 9)
        {
            Msg = TempData["Msg"] as string;

            var source = _repositoryWorkshopSeries.GetAllWorkshopSeries().AsQueryable();

             WorkshopSeriesPage = PageList<WorkshopSeries>.Create(source, pageIndex, pageSize);


        }

        public void OnPost( string searchInput, int pageIndex = 1, int pageSize = 9)
        {

            var source = _repositoryWorkshopSeries.GetAllWorkshopSeries().AsQueryable();

            if (!string.IsNullOrEmpty(searchInput))
            {
                source = source.Where(s => s.WorkshopSeriesName.Contains(searchInput, StringComparison.OrdinalIgnoreCase));
            }

            WorkshopSeriesPage = PageList<WorkshopSeries>.Create(source, pageIndex, pageSize);

        }

        public IActionResult OnPostCreateWorkshopSeries(WorkshopSeries workshopSeries )
        {
            try
            {
                if (imageFile != null && imageFile.Length > 0)
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        imageFile.CopyTo(memoryStream);
                 
                        workshopSeries.Image = Convert.ToBase64String(memoryStream.ToArray());
                    }
                }

                WorkshopSeries createdWorkshopSeries = _repositoryWorkshopSeries.CreateWorkshopSeries(workshopSeries);
                TempData["Msg"] = "Update Success!";
                return RedirectToPage("AddNewSeries", new { seriesWorkshopId = createdWorkshopSeries.Id });

            }
            catch (Exception ex)
            {
                
            }
            return Page();

        }

        public IActionResult OnPostUpdateWorkshopSeries()
        {
            try
            {
               var  workshopSeries = _repositoryWorkshopSeries.GetWorkshopSeriesById(WorkshopSeries.Id);

                if (workshopSeries == null)
                {
                    TempData["Msg"] = "User Not Exists!";
                    return RedirectToPage();
                }

                workshopSeries.WorkshopSeriesName = WorkshopSeries.WorkshopSeriesName;
                workshopSeries.Description = WorkshopSeries.Description;
                workshopSeries.StartDate = WorkshopSeries.StartDate;
                workshopSeries.EndDate = WorkshopSeries.EndDate;
                if (imageFile != null && imageFile.Length > 0)
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        imageFile.CopyTo(memoryStream);

                        workshopSeries.Image = Convert.ToBase64String(memoryStream.ToArray());
                    }
                }
                _repositoryWorkshopSeries.UpdateWorkshopSeries(workshopSeries);
                TempData["Msg"] = "Update Success!";
            }
            catch (Exception ex)
            {
                throw;
            }
            return RedirectToPage();
        }
    }
}
