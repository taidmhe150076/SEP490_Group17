using BusinessLogic.IRepository;
using DataAccess.Constants;
using DataAccess.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace COTSEClient.Pages.Survey
{
    public class UpdateSurveyModel : PageModel
    {
        private readonly IRepositorySurvey _repositorySurvey;

        public UpdateSurveyModel(IRepositorySurvey repositorySurvey)
        {
            _repositorySurvey = repositorySurvey;
        }

        [BindProperty]
        public WorkshopInfoDTO data { get; set; } = null!;

        [BindProperty(SupportsGet = true)]
        public int wssId { get; set; }

        [BindProperty(SupportsGet = true)]
        public int wsId { get; set; }


        [BindProperty(SupportsGet = true)]
        public int surveyId { get; set; }

        [BindProperty]
        public string formUrl { get; set; } = null!;

        [BindProperty]
        public string? url { get; set; } = null!;

        [BindProperty]
        public IFormFile? key { get; set; } = null!;

        [BindProperty]
        public bool IsPresenterSurvey { get; set; }

        private List<string> file_types = new List<string>() {
            ".csv",
            ".xlsx",
            "csv",
            "xlsx",
        };
        public IActionResult OnGet()
        {
            try
            {
                var x = surveyId;
                data = _repositorySurvey.getCurrentWorkshopInformation(wssId, wsId, surveyId);
                return Page();
            }
            catch (Exception ex)
            {

                TempData["err_mess"] = SurveyErrorMessage.ERR_ILLEGAL_CALL;

                return RedirectToPage("SurveyList");
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            string filePath = "";
            var state = -1;
            var updated_data = data;
            updated_data.survey_id = surveyId;
            updated_data.wssId = wssId;
            updated_data.wsId = wsId;

            if (key != null)
            {
                state = COTSEConstants.MODE_ADD_FILE;
                filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "temp", key.FileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await key.CopyToAsync(stream);
                };
                updated_data.url = filePath;
            }
            else if(url != null){
                updated_data.url = url;
                state = COTSEConstants.MODE_ADD_URL;
            }
            updated_data.FormUrl = formUrl;
            int result = -1;
            try
            {
                result = await _repositorySurvey.updateSurvey(updated_data, state);
                if (result == COTSEConstants.DB_STATUS_FAIL)
                {
                    TempData["err_mess"] = SurveyErrorMessage.ERR_SURVEY_FAIL;
                    return RedirectToPage("UpdateSurvey", new { wssId = wssId, wsId = wsId, surveyId = surveyId });
                }
                else if (result >= COTSEConstants.DB_STATUS_SUCCESS)
                {
                    return RedirectToPage("SurveyList");
                }
                else if (result >= COTSEConstants.DB_STATUS_EXIST)
                {
                    TempData["err_mess"] = SurveyErrorMessage.ERR_SURVEY_FAIL;
                    return RedirectToPage("UpdateSurvey", new { wssId = wssId, wsId = wsId, surveyId = surveyId });
                }
                else
                {
                    return RedirectToPage("SurveyList");
                }
            }
            catch (Exception e)
            {
                TempData["err_mess"] = e.Message;
                return RedirectToPage("UpdateSurvey", new { wssId = wssId, wsId = wsId, surveyId = surveyId });
            }
            finally {
                if (System.IO.File.Exists(filePath)) {
                    if (file_types.Contains(Path.GetExtension(filePath))) {
                        System.IO.File.Delete(filePath);
                    }
                }
            }
        }
    }
}
