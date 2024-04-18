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

        public List<WorkshopSeriesWorkshopDTO> seriesHasSurvey { get; set; } = null!;
        
        public async Task OnGetAsync()
        {
            seriesHasSurvey = await _repo.seriesSurvey();
        }
    }
}
