using BusinessLogic.IRepository;
using DataAccess.Common;
using DataAccess.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace COTSEClient.Pages.Survey
{
    public class SurveyByWorkshopModel : PageModel
    {
        private readonly IRepositorySurvey _repo;

        public SurveyByWorkshopModel(IRepositorySurvey repo)
        {
            _repo = repo;
        }

        // Define properties for binding
        [BindProperty(SupportsGet = true)]
        public int surveyId { get; set; }
        [BindProperty(SupportsGet = true)]
        public int wssId { get; set; }
        [BindProperty(SupportsGet = true)]
        public int wsId { get; set; }

        [BindProperty(SupportsGet = true)]
        public string path { get; set; }



        [BindProperty(SupportsGet = true)]
        public string surveyURL { get; set; }



        [BindProperty]
        public List<FeedbackResult> feedbackResults { get; set; } = null!;

        [BindProperty]
        public Dictionary<string, int> feedbackCount { get; set; } = null!;

        [BindProperty]
        public bool state_display { get; set; } = false;

        public async Task<IActionResult> OnGetAsync()
        {
            var key = await _repo.getWorkshopData(wssId, wsId, surveyURL);
            string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "temp", key);
            await Console.Out.WriteLineAsync(filePath);
            try
            {
                feedbackCount = new Dictionary<string, int>();
                var questions = _repo.GetSentimentAnswer(filePath);
                var json_data = await _repo.GetJsonSentiment(questions);
                feedbackResults = _repo.Rate(questions, json_data);
                feedbackCount.Add("Positive", feedbackResults.Where(feedback => feedback.getResult() == "Positive").Count());
                feedbackCount.Add("Negative", feedbackResults.Where(feedback => feedback.getResult() == "Negative").Count());
                feedbackCount.Add("Neutral", feedbackResults.Where(feedback => feedback.getResult() == "Neutral").Count());
                state_display = true;
            }
            catch (Exception e)
            {
                ModelState.AddModelError("File", e.Message);
            }
            return Page();

        }



    }
}
