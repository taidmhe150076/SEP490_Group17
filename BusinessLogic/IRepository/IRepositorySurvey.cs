using DataAccess.DTO;
using DataAccess.Models;

namespace BusinessLogic.IRepository
{
    public interface IRepositorySurvey
    {
        // wss
        Task<List<WorkshopSeriesWorkshopDTO>> seriesSurvey();
        List<SurveyDTO> getListSurvey(int wssId, int wsId);
        WorkshopInfoDTO getWorkshopInformation(int wssId, int wsId);
        
        // survey
        WorkshopSurveyUrl getSurveyByWorkshop(string workshop_series_id, string workshop_id);
        Task<string> getWorkshopData(int wssId, int wsId, string key);
        Task<int> createNewSurveyFile(WorkshopInfoDTO workshopInfo);
        Task<int> createNewSurveyUrl(WorkshopInfoDTO ws_info);
        Task<SurveyDTO> getSurey(int surveyId);
        Task<List<CommonQA>> getOtherData(int surveyId);
        // return file path


        //analyze
        Task<List<FeedbackResult>> getSurveySentimentResult(int surveyId);
        Task<Dictionary<string, int>> CountFeedback(List<FeedbackResult> survey_sentiment);
    }
}
