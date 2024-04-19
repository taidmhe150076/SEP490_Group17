using BusinessLogic.IRepository;
using DataAccess.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace COTSEClient.Pages.Report
{
    public class HomeChartsWorkshopModel : PageModel
    {
        private readonly IRepositorySurvey _repositorySurvey;
        public HomeChartsWorkshopModel(IRepositorySurvey repositorySurvey)
        {
            _repositorySurvey = repositorySurvey;
        }

        [BindProperty]
        public int? WorkshopId { get; set; }
        [BindProperty]
        public int? WSSeriesId { get; set; }
        [BindProperty]
        public int? SurveyDataId { get; set; }
        public IActionResult OnGet(int? workShopId, int? wsSeriesId)
        {
            WorkshopId = workShopId;
            WSSeriesId = wsSeriesId;
            SurveyDataId = _repositorySurvey.GetWorkshopSurveyUrlIdOfParticipants((int)workShopId);
            return Page();
        }
    }
}
