using BusinessLogic.IRepository;
using BusinessLogic.Repository;
using COTSEClient.DTO;
using COTSEClient.Helper;
using DataAccess.Constants;
using DataAccess.DTO;
using DataAccess.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using OfficeOpenXml;

namespace COTSEClient.Pages.Presenters
{
    public class PresenterWorkshopModel : PageModel
    {
        private readonly IConfiguration _configuration;
        private string? baseUrl;
        private readonly IRepositoryWorkshops _repositoryWorkshops;
        private readonly IRepositoryTests _repositoryTests;
        private readonly IRepositoryParticipants _repositoryParticipants;
        private readonly IRepositoryTestType _repositoryTestType;
        private readonly IRepositoryWorkshopQuestions _repositoryWorkshopQuestions;
        private readonly IRepositoryAnswerQuestion _repositoryAnswerQuestion;
        public PresenterWorkshopModel(IRepositoryWorkshops repositoryWorkshops, IRepositoryTests repositoryTests, IRepositoryParticipants repositoryParticipants, IRepositoryTestType repositoryTestType , IRepositoryWorkshopQuestions repositoryWorkshopQuestions, IRepositoryAnswerQuestion repositoryAnswerQuestion, IConfiguration configuration)
        {
            _configuration = configuration;
            baseUrl = _configuration["BaseURL"];
            _repositoryWorkshops = repositoryWorkshops;
            _repositoryTests = repositoryTests;
            _repositoryParticipants = repositoryParticipants;
            _repositoryTestType = repositoryTestType;
            _repositoryWorkshopQuestions = repositoryWorkshopQuestions;
            _repositoryAnswerQuestion = repositoryAnswerQuestion;
        }
        static string? invitationCode;
        [BindProperty]
        public IFormFile Upload { get; set; }
        [BindProperty]
        public int? WorkshopId { get; set; }
        public string? WorkshopName { get; set; }
        [BindProperty]
        public List<Test> TestList { get; set; }
        [BindProperty]
        public string? QRTest { get; set; }
        [BindProperty]
        public List<TestType> TypeTest { get; set; }
        [BindProperty]
        public int? TypeTestId { get; set; }
        [BindProperty]
        public Boolean? QuestionExits { get; set; } = false;

        public IActionResult OnGet(string key)
        {
            try
            {
                if (string.IsNullOrEmpty(key))
                {
                    return RedirectToPage("EnterKey");
                }
                invitationCode = key;
                TypeTest = _repositoryTestType.GetTypes();
                var findWorkshopByKey = _repositoryWorkshops.GetWorkshopByKeyPresenter(key);
                if (_repositoryWorkshopQuestions.GetWorkshopQuestionsByWsId(findWorkshopByKey.Id).Count() > 0)
                {
                    QuestionExits = true;
                } ;

                WorkshopId = findWorkshopByKey?.Id;
                WorkshopName = findWorkshopByKey?.WorkshopName;
                TestList = _repositoryTests.GetTestByWorkshopId(WorkshopId);
                return Page();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        public IActionResult OnPostCreateTest(string testName, DateTime expiredTime)
        {
            try
            {
                var newTest = new Test()
                {
                    TestName = testName,
                    TestTypeId = TypeTestId,
                    ExpiredTime = expiredTime,
                    DateStart = DateTime.Now,
                    WorkshopId = WorkshopId
                };
                var result = _repositoryTests.InsertTest(newTest);
                if (result > 0)
                {
                    string url = baseUrl + "Presenters/DoQuizzes" + "?testId=" + newTest.Id + "&workShopId=" + WorkshopId;
                    var genQR = Helper.HelperMethods.GenerateQRCode(url);
                    newTest.QrTest = genQR;
                    var resultUpdate = _repositoryTests.UpdateTest(newTest);
                    return RedirectToPage("PresenterWorkshop", new { key = invitationCode });
                }
                return BadRequest("Can't insert Test");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        public IActionResult OnPostCreateQuestions()
        {
            try
            {
                if (Upload != null && Upload.Length > 0)
                {
                    List<TemplateQuestionDTO> templateQuestionDTOs = new List<TemplateQuestionDTO>();
                    using (var stream = new MemoryStream())
                    {
                        Upload.CopyTo(stream);
                        ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
                        using (var package = new ExcelPackage(stream))
                        {
                            ExcelWorksheet worksheet = package.Workbook.Worksheets[0];
                            int rowCount = worksheet.Dimension.End.Row;

                            var headers = worksheet.Cells["A1:Z1"].Select(cell => cell.Value?.ToString().Trim()).ToArray();

                            for (int row = 2; row <= rowCount; row++)
                            {
                                var rowData = worksheet.Cells[row, 1, row, headers.Length].Select(cell => cell.Value?.ToString()).ToArray();
                                TemplateQuestionDTO templateQuestionDTO = new TemplateQuestionDTO
                                {
                                    QuestionText = rowData[0],
                                    Answer_text = rowData[1],
                                    Is_correct_answer = rowData[2],
                                };
                                templateQuestionDTOs.Add(templateQuestionDTO);
                            }
                        }
                        var questionTexts = templateQuestionDTOs.DistinctBy(x => x.QuestionText).ToList();
                        foreach (var question in questionTexts)
                        {
                            var anwsers = templateQuestionDTOs.Where(x => x.QuestionText == question.QuestionText).ToList();
                            if (anwsers.Count() > 0)
                            {
                                WorkshopQuestion workshopQuestion = new WorkshopQuestion
                                {
                                    WorkshopId = WorkshopId,
                                    QuestionText = question.QuestionText,
                                };
                                var resultInsert = _repositoryWorkshopQuestions.InsertQuestion(workshopQuestion);
                                if (resultInsert > 0)
                                {
                                    List<AnswerQuestion> answerQuestionsList = new List<AnswerQuestion>();

                                    for (int i = 0; i < anwsers.Count; i++)
                                    {
                                        AnswerQuestion answerQuestion = new AnswerQuestion()
                                        {
                                            QuestionId = workshopQuestion.Id,
                                            AnswerText = anwsers[i].Answer_text,
                                            IsCorrectAnswer = anwsers[i].Is_correct_answer == COTSEConstants.CORRECT_ANSWER ? true : false,
                                        };
                                        answerQuestionsList.Add(answerQuestion);
                                    }
                                    var resultInsertAnswers = _repositoryAnswerQuestion.InsertRangeQuestionAnwser(answerQuestionsList);
                                }
                            }
                        }
                    }
                    var findKeyWs = _repositoryWorkshops.GetWorkshopByWorkshopId(WorkshopId).KeyPresenter;
                    return RedirectToPage("PresenterWorkshop", new { key = findKeyWs });
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
