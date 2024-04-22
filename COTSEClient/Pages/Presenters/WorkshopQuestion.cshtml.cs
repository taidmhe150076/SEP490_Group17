using BusinessLogic.IRepository;
using DataAccess.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace COTSEClient.Pages.Presenters
{
    public class WorkshopQuestionModel : PageModel
    {
        private readonly Sep490G17DbContext _context;
        private readonly IRepositoryWorkshopQuestions _repository;

        public WorkshopQuestionModel(Sep490G17DbContext context, IRepositoryWorkshopQuestions repository)
        {
            _context = context;
            _repository = repository;
        }

        [BindProperty]
        public List<WorkshopQuestion> listWorkshopQuestion { get; set; }
        [BindProperty]
        public WorkshopQuestion WorkshopQuestion { get; set; }
        [BindProperty]
        public string QuestionText {  get; set; }
        [BindProperty]
        public string AnswerText { get; set; }
        [BindProperty]
        public string Msg { get; set; }
        [BindProperty]
        public int WorkshopId { get; set; }
        [BindProperty]
        public WorkshopQuestion question { get; set; }
        [BindProperty]
        public AnswerQuestion answer {  get; set; }
        public IActionResult OnGet(int workshopId)
        {
            Msg = TempData["Msg"] as string;
            WorkshopId = workshopId;
            
            listWorkshopQuestion = _repository.GetWorkshopQuestionsByWsId(workshopId);
            
            return Page();
        }

        public IActionResult OnPostUpdateQuestion(int workshopId)
        {
            try
            {
                var question = _repository.GetWorkshopQuestionsByWsId(workshopId);
                if (question != null)
                {
                    TempData["Msg"] = "Not found question";
                }
                return RedirectToPage();
            }
            catch (Exception ex)
            {
                throw;
            }
           
        }

        public IActionResult OnPost(int id)
        {
            try
            {
                _repository.DeleteQuestion(id);
                TempData["Msg"] = "Delete Success";
                return RedirectToPage("WorkshopQuestion", new { workshopId = WorkshopId });
            }
            catch(Exception ex)
            {
                throw;
            }
           
        }

        public IActionResult OnPostInsertQuestion()
        {         
            try
            {
                if (!AnswerText.Contains('\r') || !AnswerText.Contains(':') || !AnswerText.Contains('0') || !AnswerText.Contains('1'))
                {
                    TempData["Msg"] = "Not correct format Answer Text";
                    return RedirectToPage();
                }


                if (!string.IsNullOrEmpty(QuestionText) && !string.IsNullOrEmpty(AnswerText))
                {
                    var newQuestion = new WorkshopQuestion
                    {
                        WorkshopId = WorkshopId,
                        QuestionText = QuestionText,
                    };

                    _context.WorkshopQuestions.Add(newQuestion);
                    _context.SaveChanges();

                    var newAnswers = AnswerText.Split('\r');
                    foreach (var answer in newAnswers)
                    {
                        var newAnswer = new AnswerQuestion
                        {
                            QuestionId = newQuestion.Id,
                            AnswerText = answer.Trim().Split(":")[0].Trim(),
                            IsCorrectAnswer = answer.Trim().Split(':')[1].Trim() == "1",
                        };

                        _context.AnswerQuestions.Add(newAnswer);

                    }
                    _context.SaveChanges();
                    TempData["Msg"] = "Add success";
                    return RedirectToPage("WorkshopQuestion", new { workshopId = WorkshopId });
                }

            }catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }



            return Page();
            
        }
    }
}
