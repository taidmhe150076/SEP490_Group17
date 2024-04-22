using BusinessLogic.IRepository;
using DataAccess.Constants;
using DataAccess.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace COTSEClient.Pages.Survey
{
    [Authorize(Roles = COTSEConstants.ROLE_RESEARCHER + "," + COTSEConstants.ROLE_ORGANIZER)]
    public class SurveyDetailsModel : PageModel
    {

        private readonly IRepositorySurvey _repo;

        public SurveyDetailsModel(IRepositorySurvey repo)
        {
            _repo = repo;
        }
        // Define properties for binding
        [BindProperty(SupportsGet = true)]
        public int wssId { get; set; }
        [BindProperty(SupportsGet = true)]
        public int wsId { get; set; }
        [BindProperty(SupportsGet = true)]
        public int surveyId { get; set; }


        [BindProperty]
        public SurveyDTO surveyInfo { get; set; } = new SurveyDTO();
        [BindProperty]
        public List<FeedbackResult> feedbackResults { get; set; }

        [BindProperty]
        public Dictionary<string, int> feedbackCount { get; set; }

        [BindProperty]
        public List<CommonQA> dataList { get; set; } = null!;

        [BindProperty]
        public bool state_display { get; set; } = false;

        public async Task<IActionResult> OnGetAsync()
        {
            try
            {
                surveyInfo = await _repo.getSurey(surveyId);
                feedbackResults = await _repo.getSurveySentimentResult(surveyId);
                dataList = await _repo.getOtherData(surveyId);
                feedbackCount = await _repo.CountFeedback(feedbackResults);
                state_display = true;
                return Page();
            }
            catch (Exception e)
            {
                ModelState.AddModelError("File", e.Message);
                return Page();
            }

        }

    }
}
