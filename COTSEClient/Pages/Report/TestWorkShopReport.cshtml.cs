using BusinessLogic.IRepository;
using COTSEClient.Models;
using DataAccess.Constants;
using DataAccess.DTO;
using DataAccess.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Scripting.Utils;
using System.Collections.Generic;

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
        public List<InfoExamDTO>? InfoTestPre { get; set; }
        public List<InfoExamDTO>? InfoTestPos { get; set; }
        public BubbleChartGPADTO? BubbleChartGPAPRE { get; set; }
        public BubbleChartGPADTO BubbleChartGPAPOST { get; set; }
        public string? WorkShopName { get; set; }

        public int? ParticiPantDoPre { get; set; }
        public int? ParticiPantDoPost { get; set; }


        public void OnGet(int workShopId)
        {
            var getData = _repositoryWorkshops.GetParticiPantScoresByWorkshopId(workShopId);
            WorkShopName = _repositoryWorkshops.GetWorkshopByWorkshopId(workShopId)?.WorkshopName;
            InfoTest = _repositoryTests.GetScoresTestsByWorkshopId(workShopId);
            InfoTestPre = InfoTest.Where(x => x.TestTypeId == COTSEConstants.TEST_PRE).SelectMany(x => x.ParticiPantScores).ToList()
                                    .Select(x => new InfoExamDTO
                                    {
                                        ParticiPantName = x.Participant.ParticipantsEmail,
                                        TimeSubmit = x.SubmissionTime == null ? null : (DateTime)x.SubmissionTime,
                                        ParticiPantScores = (int?)x.Score
                                    }).ToList();
            InfoTestPos = InfoTest.Where(x => x.TestTypeId == COTSEConstants.TEST_POST).SelectMany(x => x.ParticiPantScores).ToList()
                                    .Select(x => new InfoExamDTO
                                    {
                                        ParticiPantName = x.Participant.ParticipantsEmail,
                                        TimeSubmit = x.SubmissionTime == null ? null : (DateTime)x.SubmissionTime,
                                        ParticiPantScores = (int?)x.Score
                                    }).ToList();


            if (InfoTestPre.Count > 0 && InfoTestPos.Count > 0)
            {
                
                BubbleChartGPAPRE = new BubbleChartGPADTO
                {
                    AverageScore = InfoTestPre
                                   .Where(x => x.ParticiPantScores.HasValue)
                                   .Average(x => x.ParticiPantScores.Value).ToString("N2"),
                    NumberParticipant = InfoTestPre.Count(),
                    Width = InfoTestPre.Count() < 10 ? InfoTestPre.Count() * 4 : InfoTestPre.Count() * 2
                };

                BubbleChartGPAPOST = new BubbleChartGPADTO
                {
                    AverageScore = InfoTestPos
                                        .Where(x => x.ParticiPantScores.HasValue)
                                        .Average(x => x.ParticiPantScores.Value).ToString("N2"),
                    NumberParticipant = InfoTestPos.Count(),
                    Width = InfoTestPos.Count() < 10 ? InfoTestPos.Count() * 4 : InfoTestPos.Count() * 2
                };


                List<InfoExamDTO> matchedItemsPre = new List<InfoExamDTO>();
                List<InfoExamDTO> matchedItemsPos = new List<InfoExamDTO>();

                foreach (var item1 in InfoTestPre)
                {
                    var matchingItem = InfoTestPos.FirstOrDefault(item2 => item2.ParticiPantName == item1.ParticiPantName);

                    if (matchingItem != null)
                    {
                        matchedItemsPre.Add(item1);
                        matchedItemsPos.Add(matchingItem);
                    }
                }

                InfoTestPre = matchedItemsPre;
                InfoTestPos = matchedItemsPos;

                ParticiPantDoPre = InfoTest.FirstOrDefault(x => x.TestTypeId == COTSEConstants.TEST_PRE).ParticiPantScores.Count();
                ParticiPantDoPost = InfoTest.FirstOrDefault(x => x.TestTypeId == COTSEConstants.TEST_POST).ParticiPantScores.Count();
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
