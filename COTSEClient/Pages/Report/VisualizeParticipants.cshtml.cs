using BusinessLogic.IRepository;
using DataAccess.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace COTSEClient.Pages.Report
{
    public class VisualizeParticipantsModel : PageModel
    {
        private readonly IRepositorySlideWorkshop _repositorySlideWorkshop;


        public VisualizeParticipantsModel(IRepositorySlideWorkshop repositorySlideWorkshop)
        {
            _repositorySlideWorkshop = repositorySlideWorkshop;
        }
        [BindProperty]
        public string? Msg { get; set; }
        [BindProperty]
        public List<ImagesWorkShop>? ListChartSeries { get; set; }

        public IActionResult OnGet(int? wsId)
        {
            try
            {
                ListChartSeries = _repositorySlideWorkshop.GetAllChartVideoWorkshop((int)wsId);
                if (ListChartSeries.Count == 0)
                {
                    Msg = "Biểu Đồ Hiện Tại Đang Trong Quá Trình Đợi Researcher Export!!!";
                    return Page();
                }
                return Page();
            }
            catch (Exception)
            {
                Msg = "Username Not Exits In System";
                return Page();
            }
        }
    }
}
