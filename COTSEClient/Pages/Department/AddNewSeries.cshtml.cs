using BusinessLogic.IRepository;
using BusinessLogic.Repository;
using COTSEClient.DTO;
using COTSEClient.Helper;
using DataAccess.DTO;
using DataAccess.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OfficeOpenXml;
using System.ComponentModel;

namespace COTSEClient.Pages.Department
{
    public class AddNewSeriesModel : PageModel
    {
        public readonly IConfiguration _configuration;
        public readonly IRepositoryWorkshops _repositoryWorkshops;
        public readonly IRepositoryWorkshopSeries _repositoryWorkshopsSeries;
        public string? urlRoom;
        public string? linkCF;
        public string? emailAdmin;
        public string? passwork;
        public AddNewSeriesModel(IRepositoryWorkshops repositoryWorkshops, IRepositoryWorkshopSeries repositoryWorkshopSeries, IConfiguration configuration)
        {
            _repositoryWorkshops = repositoryWorkshops;
            _repositoryWorkshopsSeries = repositoryWorkshopSeries;
            _configuration = configuration;
            urlRoom = _configuration["ConfigWorkshop:URLRoom"];
            linkCF = _configuration["ConfigWorkshop:LinkCF"];
            emailAdmin = _configuration["AccountAdmin:Email"];
            passwork = _configuration["AccountAdmin:Passwork"];
        }

        public IFormFile Upload { get; set; }
        [BindProperty]
        public List<Workshop>? WorkShopList { get; set; }
        [BindProperty]
        public List<WorkshopDTO>? OrtherWorkShopList { get; set; }
        [BindProperty]
        public int? SeriesWorkshopId { get; set; }
        public void OnGet()
        {
            //SeriesWorkshopId = 2;
            if (SeriesWorkshopId != 0)
            {
                WorkShopList = _repositoryWorkshops.GetWorkshopBySeriesWorkshopId(SeriesWorkshopId);
                OrtherWorkShopList = new List<WorkshopDTO>();
                foreach (var item in WorkShopList)
                {
                    WorkshopDTO workshop = new WorkshopDTO();
                    OrtherWorkShopList.Add(workshop);
                }
            }
        }

        public IActionResult OnPost()
        {
            if (OrtherWorkShopList.Count() > 0)
            {
                foreach (var item in OrtherWorkShopList)
                {
                    var findWs = _repositoryWorkshops.GetWorkshopBySeriesWorkshopIdAndWorkshopName(item.WorkshopSeriesId, item.WorkshopName);
                    {
                        findWs.DatePresent = item.DatePresent;
                        TeamplatePresenter teamplatePresenter = new TeamplatePresenter
                        {
                            WorkshopName = findWs.WorkshopName,
                            TimePresenter = findWs.DatePresent.ToString(),
                            KeyPresent = findWs.KeyPresenter,
                            UrlRoom = urlRoom,
                            LinkCF = linkCF,
                        };
                        var getBody = TeamplateMail.TeamplateMailPresenter(teamplatePresenter);
                        String subject = "Thư Mời Tham Dự Làm Diễn Gỉa Phòng Nghiên Cứu Lab318 FPT";
                        HelperMethods.SendMail(getBody, subject, emailAdmin, passwork, item.Email);
                        _repositoryWorkshops.UpdateDatePresent(findWs);
                    }
                }
            }
            return Page();
        }

        public IActionResult OnPostAddNewSeries()
        {
            if (Upload != null && Upload.Length > 0)
            {
                List<SeriesWorkshopForm> seriesWorkshopForm = new List<SeriesWorkshopForm>();
                using (var stream = new MemoryStream())
                {
                    Upload.CopyTo(stream);
                    ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
                    using (var package = new ExcelPackage(stream))
                    {
                        ExcelWorksheet worksheet = package.Workbook.Worksheets[0];
                        int rowCount = worksheet.Dimension.End.Row;

                        var headers = worksheet.Cells["A1:Z1"].Select(cell => cell.Value?.ToString().Trim()).ToArray();

                        for (int row = 2; row < rowCount; row++)
                        {
                            var rowData = worksheet.Cells[row, 1, row, headers.Length].Select(cell => cell.Value?.ToString()).ToArray();

                            DateTime referenceDate = new DateTime(1900, 1, 1);
                            DateTime dateTime = referenceDate.AddDays(double.Parse(rowData[0]) - 2);

                            SeriesWorkshopForm newSeriesWorkshopForm = new SeriesWorkshopForm
                            {
                                Timestamp = dateTime,
                                Email = rowData[2],
                                NameParticipant = rowData[1],
                                CourseNumber = rowData[3],
                                Major = rowData[4],
                                FavoriteTopics = rowData[5]
                            };
                            seriesWorkshopForm.Add(newSeriesWorkshopForm);
                        }
                    }
                }
                if (seriesWorkshopForm.Count() > 0)
                {
                    WorkshopSeries workshopSeries = new WorkshopSeries
                    {
                        WorkshopSeriesName = "WorkshopSeriesName",
                        DepartmentId = 1,
                        StartDate = DateTime.Now,
                        EndDate = null
                    };
                    var resultInsert = _repositoryWorkshopsSeries.InsertWorkshopSeries(workshopSeries);
                    if (resultInsert > 0)
                    {
                        // add id
                        SeriesWorkshopId = workshopSeries.Id;

                        // Get Toppic
                        List<string> listTopic = new List<string>();
                        var favoriteTopics = seriesWorkshopForm.Select(x => x.FavoriteTopics).ToList();
                        foreach (var item in favoriteTopics)
                        {
                            var splitItem = item.Split(",").Select(s => s.Trim()).ToArray(); ;
                            listTopic.AddRange(splitItem);
                        }
                        var topicNames = listTopic
                                        .GroupBy(topic => topic)
                                        .Select(group => new { Topic = group.Key, Count = group.Count() })
                                        .OrderByDescending(x => x.Count)
                                        .ToList();

                        var resultCount = 0;
                        for (int i = 0; i < topicNames.Count(); i++)
                        {

                            Workshop workshop = new Workshop
                            {
                                WorkshopName = topicNames[i].Topic,
                                DatePresent = null,
                                WorkshopSeriesId = workshopSeries.Id,
                                KeyPresenter = HelperMethods.GenerateSecretKey(topicNames[i].Topic + workshopSeries.Id, 6),
                                StatusId = 1,
                                Index = topicNames[i].Count,
                            };
                            var resultInsertWorkshop = _repositoryWorkshops.InsertWorkshop(workshop);
                            if (resultInsertWorkshop > 0)
                            {
                                resultCount++;
                            }
                        }
                        if (resultCount > 0)
                        {
                            WorkShopList = _repositoryWorkshops.GetWorkshopBySeriesWorkshopId(workshopSeries.Id).OrderByDescending(x => x.Index).ToList();
                            OrtherWorkShopList = new List<WorkshopDTO>();
                            foreach (var item in WorkShopList)
                            {
                                WorkshopDTO workshop = new WorkshopDTO();
                                OrtherWorkShopList.Add(workshop);
                            }
                        }
                    }
                }
            }
            return Page();
        }
    }
}
