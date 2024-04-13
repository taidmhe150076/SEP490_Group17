using BusinessLogic.IRepository;
using COTSEClient.Models;
using DataAccess.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace COTSEClient.Pages.Report
{
    public class TestWorkShopReportModel : PageModel
    {
        private readonly IRepositoryWorkshops _repositoryWorkshops;
        private readonly IRepositoryTests _repositoryTests;
        public TestWorkShopReportModel(IRepositoryWorkshops repositoryWorkshops, IRepositoryTests repositoryTests)
        {
            _repositoryWorkshops = repositoryWorkshops;
            _repositoryTests = repositoryTests;
        }
        readonly List<string> listScoreRange = new List<string>() { "0-1", "1-2", "2-3", "3-4", "3-4", "4-5", "5-6", "6-7", "7-8", "8-9", "9-10" };
        [BindProperty]
        public List<TestGPAScoreRangeDTO> TestGPAScoreRangeDTO { get; set; } = new List<TestGPAScoreRangeDTO>();
        [BindProperty]
        public List<Test>? InfoTest { get; set; }
        public string? WorkShopName { get; set; }

        public void OnGet(int workShopId = 3)
        {
            var getData = _repositoryWorkshops.GetParticiPantScoresByWorkshopId(workShopId);
            WorkShopName = _repositoryWorkshops.GetWorkshopByWorkshopId(workShopId)?.WorkshopName;
            InfoTest = _repositoryTests.GetScoresTestsByWorkshopId(workShopId);
            foreach (var data in getData)
            {
                foreach (var test in data.Tests)
                {
                    var gPAScoreRange = GetGPAScoreRange(test.ParticiPantScores);

                    TestGPAScoreRangeDTO newTestGPAScoreRangeDTO = new TestGPAScoreRangeDTO()
                    {
                        Name = test.TestName,
                        GPAScoreRange = gPAScoreRange,
                    };
                    TestGPAScoreRangeDTO.Add(newTestGPAScoreRangeDTO);
                }
            }
        }

        public List<GPAScoreRange> GetGPAScoreRange(ICollection<ParticiPantScore> particiPantScore)
        {
            var getGPAScoreRange = particiPantScore.GroupBy(record =>
            {
                double gpa = record.Score;
                if (gpa >= 0 && gpa <= 1)
                    return 0;
                else if (gpa > 1 && gpa <= 2)
                    return 1;
                else if (gpa > 2 && gpa <= 3)
                    return 2;
                else if (gpa > 3 && gpa <= 4)
                    return 3;
                else if (gpa > 4 && gpa <= 5)
                    return 4;
                else if (gpa > 5 && gpa <= 6)
                    return 5;
                else if (gpa > 6 && gpa <= 7)
                    return 6;
                else if (gpa > 7 && gpa <= 8)
                    return 7;
                else if (gpa > 8 && gpa <= 9)
                    return 8;
                else if (gpa > 9 && gpa <= 10)
                    return 9;
                else
                    return -1;
            })
            .Select(group => new GPAScoreRange
            {
                ScoreRange = $"{group.Key}-{group.Key + 1}",
                Count = group.Count()
            })
            .ToList();
            foreach (var range in listScoreRange)
            {
                if (!getGPAScoreRange.Any(group => group.ScoreRange == range))
                {
                    getGPAScoreRange.Add(new GPAScoreRange { ScoreRange = range, Count = 0 });
                }
            }

            var sort = getGPAScoreRange.OrderBy(group => group.ScoreRange).ToList();

            return sort;
        }
    }
}