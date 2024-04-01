using BusinessLogic.IRepository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace COTSEClient.Pages.Survey
{
    public class ListSurveyWorkshopModel : PageModel
    {
        private readonly IRepositorySurvey _repo;

        public ListSurveyWorkshopModel(IRepositorySurvey repo)
        {
            _repo = repo;
        }
        public void OnGet(int wssId, int wsId)
        {
            Console.WriteLine(wssId);
            Console.WriteLine(wsId);
        }
    }
}
