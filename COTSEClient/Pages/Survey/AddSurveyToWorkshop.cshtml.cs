using BusinessLogic.IRepository;
using BusinessLogic.Validator;
using DataAccess.Constants;
using DataAccess.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace COTSEClient.Pages.Survey
{
    [Authorize(Roles = COTSEConstants.ROLE_RESEARCHER + "," + COTSEConstants.ROLE_ORGANIZER)]
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
        public string formUrl { get; set; } = null!;

        [BindProperty]
        public IFormFile? key { get; set; } = null!;

        [BindProperty]
        public bool IsPresenterSurvey { get; set; }

        public async void OnGetAsync()
        {
            try
            {

                data = _repo.getWorkshopInformation(wssId, wsId);
            }
            catch (Exception) {
            
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            int result = -1;
            if (formUrl == null || formUrl.Trim().Length == 0)
            {
                TempData["err_mess"] = SurveyErrorMessage.ERR_FORM_URL_PROVIDE;
                return RedirectToPage("AddSurveyToWorkshop", new { wssId = wssId, wsId = wsId });
            }
            if (key != null)
            {
                //valudate file
                if (!validator.validateFileName(key.FileName))
                {
                    TempData["err_mess"] = SurveyErrorMessage.ERR_SURVEY_FAIL;
                    return RedirectToPage("AddSurveyToWorkshop", new { wssId = wssId, wsId = wsId });
                }
                else
                {
                    string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "temp", key.FileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await key.CopyToAsync(stream);
                    };
                    WorkshopInfoDTO new_survey = data;
                    new_survey.url = filePath;
                    new_survey.FormUrl = formUrl;
                    new_survey.isPresenter = IsPresenterSurvey;
                    try
                    {
                        int attempt = 0;
                        while (true) {
                            result = await _repo.createNewSurveyFile(new_survey);
                            if (result != 0) {
                                break;
                            }
                            attempt++;
                            if (attempt == 3 && result == 0) {
                                throw new Exception("out of time");
                            }
                        }
                    }
                    catch (Exception)
                    {

                        TempData["err_mess"] = SurveyErrorMessage.ERR_SURVEY_FAIL;
                        return RedirectToPage("AddSurveyToWorkshop", new { wssId = wssId, wsId = wsId });

                    }
                }
            }
            else
            {
                try
                {
                    WorkshopInfoDTO new_survey = data;
                    new_survey.FormUrl = formUrl;
                    new_survey.isPresenter = IsPresenterSurvey;
                    result = await _repo.createNewSurvey(new_survey);
                }
                catch (Exception)
                {

                    TempData["err_mess"] = SurveyErrorMessage.ERR_SURVEY_FAIL;
                    return RedirectToPage("AddSurveyToWorkshop", new { wssId = wssId, wsId = wsId });
                }
            }

            if (result == COTSEConstants.DB_STATUS_FAIL)
            {
                TempData["err_mess"] = SurveyErrorMessage.ERR_SURVEY_FAIL;
                return RedirectToPage("AddSurveyToWorkshop", new { wssId = wssId, wsId = wsId });
            }
            else if (result >= COTSEConstants.DB_STATUS_SUCCESS)
            {
                return RedirectToPage("SurveyList");
            }
            else if (result >= COTSEConstants.DB_STATUS_EXIST)
            {
                TempData["err_mess"] = SurveyErrorMessage.ERR_SURVEY_FAIL;
                return RedirectToPage("AddSurveyToWorkshop", new { wssId = wssId, wsId = wsId });
            }
            else
            {
                TempData["err_mess"] = SurveyErrorMessage.ERR_ILLEGAL_CALL;
                return RedirectToPage("AddSurveyToWorkshop", new { wssId = wssId, wsId = wsId });
            }
        }
    }
}
