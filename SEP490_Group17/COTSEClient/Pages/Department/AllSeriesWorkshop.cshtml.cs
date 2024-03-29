using BusinessLogic.IRepository;
using DataAccess.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace COTSEClient.Pages.Department
{
    public class AllSeriesWorkshopModel : PageModel
    {

        private readonly IRepositoryWorkshopSeries _repositoryWorkshopSeries;

        public List<WorkshopSeries> WorkshopSeriesList { get; set; }


        public AllSeriesWorkshopModel(IRepositoryWorkshopSeries repositoryWorkshopSeries)
        {
            _repositoryWorkshopSeries = repositoryWorkshopSeries;
        }

        public void OnGet()
        {
            WorkshopSeriesList = _repositoryWorkshopSeries.GetAllWorkshopSeries();
        }
    }
}
