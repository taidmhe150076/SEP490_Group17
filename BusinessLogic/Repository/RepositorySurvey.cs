using BusinessLogic.IRepository;
using COTSEClient.DTO;
using DataAccess.Common;
using DataAccess.Constants;
using DataAccess.DTO;
using DataAccess.Models;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Sheets.v4;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Newtonsoft.Json;
using System.Collections;
using System.Text;
using System.Text.RegularExpressions;

namespace BusinessLogic.Repository
{
    public class RepositorySurvey : IRepositorySurvey
    {


        // Repo constant
        private readonly Sep490G17DbContext _context;
        private List<string> file_types = new List<string>() {
            "csv",
            "xlsx",
        };

        private Dictionary<string, string> QAbyUser = new Dictionary<string, string>();
        private int _saveMode = 0;
        private readonly string googleClientId = Environment.GetEnvironmentVariable("googleClientId");
        private readonly string googleClientSecret = Environment.GetEnvironmentVariable("googleClientSecret");
        private readonly string huggingFaceToken = Environment.GetEnvironmentVariable("huggingfaceToken");


        public string HuggingFaceToken => huggingFaceToken;

        public string GoogleClientId => googleClientId;

        public string GoogleClientSecret => googleClientSecret;


        //constructor
        public RepositorySurvey(Sep490G17DbContext context)
        {
            _context = context;//
        }

        //get survey in workshop series
        public List<WorkshopSeriesWorkshop> seriesSurvey()
        {
            var result = new List<WorkshopSeriesWorkshop>();
            var seriesContainSurvey = _context.WorkshopSeries
                .Join(_context.WorkshopSurveyUrls,
                series => series.Id,
                survey => survey.WorkshopSeriesId,
                (series, survey) => series)
                .Distinct().ToList();
            foreach (var series in seriesContainSurvey) {
                var workshop_list = _context.Workshops
                    .Where(ws =>ws.WorkshopSeriesId==series.Id)
                    .Join(_context.WorkshopSurveyUrls, ws => ws.Id, survey => survey.WorkshopId, (ws, survey)=> ws)
                    .Select(s => new WorkshopDTO {
                         Id = s.Id,
                         DatePresent = s.DatePresent,
                         WorkshopName = s.WorkshopName,
                         KeyPresenter = s.KeyPresenter != null ? s.KeyPresenter.Replace(s.KeyPresenter, new string('*', s.KeyPresenter.Count())): "",
                    }).ToList();
                var wss = new WorkshopSeriesWorkshop {
                    Id = series.Id,
                    WorkshopSeriesName = series.WorkshopSeriesName,
                    StartDate = series.StartDate,
                    workshops = workshop_list
                };
                result.Add(wss);
            }
            return result;
        }

        //add file to temp folder (dev)
        public string GetsaveFileToTemp(string fileName, int? saveMode)
        {
            string error_message = string.Empty;
            if (saveMode == null)
            {
                saveMode = _saveMode;
            }

            var file_extension = Path.GetExtension(fileName);

            if (!file_types.Contains(file_extension)) {
                error_message = SurveyErrorMessage.ERR_FILES_LOAD.Replace("{file_type}", file_extension);
                throw new NotSupportedException(error_message);
            }

            // save to temp folder in project
            string save_dir = String.Empty;
            if (saveMode == 0)
            {
                save_dir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "temp", fileName);
            }
            // save to temp folder in computer
            else if (saveMode == 1)
            {
                save_dir = Path.Combine(Path.GetTempPath(), "Costse_web_client", fileName);
            }
            else
            {
                throw new ArgumentOutOfRangeException(SurveyErrorMessage.ERR_OUT_OF_RANGE);
            }

            return save_dir;
        }

        public void getSurvey(string wss_id) {
              
        }
        // add workshopseries to workshop
        public int addSurvey(string wss_id, string ws_id, string url)
        {
            try
            {
                var wss = _context.WorkshopSeries.SingleOrDefault(wss_detail => wss_detail.Id == int.Parse(wss_id));
                if (wss == null)
                {
                    throw new ArgumentException(SurveyErrorMessage.ERR_WSS_NOT_FOUND);
                }
                var ws = _context.Workshops.SingleOrDefault(ws_detail => ws_detail.Id == int.Parse(ws_id) && ws_detail.WorkshopSeries == wss);
                if (ws == null)
                {
                    throw new ArgumentException(SurveyErrorMessage.ERR_WS_NOT_FOUND);
                }

                //check if the folder path already exist
                var survey = _context.WorkshopSurveyUrls.SingleOrDefault(s_detail => s_detail.SurveyKey == url);
                if (survey == null)
                {
                    var survey_url = new WorkshopSurveyUrl()
                    {
                        Workshop = ws,
                        WorkshopSeries = wss,
                        SurveyKey = url,
                    };
                    _context.WorkshopSurveyUrls.Add(survey_url);
                    var state = _context.SaveChanges();
                    if (state == 0)
                    {
                        throw new Exception(SurveyErrorMessage.ERR_ADD_WS);
                    }
                    return state;
                }
                else
                {
                    return 1;
                }
            }
            catch (Exception)
            {
                throw new Exception();
            }
        }

        public List<SurveyUrl> listSurvey(string wss_id, string ws_id)
        {
            return null;
        }
        // return a list of file name with mode of correction
        public List<(string, int)> validateFilesName(List<string> filesName)
        {
            var output = new List<(string, int)>();
            foreach (string file_name in filesName)
            {
                int valid_point = validate_file_name(file_name);
                output.Add((file_name, valid_point));
            }
            return output;
        }

