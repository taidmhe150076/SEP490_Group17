using DataAccess.DTO;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.IRepository
{
    public interface IRepositorySurvey
    {
        List<string> GetSentimentAnswer(string file_path);
        bool validateFileName(string filePath);
        public List<FeedbackResult> Rate(List<string> questions, string json_data);
    }
}
