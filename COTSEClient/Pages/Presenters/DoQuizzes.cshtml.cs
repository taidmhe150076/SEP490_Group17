using BusinessLogic.IRepository;
using BusinessLogic.Repository;
using DataAccess.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace COTSEClient.Pages.Quizzes
{
    public class DoQuizzesModel : PageModel
    {
        private readonly IRepositoryWorkshopQuestions _repositoryWorkshopQuestions;
        private readonly IRepositoryParticipantAnswer _repositoryParticipantAnswer;
        private readonly IRepositoryParticiPantScore _repositoryParticiPantScore;
        public DoQuizzesModel(IRepositoryWorkshopQuestions repositoryWorkshopQuestions)
        {
            _repositoryWorkshopQuestions = repositoryWorkshopQuestions;
        }

        [BindProperty]
        public List<WorkshopQuestion> WorkshopQuestions { get; set; }
        [BindProperty]
        public List<AnswerParticipant> AnswerParticipantList { get; set; }
        [BindProperty]
        public string ParticipantEmail { get; set; }
        [BindProperty]
        public int TestCurrentId { get; set; }
        [BindProperty]
        public int WorkShopId { get; set; }
        public IActionResult OnGet(int testId = 1, int workShopId = 3)
        {
            TestCurrentId = testId;
            WorkshopQuestions = _repositoryWorkshopQuestions.GetWorkshopQuestionsByWsId(workShopId);
            WorkShopId = workShopId;
            AnswerParticipantList = new List<AnswerParticipant>();
            foreach (var question in WorkshopQuestions)
            {
                AnswerParticipantList.Add(new AnswerParticipant()
                {
                    QuestionId = question.Id,
                    TestId = testId
                });
            }
            return Page();
        }

        public IActionResult OnPost()
        {
            try
            {
                float score = 0;
                float sountScore = 0;
                WorkshopQuestions = _repositoryWorkshopQuestions.GetWorkshopQuestionsByWsId(WorkShopId);

                foreach (var item in WorkshopQuestions)
                {
                    foreach (var answer in AnswerParticipantList)
                    {
                        if (item.Id == answer.QuestionId)
                        {
                            var isCorrectAnswer = item.AnswerQuestions.FirstOrDefault(x => x.IsCorrectAnswer == true);
                            if (isCorrectAnswer != null && !string.IsNullOrEmpty(answer.Answer) && answer.Answer.Equals(isCorrectAnswer.AnswerText))
                            {
                                sountScore++;
                            }
                        }
                    }
                }
                score = ((WorkshopQuestions.Count() + 1) / 10) * sountScore;

                ParticipantAnswer participantAnswer = new ParticipantAnswer
                {
                    ParticipantsEmail = ParticipantEmail,
                };
                var resultInsertParticipantAnswer = _repositoryParticipantAnswer.InsertParticipantAnswer(participantAnswer);
                if (resultInsertParticipantAnswer > 0)
                {
                    ParticiPantScore newParticiPantScore = new ParticiPantScore
                    {
                        Score = score,
                        ParticipantId = participantAnswer.Id,
                        TestId = TestCurrentId,
                    };
                    _repositoryParticiPantScore.InsertParticiPantScore(newParticiPantScore);
                }
                return Page();
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }
    }
}
