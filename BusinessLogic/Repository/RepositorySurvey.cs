using BusinessLogic.IRepository;
using DataAccess.DTO;
using DataAccess.Models;
using Microsoft.IdentityModel.Protocols;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace BusinessLogic.Repository
{
    public class RepositorySurvey : IRepositorySurvey
    {
        private List<string> file_types = new List<string>() {
            "csv",
            "xlsx",
        };

        private string HUGGING_FACE_TOKEN = "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";
        private string API_URL = "https://api-inference.huggingface.co/models/wonrax/phobert-base-vietnamese-sentiment";


        public List<string> GetSentimentAnswer(string file_path)
        {
            List<SurveyAnswer> survey = getPaticipantFeedback(file_path);
            var unchecked_answer = new List<string>();
            foreach (var answer in survey)
            {
                var sentiment_string = answer.QA.Last().Value;
                unchecked_answer.Add(sentiment_string);
            }
            return unchecked_answer;
        }

        public List<FeedbackResult> Rate(List<string> question_list, string json_string)
        {
            var sentiment_result = new List<FeedbackResult>();
            double? NEU = null;
            double? NEG = null;
            double? POS = null;
            List<List<Dictionary<string, string>>>? json_data =
                JsonConvert.DeserializeObject<List<List<Dictionary<string, string>>>>(json_string);
            for (int i = 0; i < json_data.Count; i++)
            {
                for (int j = 0; j < json_data[i].Count; j++)
                {
                    //get 3 dictionary with value pos and value message
                    // get key : contain POS, NEU, NEG
                    var key = json_data[i][j]["label"];
                    // get value: contain the score of key
                    var value = json_data[i][j]["score"];
                    switch (key)
                    {
                        case "NEG":
                            NEG = double.Parse(value);
                            break;
                        case "NEU":
                            NEU = double.Parse(value);
                            break;
                        case "POS":
                            POS = double.Parse(value);
                            break;
                        default:
                            throw new Exception("value error");
                    }
                }
                if (NEG != null && NEU != null && POS != null)
                {
                    var result = new FeedbackResult()
                    {
                        Question = question_list[i],
                        NEG = (double)NEG,
                        POS = (double)POS,
                        NEU = (double)NEU
                    };
                    sentiment_result.Add(result);
                }
            }

            return sentiment_result;
        }
        
        public bool validateFileName(string filePath)
        {
            string file_type = filePath.Split(".")[^1];
            string file_name = filePath.Split(".")[0];
            if (!file_types.Contains(file_type))
            {
                return false;
            }
            var file_name_part = file_name.Split("_");
            //file name must contain the workshop name file name must contain workshop date
            return true;
        }

        private Exception DataInvalidException()
        {
            throw new Exception("Invalid file path");
        }

        private List<SurveyAnswer> getPaticipantFeedback(string file_path)
        {
            if (!File.Exists(file_path))
            {
                throw DataInvalidException();
            }
            switch (file_path.Split(".")[^1])
            {
                case "xlsx":
                    return ReadExcelFile(file_path);
                case "csv":
                    return ReadCSVFile(file_path);
                default:
                    throw DataInvalidException();
            }
        }
        private List<SurveyAnswer> ReadCSVFile(string file_path)
        {
            List<SurveyAnswer> survey_answer = new List<SurveyAnswer>();
            using (var reader = new StreamReader(file_path, Encoding.UTF8))
            {
                var current_line = 0;
                string? name = null;
                DateTime? timestamp = null;
                var headers = new List<string>();
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    var line_split = line.Split(",");
                    //add header
                    if (current_line == 0)
                    {
                        foreach (var header in line_split.Skip(2).ToList())
                        {
                            headers.Add(header);
                        }
                    }
                    else
                    {
                        var qa = new Dictionary<string, string>();
                        timestamp = DateTime.Parse(line_split[0]);
                        name = line_split[1];
                        var list_answer = line_split.Skip(2).ToList();
                        for (int i = 0; i < headers.Count; i++)
                        {
                            qa[headers[i]] = list_answer[i];
                        }
                        SurveyAnswer answer = new SurveyAnswer()
                        {
                            timeStamp = timestamp,
                            AnswerBy = name,
                            QA = qa,
                        };
                        survey_answer.Add(answer);
                    }
                    //next line
                    current_line++;
                }
            }
            return survey_answer;
        }

        private List<SurveyAnswer> ReadExcelFile(string file_path)
        {
            throw new NotImplementedException();
        }
    }
}
