using BusinessLogic.IRepository;
using DataAccess.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace COTSEClient.Pages.Department
{
    public class AllWorkshopModel : PageModel
    {
        public readonly IRepositoryWorkshops _repositoryWorkshop;
        public readonly IRepositoryWorkshopSeries _repositoryWorkshopSeries ;
        public AllWorkshopModel(IRepositoryWorkshops repositoryWorkshop, IRepositoryWorkshopSeries repositoryWorkshopSeries)
        {
            _repositoryWorkshop = repositoryWorkshop;
            _repositoryWorkshopSeries = repositoryWorkshopSeries;
        }
        [BindProperty]
        public List<Workshop> Workshops {  get; set; }
        [BindProperty]
        public int WorkshopSeriesId { get; set; }
        [BindProperty]
        public WorkshopSeries workshopSeries { get; set; }
        public IActionResult OnGet(int wsSeriesId)
        {
            try
            {
                WorkshopSeriesId = wsSeriesId;
                workshopSeries = _repositoryWorkshopSeries.GetWorkshopSeriesById(wsSeriesId);
                Workshops = _repositoryWorkshop.GetWorkshopBySeriesWorkshopId(wsSeriesId);
                return Page();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);    
            }
           
        }
    }
}
