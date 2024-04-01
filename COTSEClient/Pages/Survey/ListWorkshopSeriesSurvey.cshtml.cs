using BusinessLogic.IRepository;
using DataAccess.DTO;
using DataAccess.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace COTSEClient.Pages.Survey
{
    public class ListWorkshopSeriesSurveyModel : PageModel
    {

        private readonly IRepositorySurvey _repo;
        public ListWorkshopSeriesSurveyModel(IRepositorySurvey repo)
        {
            _repo = repo;
        }

        public List<WorkshopSeriesWorkshop> seriesHasSurvey { get; set; } = null!;
        
        public void OnGet()
        {
            seriesHasSurvey = _repo.seriesSurvey();
        }
    }
}
