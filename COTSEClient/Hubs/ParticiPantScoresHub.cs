using BusinessLogic.IRepository;
using DataAccess.DTO;
using DataAccess.Models;
using Microsoft.AspNetCore.SignalR;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace COTSEClient.Hubs
{
    public class ParticiPantScoresHub : Hub
    {
        //private readonly IRepositoryParticiPantScore _repositoryParticiPantScore;

        //public ParticiPantScoresHub(IRepositoryParticiPantScore repositoryParticiPantScore)
        //{
        //    _repositoryParticiPantScore = repositoryParticiPantScore;
        //}
        //public async Task UpdateParticiPantScoresAndSendToClient(int testId)
        //{
        //    try
        //    {
        //        List<ParticiPantScore> ParticiPantScore = _repositoryParticiPantScore.GetParticiPantScoreByTestId(testId);
        //        var result = ParticiPantScore.Select(x => new ParticiPantScoreDTO
        //        {
        //            TestName = x.Test.TestName,
        //            ParticipantName = x.Participant?.ParticipantsEmail,
        //            Score = x.Score
        //        }).ToList();
        //        var resultjson = JsonSerializer.Serialize<List<ParticiPantScoreDTO>>(result);
        //        await Clients.All.SendAsync("Message", resultjson);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}
    }
}
