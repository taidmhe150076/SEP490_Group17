using BusinessLogic.IRepository;
using BusinessLogic.Repository;
using COTSEClient.DTO;
using COTSEClient.Helper;
using DataAccess.Constants;
using DataAccess.DTO;
using DataAccess.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OfficeOpenXml;
using System.ComponentModel;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace COTSEClient.Pages.Department
{
    [Authorize(Roles = COTSEConstants.ROLE_ORGANIZER)]
    public class AddNewSeriesModel : PageModel
    {
        private readonly Sep490G17DbContext _context = new Sep490G17DbContext();
        public readonly IConfiguration _configuration;
        public readonly IRepositoryWorkshops _repositoryWorkshops;
        public readonly IRepositoryWorkshopSeries _repositoryWorkshopsSeries;
        public readonly IRepositoryPresenter _repositoryPresenter;
        public readonly IRepositoryParticipants _repositoryParticipants;
        public readonly IRepositoryAssign _repositoryAssign;
        public readonly IRepositoryUser _repositoryUser;
        public string? urlRoom;
        public string? linkCF;
        public string? linkDownloadTools;
        public string? baseURL;
        public string? emailAdmin;
        public string? passwork;
        public AddNewSeriesModel(IRepositoryWorkshops repositoryWorkshops, IRepositoryWorkshopSeries repositoryWorkshopSeries, IRepositoryPresenter repositoryPresenter, IRepositoryParticipants repositoryParticipants, IRepositoryAssign repositoryAssign, IRepositoryUser repositoryUser, IConfiguration configuration)
        {
            _repositoryWorkshops = repositoryWorkshops;
            _repositoryWorkshopsSeries = repositoryWorkshopSeries;
            _repositoryPresenter = repositoryPresenter;
            _repositoryParticipants = repositoryParticipants;
            _repositoryAssign = repositoryAssign;
            _repositoryUser = repositoryUser;
            _configuration = configuration;
            urlRoom = _configuration["ConfigWorkshop:URLRoom"];
            linkCF = _configuration["ConfigWorkshop:LinkCF"];
            baseURL = _configuration["BaseURL"];
            linkDownloadTools = _configuration["LinkDowloandTools"];
            emailAdmin = _configuration["AccountAdmin:Email"];
            passwork = _configuration["AccountAdmin:Passwork"];
        }
        [BindProperty]
        public IFormFile Upload { get; set; }
        [BindProperty]
        public string NameSersies { get; set; }
        [BindProperty]
        public WorkshopSeries WorkshopSeries { get; set; }
        [BindProperty]
        public List<Workshop>? WorkShopList { get; set; }
        [BindProperty]
        public List<WorkshopDTO>? OrtherWorkShopList { get; set; }
        [BindProperty]
        public int? SeriesWorkshopId { get; set; }
        [BindProperty]
        public SystemUser? Researcher { get; set; }
        [BindProperty]
        public WorkshopUpdateDTO? WorkShopUpdate { get; set; }
        [BindProperty]
        public string? Msg { get; set; }

        public void OnGet(int? seriesWorkshopId, int? idWorkshop, string? email, DateTime? date)
        {
            try
            {
                Msg = TempData["Msg"] as string;

                if (seriesWorkshopId != null)
                {
                    SeriesWorkshopId = seriesWorkshopId;
                    WorkshopSeries = _context.WorkshopSeries.FirstOrDefault(x => x.Id == seriesWorkshopId);
                    NameSersies = WorkshopSeries?.WorkshopSeriesName;
                    var researcherId = _repositoryAssign.GetResearchIdBySwsId((int)seriesWorkshopId);
                    Researcher = _repositoryUser.getUserById(researcherId);
                }
                if (idWorkshop != null && email != null && date != null)
                {
                    Presenter presenter = new Presenter
                    {
                        PresenterEmail = email,
                    };
                    if (_repositoryPresenter.InsertPresenter(presenter) > 0)
                    {
                        var findWs = _repositoryWorkshops.GetWorkshopByWorkshopId(idWorkshop);
                        if (findWs != null)
                        {
                            findWs.DatePresent = date;
                            findWs.StatusId = COTSEConstants.STATUS_PENDING;
                            findWs.PresenterId = presenter.PresenterId;
                            TeamplatePresenter teamplatePresenter = new TeamplatePresenter
                            {
                                WorkshopName = findWs.WorkshopName,
                                TimePresenter = findWs.DatePresent.ToString(),
                                KeyPresent = findWs.KeyPresenter,
                                UrlRoom = urlRoom,
                                LinkCF = linkCF,
                            };
                            var getBody = TeamplateMail.TeamplateMailPresenter(teamplatePresenter);
                            string subject = "Thư Mời Tham Dự Làm Diễn Gỉa Phòng Nghiên Cứu Lab318 FPT";
                            HelperMethods.SendMail(getBody, subject, emailAdmin, passwork, email);
                            _repositoryWorkshops.UpdateDatePresent(findWs);
                        }
                    }
                }
                //SeriesWorkshopId = 2;
                if (seriesWorkshopId > 0)
                {
                    WorkShopList = _repositoryWorkshops.GetWorkshopBySeriesWorkshopId(seriesWorkshopId);
                    OrtherWorkShopList = new List<WorkshopDTO>();
                    foreach (var item in WorkShopList)
                    {
                        WorkshopDTO workshop = new WorkshopDTO();
                        OrtherWorkShopList.Add(workshop);
                    }
                }
            }
            catch (Exception ex)
            {
                Msg = ex.Message;
            }
        }
        
        public IActionResult OnGetAsigntPresenter(int? id)
        {
            if (WorkShopUpdate != null)
            {
                var findWs = _repositoryWorkshops.GetWorkshopByWorkshopId(WorkShopUpdate.Id);
                var findPresenter = _repositoryPresenter.GetPresenterById((int)findWs.PresenterId);
                findPresenter.PresenterEmail = WorkShopUpdate.Email;
                findWs.DatePresent = WorkShopUpdate.DatePresent;
                var resultUpdateDate = _repositoryWorkshops.UpdateDatePresent(findWs);
                var resultUpdatePresenter = _repositoryPresenter.UpdatePresenter(findPresenter);
                if (resultUpdatePresenter > 0)
                {
                    TeamplatePresenter teamplatePresenter = new TeamplatePresenter
                    {
                        WorkshopName = findWs.WorkshopName,
                        TimePresenter = findWs.DatePresent.ToString(),
                        KeyPresent = findWs.KeyPresenter,
                        UrlRoom = urlRoom,
                        LinkCF = linkCF,
                    };
                    var getBody = TeamplateMail.TeamplateMailPresenter(teamplatePresenter);
                    string subject = "Thư Mời Tham Dự Làm Diễn Gỉa Phòng Nghiên Cứu Lab318 FPT";
                    HelperMethods.SendMail(getBody, subject, emailAdmin, passwork, WorkShopUpdate.Email);
                    return RedirectToPage("AddNewSeries", new { seriesWorkshopId = findWs.WorkshopSeriesId });
                }
            }
            if (OrtherWorkShopList.Count() > 0 && id > 0)
            {
                var findWsOrthher = OrtherWorkShopList.FirstOrDefault(x => x.Id == id);
                if (findWsOrthher != null)
                {
                    var findWs = _repositoryWorkshops.GetWorkshopByWorkshopId(findWsOrthher.Id);
                    findWs.DatePresent = findWsOrthher.DatePresent;
                    TeamplatePresenter teamplatePresenter = new TeamplatePresenter
                    {
                        WorkshopName = findWs.WorkshopName,
                        TimePresenter = findWs.DatePresent.ToString(),
                        KeyPresent = findWs.KeyPresenter,
                        UrlRoom = urlRoom,
                        LinkCF = linkCF,
                    };
                    var getBody = TeamplateMail.TeamplateMailPresenter(teamplatePresenter);
                    string subject = "Thư Mời Tham Dự Làm Diễn Gỉa Phòng Nghiên Cứu Lab318 FPT";
                    HelperMethods.SendMail(getBody, subject, emailAdmin, passwork, findWsOrthher.Email);
                    _repositoryWorkshops.UpdateDatePresent(findWs);
                }
                return RedirectToPage("AddNewSeries", new { seriesWorkshopId = findWsOrthher.WorkshopSeriesId});
            }
            return RedirectToPage("AddNewSeries", new { seriesWorkshopId = SeriesWorkshopId });
        }
        public IActionResult OnPostAsigntPresenter()
        {
            if (WorkShopUpdate != null)
            {
                var findWs = _repositoryWorkshops.GetWorkshopByWorkshopId(WorkShopUpdate.Id);
                var findPresenter = _repositoryPresenter.GetPresenterById((int)findWs.PresenterId);
                findPresenter.PresenterEmail = WorkShopUpdate.Email;
                findWs.DatePresent = WorkShopUpdate.DatePresent;
                var resultUpdateDate = _repositoryWorkshops.UpdateDatePresent(findWs);
                var resultUpdatePresenter = _repositoryPresenter.UpdatePresenter(findPresenter);
                if (resultUpdatePresenter > 0)
                {
                    TeamplatePresenter teamplatePresenter = new TeamplatePresenter
                    {
                        WorkshopName = findWs.WorkshopName,
                        TimePresenter = findWs.DatePresent.ToString(),
                        KeyPresent = findWs.KeyPresenter,
                        UrlRoom = urlRoom,
                        LinkCF = linkCF,
                    };
                    var getBody = TeamplateMail.TeamplateMailPresenter(teamplatePresenter);
                    string subject = "Thư Mời Tham Dự Làm Diễn Gỉa Phòng Nghiên Cứu Lab318 FPT";
                    HelperMethods.SendMail(getBody, subject, emailAdmin, passwork, WorkShopUpdate.Email);
                    Msg = "Update Prenter Success!!!";
                    return RedirectToPage("AddNewSeries", new { seriesWorkshopId = findWs.WorkshopSeriesId });
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
                                FavoriteTopics = rowData[5],
                                WorkshopSeriesId = SeriesWorkshopId
                            };
                            seriesWorkshopForm.Add(newSeriesWorkshopForm);
                        }
                    }
                }
                if (seriesWorkshopForm.Count() > 0)
                {
                    var listParticipants = seriesWorkshopForm.Select(x => new Participant
                    {
                        Email = x.Email,
                        TimeStamp = x.Timestamp,
                        FullName = x.NameParticipant,
                        FavoriteTopics = x.FavoriteTopics,
                        Major = x.Major,
                        WorkshopSeriesId= x.WorkshopSeriesId
                    }).ToList();
                    var resultInsert = _repositoryParticipants.InsertRange(listParticipants);
                    if (resultInsert > 0)
                    {
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
                                WorkshopSeriesId = SeriesWorkshopId,
                                KeyPresenter = HelperMethods.GenerateSecretKey(topicNames[i].Topic + SeriesWorkshopId, 6),
                                StatusId = COTSEConstants.STATUS_DEFAULT,
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
                            WorkShopList = _repositoryWorkshops.GetWorkshopBySeriesWorkshopId(SeriesWorkshopId).OrderByDescending(x => x.Index).ToList();

                            WorkshopSeries = _context.WorkshopSeries.FirstOrDefault(x => x.Id == SeriesWorkshopId);
                            var researcherId = _repositoryAssign.GetResearchIdBySwsId((int)SeriesWorkshopId);
                            Researcher = _repositoryUser.getUserById(researcherId);

                            StringBuilder workshopInfoBuilder = new StringBuilder();
                            List<WorkshopInformation> workshopInformations = new List<WorkshopInformation>();
                            foreach (var workshop in WorkShopList)
                            {
                                WorkshopInformation wsIf = new WorkshopInformation
                                {
                                    WorkshopName = workshop.WorkshopName,
                                    WorkshopKey = workshop.KeyPresenter,
                                };

                                workshopInformations.Add(wsIf);
                            }

                            TemplateMailResearcher templateMailResearcher = new TemplateMailResearcher
                            {
                                WorkshopSeriesName = WorkshopSeries.WorkshopSeriesName,
                                TimeStart = WorkshopSeries.StartDate.ToString(),
                                WorkshopInformation = workshopInformations,
                                UrlRoom = urlRoom,
                                UrlDownLoadTool = baseURL+ COTSEConstants.DOWNLOADTOOLS,
                                UrlWebLogin = baseURL + COTSEConstants.LOGIN
                            };
                            var bodyMail = TeamplateMail.TeamplateMailResearch(templateMailResearcher);
                            string subject = "Thư Mời Researcher";
                            HelperMethods.SendMail(bodyMail, subject, emailAdmin, passwork, Researcher?.Email);
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
