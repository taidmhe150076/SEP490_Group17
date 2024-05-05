using DataAccess.DTO;
using DataAccess.Models;

namespace BusinessLogic.IRepository
{
    public interface IRepositorySurvey
    {
        // wss
        //Task<List<WorkshopSurveyDTO>> seriesSurvey();
        //List<SurveyDTO> getListSurvey(int wssId, int wsId);
        //Task<string> getWorkshopData(int wssId, int wsId, string key);
        //WorkshopSurveyUrl getSurveyByWorkshop(string workshop_series_id, string workshop_id);

        // survey
        WorkshopInfoDTO getWorkshopInformation(int wssId, int wsId);

        WorkshopInfoDTO getCurrentWorkshopInformation(int wssId, int wsId, int surveyId);
        Task<int> createNewSurveyFile(WorkshopInfoDTO workshopInfo);
        Task<int> createNewSurvey(WorkshopInfoDTO workshopInfo);
        Task<SurveyDTO> getSurey(int surveyId);
        Task<WorkshopSurveyUrl> findSurvey(int wssId, int wsId, int surveyId);
        Task<List<CommonQA>> getOtherData(int surveyId);
        Task<int> updateSurvey(WorkshopInfoDTO workshopInformation);
        // return file path
        Task<List<WorkshopSurveyDTO>> surveyList(List<Assign> assignList);

        //analyze
        Task<List<FeedbackResult>> getSurveySentimentResult(int surveyId);
        Task<Dictionary<string, int>> CountFeedback(List<SentimentAnswerResult> survey_sentiment);
        Task<List<SentimentAnswerResult>> getSentimentList(int surveyId);

        Task<bool> deleteSurvey(int wssId, int wsId, int surveyId);

        public int GetWorkshopSurveyUrlIdOfParticipants(int wsId);
    }
}
