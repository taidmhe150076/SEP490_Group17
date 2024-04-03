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
        [BindProperty(SupportsGet = true)]
        public string surveyId { get; set; }
        [BindProperty(SupportsGet = true)]
        public string wssId { get; set; }
        [BindProperty(SupportsGet = true)]
        public string wsId { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            return Page();
        }



    }
}
