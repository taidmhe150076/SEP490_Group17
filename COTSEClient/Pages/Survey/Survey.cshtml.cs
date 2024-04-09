using BusinessLogic.IRepository;
using BusinessLogic.Repository;
using BusinessLogic.Validator;
using DataAccess.Common;
using DataAccess.Constants;
using DataAccess.DTO;
using DataAccess.Models;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using System.Text;

namespace COTSEClient.Pages.Survey
{
    public class SurveyModel : PageModel
    {
        private readonly IRepositorySurvey _repo;
        private readonly SurveyValidator _validator;

        public SurveyModel(IRepositorySurvey repo)
        {
            _repo = repo;
            _validator = new SurveyValidator();
        }

        [BindProperty]
        public List<FeedbackResult> feedbackResults { get; set; } = null!;

        [BindProperty]
        public Dictionary<string, int> feedbackCount { get; set; } = null;

        public void OnGet()
        {
        }
        [BindProperty]
        public bool state_display { get; set; } = false;

        [BindProperty]
        public List<IFormFile> fileSurveys { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            if (fileSurveys.Count == 1)
            {
                if (fileSurveys[0] == null)
                {
                    ModelState.AddModelError("File", "File not imported");
                    return Page();
                }
                if (!_validator.validateFileName(fileSurveys[0].FileName))
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
                    feedbackResults = await _repo.getSurveySentimentResult(1);
                    feedbackCount = await _repo.CountFeedback(feedbackResults);
                    state_display = true;
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

            else if (fileSurveys == null || fileSurveys.Count == 0)
            {
                ModelState.AddModelError("ERR_FILES_LOAD", SurveyErrorMessage.ERR_FILES_LOAD);
                return Page();
            }
            else
            {
                var list_file_name = fileSurveys.Select(x => x.FileName).ToList();
                var validate_data = _validator.validateFilesName(list_file_name);
                bool all_validate = validate_data.All(kv => kv.Item2 == SurveyConstant.VALID_NAME_FORMAT);
                if (!all_validate)
                {
                    string error_message = SurveyErrorMessage.ERR_FILENAMES_FORMAT;
                    var wrong_files_format = validate_data.Where(kv => kv.Item2 != SurveyConstant.VALID_NAME_FORMAT).Select(kv => kv.Item1).ToList();

                    var test = validate_data.Where(kv => kv.Item2 != SurveyConstant.VALID_NAME_FORMAT).ToList();
                    foreach (var t in test)
                    {
                        await Console.Out.WriteLineAsync($"key:{t.Item1}");
                        await Console.Out.WriteLineAsync($"value:{t.Item2}");
                    }
                    var message_list = "(";
                    foreach (var wrong_file_format in wrong_files_format)
                    {
                        message_list += wrong_file_format + ",";
                    }
                    message_list += ")";
                    string output = error_message.Replace("{files}", message_list);
                    ModelState.AddModelError("ERR_FILENAMES_FORMAT", output);
                    return Page();
                }
            }
            return Page();
        }

    }
}
