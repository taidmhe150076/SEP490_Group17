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

     
        public PageList<WorkshopSeries> WorkshopSeriesPage { get; set; }
     
        public AllSeriesWorkshopModel(IRepositoryWorkshopSeries repositoryWorkshopSeries)
        {
            _repositoryWorkshopSeries = repositoryWorkshopSeries;
        }

        public void OnGet(DateTime curentDate , string searchInput, int pageIndex = 1, int pageSize = 3)
        {

            var source = _repositoryWorkshopSeries.GetAllWorkshopSeries().AsQueryable();

            if (!string.IsNullOrEmpty(searchInput))
            {
                source = source.Where(s => s.WorkshopSeriesName.Contains(searchInput, StringComparison.OrdinalIgnoreCase));
            }

            if (curentDate != DateTime.MinValue)
            {
                source = source.Where(s => s.StartDate <= curentDate && s.EndDate >= curentDate);
            }
         

            WorkshopSeriesPage = PageList<WorkshopSeries>.Create(source, pageIndex, pageSize);


        }

        public void OnPostAsync()
        {
           

          
        }
    }
}
