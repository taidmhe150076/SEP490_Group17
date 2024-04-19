using BusinessLogic.IRepository;
using DataAccess.DTO;
using DataAccess.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace COTSEClient.Pages.Survey
{
    public class SurveyListModel : PageModel
    {

        private readonly IRepositorySurvey _repositorySurvey;
        public SurveyListModel(IRepositorySurvey repositorySurvey)
        {
            _repositorySurvey = repositorySurvey;
        }

        [BindProperty]
        public List<WorkshopSurveyDTO> list_data { get; set; } = null!;

        [BindProperty]
        public int wssId { get; set; }

        [BindProperty]
        public int wsId { get; set; }

        [BindProperty]
        public int surveyId { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            //list_data = await _repositorySurvey.surveyList();
            return Page();
        }


        public async Task<IActionResult> OnGetListAsync()
        {
            return new JsonResult(await _repositorySurvey.surveyList());
        }

        
        public async Task<IActionResult> OnPostDeleteSurveyAsync()
        {
            var state = await _repositorySurvey.deleteSurvey(wssId, wsId, surveyId);
            var result = new { result = state };
            return new JsonResult(result);
        }
    }
}
