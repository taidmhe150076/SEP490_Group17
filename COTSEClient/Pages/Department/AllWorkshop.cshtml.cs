using BusinessLogic.IRepository;
using DataAccess.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace COTSEClient.Pages.Department
{
    public class AllWorkshopModel : PageModel
    {
        public readonly IRepositoryWorkshops _repositoryWorkshop;
        public AllWorkshopModel(IRepositoryWorkshops repositoryWorkshop)
        {
            _repositoryWorkshop = repositoryWorkshop;
        }
        [BindProperty]
        public List<Workshop> Workshops {  get; set; }
        [BindProperty]
        public int WorkshopSeriesId { get; set; }
        public IActionResult OnGet(int wsSeriesId)
        {
            WorkshopSeriesId = wsSeriesId;
            Workshops = _repositoryWorkshop.GetWorkshopBySeriesWorkshopId(wsSeriesId);
            return Page();
        }
    }
}
