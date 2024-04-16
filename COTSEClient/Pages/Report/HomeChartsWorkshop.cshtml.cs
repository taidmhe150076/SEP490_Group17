using BusinessLogic.IRepository;
using DataAccess.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace COTSEClient.Pages.Report
{
    public class HomeChartsWorkshopModel : PageModel
    {
        [BindProperty]
        public int? WorkshopId { get; set; }
        [BindProperty]
        public int? WSSeriesId { get; set; }
        public IActionResult OnGet(int? workShopId, int? wsSeriesId)
        {
            WorkshopId = workShopId;
            WSSeriesId = wsSeriesId;
            return Page();
        }
    }
}
