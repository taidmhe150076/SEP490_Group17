using BusinessLogic.IRepository;
using BusinessLogic.Repository;
using DataAccess.Common;
using DataAccess.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using System.Text;

namespace COTSEClient.Pages.Survey
{
    public class SurveyModel : PageModel
    {
        private readonly string HUGGING_FACE_TOKEN = "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";
        private readonly string API_URL = "https://api-inference.huggingface.co/models/wonrax/phobert-base-vietnamese-sentiment";

        private IRepositorySurvey _repo = new RepositorySurvey();

#pragma warning disable CS8618 
        public SurveyModel()
#pragma warning restore CS8618 
        {
        }

        [BindProperty]
        public List<FeedbackResult> feedbackResults { get; set; } = null!;

        [BindProperty]
        public Dictionary<string, int> feedbackCount { get; set; } = null;

        public void OnGet()
        {

        }

        [BindProperty]
        public List<IFormFile> fileSurveys { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            if (fileSurveys.Count == 1) {
                if (fileSurveys[0] == null)
                {
                    ModelState.AddModelError("File", "File not imported");
                    return Page();
                }
                if (!_repo.validateFileName(fileSurveys[0].FileName))
                {
                    ModelState.AddModelError("File", "wrong file format");
                    return Page();
                }
                string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "temp", fileSurveys[0].FileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await fileSurveys[0].CopyToAsync(stream);
                };
                try
                {
                    var questions = _repo.GetSentimentAnswer(filePath);
                    var json_data = await getFeedbackJsonString(questions);
                    feedbackResults = _repo.Rate(questions, json_data);
                    feedbackCount.Add("Positive", feedbackResults.Where(feedback => feedback.getResult() == "Positive").Count());
                    feedbackCount.Add("Negative", feedbackResults.Where(feedback => feedback.getResult() == "Negative").Count());
                    feedbackCount.Add("Neutral", feedbackResults.Where(feedback => feedback.getResult() == "Neutral").Count());
                    //feedbackCount.Add("Very bad", feedbackResults.Where(feedback => feedback.startRating() == 1).Count());
                    //feedbackCount.Add("Bad", feedbackResults.Where(feedback => feedback.startRating() == 2).Count());
                    //feedbackCount.Add("Neutral", feedbackResults.Where(feedback => feedback.startRating() == 3).Count());
                    //feedbackCount.Add("Good", feedbackResults.Where(feedback => feedback.startRating() == 4).Count());
                    //feedbackCount.Add("Very Good", feedbackResults.Where(feedback => feedback.startRating() == 5).Count());
                }
                catch (Exception e)
                {
                    ModelState.AddModelError("File", e.Message);
                }

            }

            if (fileSurveys == null || fileSurveys.Count == 0) {
                ModelState.AddModelError("ERR_FILES_LOAD", SurveyErrorMessage.ERR_FILES_LOAD);
                return Page();
            }
            var list_file_name = fileSurveys.Select(x => x.FileName).ToList();
            bool all_validate = _repo.validateFilesName(list_file_name).All(kv => kv.Item2);
            if (!all_validate)
            {
                string error_message = SurveyErrorMessage.ERR_FILENAMES_FORMAT;

                ModelState.AddModelError("","");
            }
            return Page();
        }


        //get json feedback
        private async Task<string> getFeedbackJsonString(List<string> sentiment_data_list)
        {
            using (HttpClient client = new HttpClient())
            {
                var json_string = JsonConvert.SerializeObject(sentiment_data_list);
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {HUGGING_FACE_TOKEN}");
                client.DefaultRequestHeaders.Add("Accept", "application/json");
                var content = new StringContent(json_string, Encoding.UTF8, "application/json");
                HttpResponseMessage resp = await client.PostAsync(API_URL, content);
                if (resp.IsSuccessStatusCode)
                {
                    string json_data = await resp.Content.ReadAsStringAsync();
                    return json_data;
                }
                else
                {
                    throw new Exception(resp.Content.ToString());
                }
            }
        }
    }
}
