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

        [BindProperty(SupportsGet = true)]
        public int wssId { get; set; }

        [BindProperty(SupportsGet = true)]
        public int wsId { get; set; }

        [BindProperty(SupportsGet =true)]
        public int surveyId { get; set; }

        [BindProperty]
        public string key { get; set; }
        
        public void OnGet()
        {
            _survey = _repo.getListSurvey(wssId, wsId);
        }

        public IActionResult OnPost()
        {
            return Redirect($"~/Surveys/series-{wssId}/workshop-{wsId}/survey-{surveyId}/{key}");
        }
    }
}
