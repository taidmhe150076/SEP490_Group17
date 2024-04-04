using DataAccess.DTO;
using DataAccess.Models;

namespace BusinessLogic.IRepository
{
    public interface IRepositorySurvey
    {
        // wss
        Task<List<WorkshopSeriesWorkshop>> seriesSurvey();
        List<SurveyDTO> getListSurvey(int wssId, int wsId);
        WorkshopInfoDTO getWorkshopInformation(int wssId, int wsId);
        Task<int> addSurvey(WorkshopInfoDTO workshopinfo, int mode, string? filePath);
        
        // survey
        WorkshopSurveyUrl getSurveyByWorkshop(string workshop_series_id, string workshop_id);
        Task<string> getWorkshopData(int wssId, int wsId, string key);

        //analys
        List<string> GetSentimentAnswer(string file_path);
        public List<FeedbackResult> Rate(List<string> questions, string json_data);
        Task<string> GetJsonSentiment(List<string> sentiment_data_list);
    }
}
