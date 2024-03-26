using BusinessLogic.IRepository;
using DataAccess.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace COTSEClient.Pages.Presenters
{
    public class PresenterWorkshopModel : PageModel
    {
        public readonly IRepositoryWorkshops _repositoryWorkshops;
        public readonly IRepositoryTests _repositoryTests;
        public readonly IRepositoryParticipants _repositoryParticipants;
        public PresenterWorkshopModel(IRepositoryWorkshops repositoryWorkshops, IRepositoryTests repositoryTests, IRepositoryParticipants repositoryParticipants)
        {
            _repositoryWorkshops = repositoryWorkshops;
            _repositoryTests = repositoryTests;
            _repositoryParticipants = repositoryParticipants;
        }

        [BindProperty]
        public int? WorkshopId { get; set; }
        public string? WorkshopName { get; set; }
        [BindProperty]
        public List<Test> TestList { get; set; }
        [BindProperty]
        public string? QRTest { get; set; }
        public IActionResult OnGet(string key)
        {
            try
            {
                if (string.IsNullOrEmpty(key))
                {
                    return RedirectToPage("EnterKey");
                }
                var findWorkshopByKey = _repositoryWorkshops.GetWorkshopByKeyPresenter(key);
                WorkshopId = findWorkshopByKey?.Id;
                WorkshopName = findWorkshopByKey?.WorkshopName;
                TestList = _repositoryTests.GetTestByWorkshopId(WorkshopId);
                return Page();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public IActionResult OnPostCreateTest(string testName, DateTime expiredTime)
        {
            try
            {
                var newTest = new Test()
                {
                    TestName = testName,
                    ExpiredTime = expiredTime,
                    DateStart = DateTime.Now,
                    WorkshopId = WorkshopId
                };
                var result = _repositoryTests.InsertTest(newTest);
                if (result > 0)
                {
                    string url = "https://localhost:7149/workshopTest/" + "QuestionTest?testId=" + newTest.Id + "&&workShopId=" + WorkshopId;
                    QRTest = Helper.HelperMethods.GenerateQRCode(url);
                }
                return Page();
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