        private int validate_file_name(string file)
        {
            var file_part = file.Split('.');
            if (!file_types.Contains(file_part[^1]))
            {
                return SurveyConstant.INVALID_FILE_EXE;
            }
            else
            {
                var file_name = file_part[0];
                var file_name_part = file_name.Split("_");
                // workshopSeriesName_workshopSeriesName
                string series_name = file_name_part[0];
                var valid_wss = _context.WorkshopSeries.SingleOrDefault(wss => wss.WorkshopSeriesName == series_name);
                if (valid_wss == null)
                {
                    return SurveyConstant.INVALID_WORKSHOP_SERIES;
                }
                string workshop_name = file_name_part[1];
                var valid_ws = _context.Workshops.Where(ws => ws.WorkshopSeriesId == valid_wss.Id)
                    .AsEnumerable()
                    .Where(ws => replace_special_character(ws.WorkshopName) == workshop_name).FirstOrDefault();
                if (valid_ws == null)
                {
                    return SurveyConstant.INVALID_WORKSHOP;
                }
            }
            return SurveyConstant.VALID_NAME_FORMAT;
        }


        // get single survey by workshop name
        public WorkshopSurveyUrl getSurveyByWorkshop(string workshop_series_id, string workshop_id)
        {
            var survey = _context.WorkshopSurveyUrls.SingleOrDefault(s => s.WorkshopSeriesId == int.Parse(workshop_series_id) && s.WorkshopId == int.Parse(workshop_id));
            if (survey == null)
            {
                throw new Exception(SurveyErrorMessage.ERR_WS_URL);
            }
            return survey;
        }

        //get goole api data
        public Task GoogleSheetApi()
        {
            Login();
            return null;
        }

        // get hugging face api
        public async Task<string> GetJsonSentiment(List<string> sentiment_data_list)
        {
            var json_input = JsonConvert.SerializeObject(sentiment_data_list);
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {HuggingFaceToken} ");
                client.DefaultRequestHeaders.Add("Accept", "application/json");
                var json_content = new StringContent(json_input, Encoding.UTF8, "application/json");
                HttpResponseMessage resp = await client.PostAsync(SurveyConstant.HUGGINGFACE_API_URL, json_content);
                if (resp.IsSuccessStatusCode)
                {
                    string json_output = await resp.Content.ReadAsStringAsync();
                    return json_output;
                }
                else
                {
                    throw new Exception(SurveyErrorMessage.ERR_HF_API);
                }
            }
            throw new NotImplementedException();
        }


        public List<string> GetSentimentAnswer(string file_path)
        {
            List<SurveyAnswer> survey = getPaticipantFeedback(file_path);
            var unchecked_answer = new List<string>();
            foreach (var answer in survey)
            {
                var sentiment_string = answer.QA.Last().Value;
                unchecked_answer.Add(sentiment_string);
            }
            var clean_1 = validateSurveyPhase1(unchecked_answer);
            return clean_1;
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

        // support function
        private UserCredential Login()
        {
            string[] scopes = new[] { SheetsService.Scope.Spreadsheets };
            ClientSecrets secret = new ClientSecrets()
            {
                ClientId = GoogleClientId,
                ClientSecret = GoogleClientSecret,
            };
            try
            {
                Console.WriteLine("login successful");
                return GoogleWebAuthorizationBroker.AuthorizeAsync(secret, scopes, "user", CancellationToken.None).Result;
            }
            catch (Exception)
            {
                throw new Exception(SurveyErrorMessage.ERR_GOOGLE_API_CALL);
            }
        }

        private string replace_special_character(string s)
        {
            string output = "";
            output = Regex.Replace(s, SurveyConstant.REGREX_FILE_NAME, "");
            return output;
        }

        private Dictionary<string, List<string>> validate_row()
        {
            return null;
        }

        private List<string> validateSurveyPhase1(List<string> survey_question)
        {
            var clean_list = new List<string>();
            var no_stopword_string = new List<string>();
            foreach (string question in survey_question)
            {
                bool char_is_duplicate = false;
                string trimmed_question = question.Trim();
                string remove_special_character_question = Regex.Replace(trimmed_question, SurveyConstant.REGREX_SPECIAL_CHARACTER, " ");
                // check if the string only contain special character
                if (remove_special_character_question.Length == 0)
                {
                    continue;
                }
                // list of each character in the question
                var list_character = remove_special_character_question.Split(" ");
                // check for spam word next to each other
                for (int i = 0; i < list_character.Length; i++)
                {
                    if (i + 1 < list_character.Length)
                    {
                        if (remove_special_character_question[i] == remove_special_character_question[i + 1])
                        {
                            char_is_duplicate = true;
                            break;
                        }
                    }

                    // check for duplicate characters in the current string
                    for (int j = 0; j < list_character[i].Length; j++)
                    {
                        if (j + 1 < list_character[i].Length)
                        {
                            if (list_character[i][j] == list_character[i][j + 1])
                            {
                                char_is_duplicate = true;
                                break;
                            }
                        }
                    }
                }

                if (!char_is_duplicate)
                {
                    clean_list.Add(remove_special_character_question);
                }

            }
            return clean_list;
        }

        private void validateSurveyPhase2(List<string> survey_question, List<FeedbackResult> sentiment_feedback)
        {

        }

        private void groupSurveyByWorkshopSeries()
        {

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
