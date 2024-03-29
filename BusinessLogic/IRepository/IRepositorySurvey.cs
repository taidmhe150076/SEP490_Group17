using DataAccess.DTO;
using DataAccess.Models;

namespace BusinessLogic.IRepository
{
    public interface IRepositorySurvey
    {
        SurveyUrl getSurveyByWorkshop(string workshop_series_id, string workshop_id);
        List<string> GetSentimentAnswer(string file_path);
        bool validateFileName(string filePath);
        public List<FeedbackResult> Rate(List<string> questions, string json_data);
        List<(string, int)> validateFilesName(List<string> filesName);
        Task GoogleSheetApi();
        Task<string> GetJsonSentiment(List<string> sentiment_data_list);
    }
}
