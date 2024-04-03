using BusinessLogic.IRepository;
using BusinessLogic.Validator;
using DataAccess.Constants;
using DataAccess.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace COTSEClient.Pages.Survey
{
    public class AddSurveyToWorkshopModel : PageModel
    {

        private readonly IRepositorySurvey _repo;
        private readonly SurveyValidator validator;
        public AddSurveyToWorkshopModel(IRepositorySurvey repo)
        {
            _repo = repo;
            validator = new SurveyValidator();
        }
        [BindProperty]
        public WorkshopInfoDTO data { get; set; } = null!;

        [BindProperty(SupportsGet = true)]
        public int wssId { get; set; }

        [BindProperty(SupportsGet = true)]
        public int wsId { get; set; }

        [BindProperty]
        public string? url { get; set; } = null!;

        [BindProperty]
        public IFormFile? key { get; set; } = null!;

        public void OnGet()
        {
            data = _repo.getWorkshopInformation(wssId, wsId);
        }

        public async Task<IActionResult> OnPostAsync()
        {
            bool url_selected = false;
            if (url != null && key != null)
            {
                TempData["err_mess"] = SurveyErrorMessage.ERR_ILLEGAL_CALL;
                return Redirect($"/Workshop/AddSurvey?wssId={wssId}&wsId={wsId}");
            }
            else
            {
                if (url != null) { url_selected = true; } // check if is url
                if (url_selected)
                {
                    //validate url
                    if (!validator.validateUrlPath(url))
                    {
                        TempData["err_mess"] = SurveyErrorMessage.ERR_SURVEY_FAIL;
                        return Redirect($"/Workshop/AddSurvey?wssId={wssId}&wsId={wsId}");
                    }
                    else
                    {
                        WorkshopInfoDTO new_survey = data;
                        new_survey.url = url;
                    }
                }
                else
                {
                    //valudate file
                    if (!validator.validateFileName(key.FileName))
                    {
                        TempData["err_mess"] = SurveyErrorMessage.ERR_SURVEY_FAIL;
                        return Redirect($"/Workshop/AddSurvey?wssId={wssId}&wsId={wsId}");
                    }
                    else {
                        string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "temp", key.FileName);
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await key.CopyToAsync(stream);
                        };
                        WorkshopInfoDTO new_survey = data;
                        new_survey.url = key.FileName;
                        await Console.Out.WriteLineAsync(filePath);
                    }
                }
                return Redirect($"/Surveys/series-{wssId}/workshop-{wsId}");
            }
        }
    }
}
