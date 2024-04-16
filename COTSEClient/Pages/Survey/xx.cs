using BusinessLogic.IRepository;
using DataAccess.Common;
using DataAccess.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace COTSEClient.Pages.Survey
{
    public class SurveyDetailModel : PageModel
    {
        private readonly IRepositorySurvey _repo;

        public SurveyDetailModel(IRepositorySurvey repo)
        {
            _repo = repo;
        }

        // Define properties for binding
        [BindProperty(SupportsGet = true)]
        public int wssId { get; set; }
        [BindProperty(SupportsGet = true)]
        public int wsId { get; set; }
        [BindProperty(SupportsGet = true)]
        public int survey_id { get; set; }



        [BindProperty]
        public SurveyDTO surveyInfo { get; set; } = null!;
        [BindProperty]
        public List<FeedbackResult> feedbackResults { get; set; } = null!;

        [BindProperty]
        public Dictionary<string, int> feedbackCount { get; set; } = null!;

        [BindProperty]
        public List<CommonQA> dataList { get; set; } = null!;

        [BindProperty]
        public bool state_display { get; set; } = false;

        public async Task<IActionResult> OnGetAsync()
        {
            try
            {
                surveyInfo = await _repo.getSurey(survey_id);
                feedbackResults = await _repo.getSurveySentimentResult(survey_id);
                dataList = await _repo.getOtherData(survey_id);
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
