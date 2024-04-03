using BusinessLogic.IRepository;
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
        public AddSurveyToWorkshopModel(IRepositorySurvey repo)
        {
            _repo = repo;
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
        
        public async Task<IActionResult> OnGetAsync()
        {
            data = await _repo.getWorkshopInformation(wssId, wsId);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var currentPage = LocalRedirect($"/Workshop/AddSurvey?wssId={wssId}&wsId={wsId}");
            if (url != null && key != null)
            {
                ModelState.AddModelError("ERR_ILLEGAL_CALL", SurveyErrorMessage.ERR_ILLEGAL_CALL);
                return Page();
            }
            else
            {
                return Redirect($"/Surveys/series-{wssId}/workshop-{wsId}");
            }
        }
    }
}
