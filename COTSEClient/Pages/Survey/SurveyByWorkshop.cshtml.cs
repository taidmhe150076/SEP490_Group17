using BusinessLogic.IRepository;
using DataAccess.Common;
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
        public async Task OnGetAsync(string workshop_series_id, string workshop_id)
        {

            await _repo.GoogleSheetApi();
            try
            {
                var survey = _repo.getSurveyByWorkshop("1", "3");
                string s3_object = survey.SurveyKey; // get data from file_path
                string google_url = survey.SurveyUrl; // get data from google

            }
            catch (Exception e) {
                ModelState.AddModelError("ERR_WS_URL", e.Message);
            }
            
        }

    }
}
