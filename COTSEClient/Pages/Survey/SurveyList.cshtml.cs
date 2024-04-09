using BusinessLogic.IRepository;
using DataAccess.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace COTSEClient.Pages.Survey
{
    public class SurveyListModel : PageModel
    {

        private readonly IRepositorySurvey _repo;
        public SurveyListModel(IRepositorySurvey repo)
        {
            _repo = repo;
        }

        [BindProperty]
        public List<WorkshopSeriesWorkshopDTO> list_data { get; set; } = null!;

        [BindProperty]


        public int wssId { get; set; }
        [BindProperty]
        public int wsId { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            list_data = await _repo.seriesSurvey();
            return Page();
        }
    }
}
