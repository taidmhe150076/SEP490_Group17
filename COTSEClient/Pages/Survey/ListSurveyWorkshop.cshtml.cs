using BusinessLogic.IRepository;
using DataAccess.DTO;
using DataAccess.Models;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace COTSEClient.Pages.Survey
{
    public class ListSurveyWorkshopModel : PageModel
    {
        private readonly IRepositorySurvey _repo;

        public ListSurveyWorkshopModel(IRepositorySurvey repo)
        {
            _repo = repo;
        }

        [BindProperty]
        public string WorkshopName { get; set; } = null!;

        [BindProperty]
        public List<SurveyDTO> _survey { get; set; } = null!;

        public void OnGet(int wssId, int wsId)
        {
            _survey = _repo.getListSurvey(wssId, wsId);
        }

        public IActionResult OnPost(int surveyId, int wssId, int wsId) {

            return Redirect($"~/Surveys/series-{wssId}/workshop-{wsId}/survey-{surveyId}");
        }
    }
}
