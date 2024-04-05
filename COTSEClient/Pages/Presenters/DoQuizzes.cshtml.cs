using BusinessLogic.IRepository;
using BusinessLogic.Repository;
using COTSEClient.Hubs;
using DataAccess.DTO;
using DataAccess.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.SignalR;
using System.Text.Json;

namespace COTSEClient.Pages.Quizzes
{
    public class DoQuizzesModel : PageModel
    {
        private readonly IRepositoryWorkshopQuestions _repositoryWorkshopQuestions;
        private readonly IRepositoryParticipantAnswer _repositoryParticipantAnswer;
        private readonly IRepositoryParticiPantScore _repositoryParticiPantScore;
        private readonly IRepositoryAnswerParticipants _repositoryAnswerParticipants;
        private readonly IHubContext<ParticiPantScoresHub> _hubContext;


        public DoQuizzesModel(IRepositoryWorkshopQuestions repositoryWorkshopQuestions, IRepositoryParticipantAnswer repositoryParticipantAnswer, IRepositoryParticiPantScore repositoryParticiPantScore, IHubContext<ParticiPantScoresHub> hubContext, IRepositoryAnswerParticipants repositoryAnswerParticipants)
        {
            _repositoryWorkshopQuestions = repositoryWorkshopQuestions;
            _repositoryParticipantAnswer = repositoryParticipantAnswer;
            _repositoryParticiPantScore = repositoryParticiPantScore;
            _repositoryAnswerParticipants = repositoryAnswerParticipants;
            _hubContext = hubContext;
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
        public IActionResult OnGet(int testId, int workShopId)
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

        public async Task<IActionResult> OnPost()
        {
            try
            {
                double score = 0;
                int countScore = 0;
                WorkshopQuestions = _repositoryWorkshopQuestions.GetWorkshopQuestionsByWsId(WorkShopId);

                foreach (var item in WorkshopQuestions)
                {
                    foreach (var answer in AnswerParticipantList)
                    {
                        if (item.Id == answer.QuestionId)
                        {
                            var isCorrectAnswer = item.AnswerQuestions.FirstOrDefault(x => x.IsCorrectAnswer == true);
                            if (isCorrectAnswer != null && answer.AnswerId != 0 && answer.AnswerId == isCorrectAnswer.Id)
                            {
                                countScore++;
                            }
                        }
                    }
                }
                score = (countScore / (double)WorkshopQuestions.Count()) * 10;
                score = (double)Math.Round(score, 2);

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
                    var resultInsertScore = _repositoryParticiPantScore.InsertParticiPantScore(newParticiPantScore);

                    //var resultList = AnswerParticipantList.Select(x => new AnswerParticipant()
                    //{
                    //    QuestionId = x.QuestionId,
                    //    TestId = TestCurrentId,
                    //    AnswerId = x.AnswerId != 0 ? x.AnswerId : 0,
                    //    ParticipantId = participantAnswer.Id,
                    //    SubmissionTime = DateTime.Now,
                    //}).ToList();

                    //_repositoryAnswerParticipants.InsertRangeAnswerOfParticipants(resultList);
                    if (resultInsertScore > 0)
                    {
                        List<ParticiPantScore> ParticiPantScore = _repositoryParticiPantScore.GetParticiPantScoreByTestId(newParticiPantScore.TestId);
                        var result = ParticiPantScore.Select(x => new ParticiPantScoreDTO
                        {
                            TestName = x.Test.TestName,
                            ParticipantName = x.Participant?.ParticipantsEmail,
                            Score = x.Score
                        }).OrderByDescending(x => x.Score).ToList();
                        var resultjson = JsonSerializer.Serialize<List<ParticiPantScoreDTO>>(result);
                        await _hubContext.Clients.All.SendAsync("Message", resultjson);
                    }
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
