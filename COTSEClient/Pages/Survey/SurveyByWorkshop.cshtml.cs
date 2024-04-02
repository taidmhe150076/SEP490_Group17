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

        // Define properties for binding
        public string surveyId { get; set; }
        public string wssId { get; set; }
        public string wsId { get; set; }

        public async Task OnGetAsync(int wssId, int wsId, int survey_id)
        {

        }



    }
}
