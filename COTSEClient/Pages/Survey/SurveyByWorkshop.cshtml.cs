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
        public void OnGet(string workshop_series_id, string workshop_id)
        {
            try
            {
                string url = _repo.getSurveyByWorkshop("1", "3").SurveyUrl1;
                var is_https = url.Split(":")[0] == "https";
                if (is_https) {
                    _repo.GoogleSheetApi();
                }
                Console.WriteLine(url);
            }
            catch (Exception e) {
                ModelState.AddModelError("ERR_WS_URL", e.Message);
            }
            
        }

    }
}
