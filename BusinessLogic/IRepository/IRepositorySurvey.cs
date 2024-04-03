using DataAccess.DTO;
using DataAccess.Models;

namespace BusinessLogic.IRepository
{
    public interface IRepositorySurvey
    {
        //temp
        int getTotalTempFile();
        // wss
        Task<List<WorkshopSeriesWorkshop>> seriesSurvey();
        List<SurveyDTO> getListSurvey(int wss_id, int ws_id);
        WorkshopInfoDTO getWorkshopInformation(int wss_id, int ws_id);
        Task<int> addSurvey(WorkshopInfoDTO workshopinfo, int mode);
        
        // survey
        WorkshopSurveyUrl getSurveyByWorkshop(string workshop_series_id, string workshop_id);
        

        //analyse
        List<string> GetSentimentAnswer(string file_path);
        public List<FeedbackResult> Rate(List<string> questions, string json_data);
        Task<string> GetJsonSentiment(List<string> sentiment_data_list);
    }
}
