using BusinessLogic.IRepository;
using DataAccess.FileControl;
using DataAccess.Models;
using LiveCharts;
using LiveCharts.Defaults;

namespace BusinessLogic.Repository
{
    public class RepositoryGPACapstone : IRepositoryGPACapstone
    {
        List<GPAScoreRange> IRepositoryGPACapstone.GetValueGPACapstoneByMajorAndYear(string major, int year)
        {

            List<string> listScoreRange = new List<string>()
            {
                "0-1",
                "1-2",
                "2-3",
                "3-4",
                "4-5",
                "5-6",
                "6-7",
                "7-8",
                "8-9",
                "9-10"
            };
            List<GradesGPA> readCsvTask = FileControl.Instance.GetObjectList();

            var dataQuery = readCsvTask.Where(x => x.Is_Capstone == true && x.SemesterCredit == 10 && x.MemberMajor.Equals(major) && x.Year == year).ToList();

            var groupedData = dataQuery.GroupBy(record =>
            {
                double gpa = record.Gpa;
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

            foreach (var item in groupedData)
            {
                Console.WriteLine(item.ScoreRange + item.Count);
            }

            foreach (var range in listScoreRange)
            {
                if (!groupedData.Any(group => group.ScoreRange == range))
                {
                    groupedData.Add(new GPAScoreRange { ScoreRange = range, Count = 0 });
                }
            }

            var groupedDataSorted = groupedData.OrderBy(group => group.ScoreRange).ToList();

            return groupedDataSorted;
        }
    }
}
