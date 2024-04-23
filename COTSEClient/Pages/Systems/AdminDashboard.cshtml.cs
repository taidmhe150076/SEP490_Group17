using BusinessLogic.IRepository;
using DataAccess.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;

namespace COTSEClient.Pages.Systems
{
    public class AdminDashboardModel : PageModel
    {
        private readonly IRepositoryWorkshops _workshopRepository;
        private readonly IRepositoryUser _userRepository;

        public AdminDashboardModel(IRepositoryUser userRepository, IRepositoryWorkshops workshopRepository)
        {
            _userRepository = userRepository;
            _workshopRepository = workshopRepository;
        }

        public int TotalUsers { get; private set; }
        public int TotalWorkshops { get; private set; }
        public string Months { get; private set; }
        public string WorkshopCounts { get; private set; }

        public void OnGet()
        {
            TotalUsers = _userRepository.getAllUser().Count();
            TotalWorkshops = _workshopRepository.GetWorkshops().Where(X => X.StatusId == 2).Count();

            GetChartData(DateTime.Now.Year);
        }

        public IActionResult OnGetChartData(int year)
        {
            GetChartData(year);
            return new JsonResult(new { Months, WorkshopCounts });
        }

        private void GetChartData(int year)
        {
            var workshopCountsByMonth = _workshopRepository.GetWorkshops()
                .Where(w => w.DatePresent != null && w.DatePresent.Value.Year == year)
                .GroupBy(w => w.DatePresent.Value.Month)
                .OrderBy(g => g.Key)
                .Select(g => new { Month = g.Key, Count = g.Count() })
                .ToList();

            var monthsList = Enumerable.Range(1, 12).ToList();

            for (int month = 1; month <= 12; month++)
            {
                if (!workshopCountsByMonth.Any(w => w.Month == month))
                {
                    workshopCountsByMonth.Add(new { Month = month, Count = 0 });
                }
            }

            workshopCountsByMonth = workshopCountsByMonth.OrderBy(w => w.Month).ToList();

            var countsList = workshopCountsByMonth.Select(w => w.Count).ToList();

            Months = string.Join(",", monthsList);
            WorkshopCounts = string.Join(",", countsList);
        }
    }
}
