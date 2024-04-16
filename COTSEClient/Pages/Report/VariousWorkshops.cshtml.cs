using BusinessLogic.IRepository;
using DataAccess.DTO;
using DataAccess.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace COTSEClient.Pages.Report
{
    public class VariousWorkshopsModel : PageModel
    {
        public readonly Sep490G17DbContext context = new Sep490G17DbContext();
        public readonly IRepositoryWorkshops _repositoryWorkshops;
        public VariousWorkshopsModel(IRepositoryWorkshops repositoryWorkshops)
        {
            _repositoryWorkshops = repositoryWorkshops;
        }
        [BindProperty]
        public List<Workshop>? WorkShopList { get; set; }
        public List<ParticipantDTO>? ListParticipant { get; set; }
        public List<Workshop>? ListWorkshops { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        [BindProperty]
        public string SeriesWsName { get; set; }
        public void OnGet(int srId)
        {
            if (srId != 0)
            {
                var listdata = context.Participants.Where(x => x.WorkshopSeriesId == srId).OrderBy(x => x.TimeStamp).ToList();
                ListParticipant = listdata.Select(x => new ParticipantDTO
                {
                    Email = x.Email,
                    FavoriteTopics = x.FavoriteTopics,
                    Major = x.Major,
                    FullName = x.FullName,
                    TimeStamp = x.TimeStamp,
                    WorkshopSeriesId = x.WorkshopSeriesId,
                }).ToList();
                WorkShopList = _repositoryWorkshops.GetWorkshopBySeriesWorkshopId(srId);
                SeriesWsName = context.WorkshopSeries.FirstOrDefault(x => x.Id ==  srId).WorkshopSeriesName;
                //ListWorkshops = context.Workshops.ToList();
                StartDate = ListParticipant.First().TimeStamp;
                EndDate = ListParticipant.Last().TimeStamp;
            }
        }
    }
}
