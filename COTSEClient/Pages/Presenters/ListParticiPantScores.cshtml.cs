using BusinessLogic.IRepository;
using DataAccess.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace COTSEClient.Pages.Presenters
{
    public class ListParticiPantScoresModel : PageModel
    {
        private readonly IRepositoryParticiPantScore _repositoryParticiPantScore;
        public ListParticiPantScoresModel(IRepositoryParticiPantScore repositoryParticiPantScore)
        {
            _repositoryParticiPantScore = repositoryParticiPantScore;
        }
        [BindProperty]
        public List<ParticiPantScore> ListParticiPantScore { get; set; }
        public IActionResult OnGet(int testId)
        {
            ListParticiPantScore = _repositoryParticiPantScore.GetParticiPantScoreByTestId(testId).OrderByDescending(x => x.Score).ToList();
            return Page();
        }
    }
}
