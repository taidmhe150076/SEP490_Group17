//using BusinessLogic.IRepository;
//using COTSEClient.DTO;
//using DataAccess.Common;
//using DataAccess.Constants;
//using DataAccess.DTO;
//using DataAccess.Models;
//using Microsoft.EntityFrameworkCore;
//using Newtonsoft.Json;
//using System.Drawing;
//using System.Text;
//using System.Text.RegularExpressions;

//namespace BusinessLogic.Repository
//{
//    public class RepositorySurvey : IRepositorySurvey
//    {
//        // Repo constant
//        private readonly Sep490G17DbContext _context;
//        private IRepositoryAWS _aws_repo;
//        private readonly int key_length = 10;


//        private readonly string? huggingFaceToken = Environment.GetEnvironmentVariable("huggingfaceToken");

//        //constructor
//        public RepositorySurvey(Sep490G17DbContext context)
//        {
//            _context = context;//
//            _aws_repo = new RepositoryAWS();
//        }


//        // get survey infomation 
//        public List<SurveyDTO> getListSurvey(int wss_id, int ws_id)
//        {
//            var survey_list = _context.WorkshopSurveyUrls
//                .Where(survey => survey.WorkshopSeriesId == wss_id && survey.WorkshopId == ws_id)
//                .Select(survey => new SurveyDTO
//                {
//                    Id = survey.Id,
//                    wssId = wss_id,
//                    wsId = wss_id,
//                    survey_name = survey.SurveyName,
//                    survey_url = survey.SurveyUrl != null ? survey.SurveyUrl : survey.SurveyKey,
//                    added_date = survey.AddedDate
//                })
//                .ToList();
//            return survey_list;
//        }


//        public async Task<string> getWorkshopData(int wssId, int wsId, string key)
//        {

//            try
//            {
//                int state = -1;
//                WorkshopSurveyUrl? current_ws = null;
//                var data = await _context.WorkshopSurveyUrls.Where(survey => survey.WorkshopId == wsId && survey.WorkshopSeriesId == wssId).ToListAsync();
//                if (data.Any(survey => survey.SurveyUrl == key))
//                {
//                    current_ws = data.SingleOrDefault(survey => survey.SurveyUrl == key);
//                    state = COTSEConstants.MODE_ADD_URL;
//                }
//                else
//                {
//                    current_ws = data.SingleOrDefault(survey => survey.SurveyKey == key);
//                    state = COTSEConstants.MODE_ADD_FILE;
//                }
//                if (state == -1)
//                {
//                    throw new Exception();
//                }
//                else if (state == COTSEConstants.MODE_ADD_URL)
//                {
//                    throw new Exception("not support yet");
//                }
//                else if (state == COTSEConstants.MODE_ADD_FILE)
//                {
//                    var s3_key = current_ws?.SurveyKey;
//                    var temp_data = await _aws_repo.GetS3Object(s3_key);
//                    if (temp_data)
//                    {
//                        return s3_key;
//                    }
//                    else  {
//                        throw new Exception("key not found");
//                    }
//                }
//                else
//                {

//                    throw new Exception();
//                }


//            }
//            catch (Exception)
//            {
//                throw new Exception();
//            }
//        }

//        // get workshop information
//        public WorkshopInfoDTO getWorkshopInformation(int wss_id, int ws_id)
//        {
//            var selected_ws = _context.WorkshopSeries
//                .Join(_context.Workshops,
//                series => series.Id,
//                ws => ws.WorkshopSeriesId,
//                (series, ws) => new WorkshopInfoDTO
//                {
//                    wssId = series.Id,
//                    wsId = ws.Id,
//                    SeriesName = series.WorkshopSeriesName,
//                    WorkshopName = ws.WorkshopName
//                }).SingleOrDefault(ws_info => ws_info.wssId == wss_id && ws_info.wsId == ws_id);
//            if (selected_ws != null)
//            {
//                return selected_ws;
//            }
//            else
//            {
//                throw new Exception("cant find ws");
//            }
//        }


//        public async Task<int> addSurvey(WorkshopInfoDTO workshopinfo, int mode, string? filePath)
//        {
//            if (!int.TryParse(workshopinfo.wsId.ToString(), out int wsId) || !int.TryParse(workshopinfo.wssId.ToString(), out int wssId))
//            {
//                return COTSEConstants.DB_STATUS_FAIL;
//            }

//            WorkshopSurveyUrl newSurvey = new WorkshopSurveyUrl
//            {
//                WorkshopSeriesId = wssId,
//                WorkshopId = wsId,
//                AddedDate = DateTime.Now
//            };

//            if (mode == COTSEConstants.MODE_ADD_URL)
//            {
//                newSurvey.SurveyUrl = workshopinfo.url;
//                var existingSurveyUrl = await _context.WorkshopSurveyUrls.SingleOrDefaultAsync(survey => survey.SurveyUrl == newSurvey.SurveyUrl);
//                if (existingSurveyUrl != null)
//                {
//                    return COTSEConstants.DB_STATUS_EXIST;
//                }
//                else
//                {
//                    newSurvey.SurveyName = "Google Form survey";
//                }
//            }
//            else if (mode == COTSEConstants.MODE_ADD_FILE)
//            {
//                newSurvey.SurveyKey = workshopinfo.url;
//                string surveyName = workshopinfo.url.Split(".")[0];
//                int similarSurveyCount = await _context.WorkshopSurveyUrls.Where(survey => survey.SurveyName.Contains(surveyName)).CountAsync();
//                newSurvey.SurveyName = similarSurveyCount > 0 ? $"{surveyName}-{similarSurveyCount}" : surveyName;
//                newSurvey.SurveyKey = $"{await GenerateUniqueSurveyKey(key_length)}.{workshopinfo.url.Split(".")[^1]}";
//                int awsStatus = await _aws_repo.UploadDataToS3(filePath, $"{newSurvey.SurveyKey}");
//                if (awsStatus != COTSEConstants.DB_STATUS_SUCCESS)
//                {
//                    return COTSEConstants.DB_STATUS_FAIL;
//                }
//            }
//            else
//            {
//                return COTSEConstants.DB_STATUS_FAIL;
//            }

//            _context.WorkshopSurveyUrls.Add(newSurvey);
//            int state = await _context.SaveChangesAsync();
//            return state == 0 ? COTSEConstants.DB_STATUS_FAIL : COTSEConstants.DB_STATUS_SUCCESS;
//        }


//        //get survey in workshop series
//        public async Task<List<WorkshopSeriesWorkshop>> seriesSurvey()
//        {
//            var result = new List<WorkshopSeriesWorkshop>();
//            var seriesContainSurvey = await _context.WorkshopSeries
//                //.Join(_context.WorkshopSurveyUrls, series => series.Id, survey => survey.WorkshopSeriesId, (series, survey) => series)
//                .Distinct().ToListAsync();
//            foreach (var series in seriesContainSurvey)
//            {
//                var workshop_list = await _context.Workshops
//                    .Where(ws => ws.WorkshopSeriesId == series.Id)
//                    //.Join(_context.WorkshopSurveyUrls, ws => ws.Id, survey => survey.WorkshopId, (ws, survey) => ws)
//                    .Select(s => new WorkshopDTO
//                    {
//                        Id = s.Id,
//                        DatePresent = s.DatePresent,
//                        WorkshopName = s.WorkshopName,
//                        KeyPresenter = s.KeyPresenter != null ? s.KeyPresenter.Replace(s.KeyPresenter, new string('*', s.KeyPresenter.Count())) : "",
//                    }).ToListAsync();
//                var wss = new WorkshopSeriesWorkshop
//                {
//                    Id = series.Id,
//                    WorkshopSeriesName = series.WorkshopSeriesName,
//                    StartDate = series.StartDate,
//                    workshops = workshop_list
//                };
//                result.Add(wss);
//            }
//            return result;
//        }


//        public void getSurvey(string wss_id)
//        {

//        }


//        public List<SurveyUrl> listSurvey(string wss_id, string ws_id)
//        {
//            return null;
//        }


//        // get single survey by workshop name
//        public WorkshopSurveyUrl getSurveyByWorkshop(string workshop_series_id, string workshop_id)
//        {
//            var survey = _context.WorkshopSurveyUrls.SingleOrDefault(s => s.WorkshopSeriesId == int.Parse(workshop_series_id) && s.WorkshopId == int.Parse(workshop_id));
//            if (survey == null)
//            {
//                throw new Exception(SurveyErrorMessage.ERR_WS_URL);
//            }
//            return survey;
//        }



//        // get hugging face api
//        public async Task<string> GetJsonSentiment(List<string> sentiment_data_list)
//        {
//            var json_input = JsonConvert.SerializeObject(sentiment_data_list);
//            using (HttpClient client = new HttpClient())
//            {
//                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {huggingFaceToken} ");
//                client.DefaultRequestHeaders.Add("Accept", "application/json");
//                var json_content = new StringContent(json_input, Encoding.UTF8, "application/json");
//                HttpResponseMessage resp = await client.PostAsync(SurveyConstant.HUGGINGFACE_API_URL, json_content);
//                if (resp.IsSuccessStatusCode)
//                {
//                    string json_output = await resp.Content.ReadAsStringAsync();
//                    return json_output;
//                }
//                else
//                {
//                    throw new Exception(SurveyErrorMessage.ERR_HF_API);
//                }
//            }
//            throw new NotImplementedException();
//        }


//        public List<string> GetSentimentAnswer(string file_path)
//        {
//            List<SurveyAnswer> survey = getPaticipantFeedback(file_path);
//            var unchecked_answer = new List<string>();
//            foreach (var answer in survey)
//            {
//                var sentiment_string = answer.QA.Last().Value;
//                unchecked_answer.Add(sentiment_string);
//            }
//            var clean_1 = validateSurveyPhase1(unchecked_answer);
//            return clean_1;
//        }

//        public List<FeedbackResult> Rate(List<string> question_list, string json_string)
//        {
//            var sentiment_result = new List<FeedbackResult>();
//            double? NEU = null;
//            double? NEG = null;
//            double? POS = null;
//            List<List<Dictionary<string, string>>>? json_data =
//                JsonConvert.DeserializeObject<List<List<Dictionary<string, string>>>>(json_string);
//            for (int i = 0; i < json_data.Count; i++)
//            {
//                for (int j = 0; j < json_data[i].Count; j++)
//                {
//                    //get 3 dictionary with value pos and value message
//                    // get key : contain POS, NEU, NEG
//                    var key = json_data[i][j]["label"];
//                    // get value: contain the score of key
//                    var value = json_data[i][j]["score"];
//                    switch (key)
//                    {
//                        case "NEG":
//                            NEG = double.Parse(value);
//                            break;
//                        case "NEU":
//                            NEU = double.Parse(value);
//                            break;
//                        case "POS":
//                            POS = double.Parse(value);
//                            break;
//                        default:
//                            throw new Exception("value error");
//                    }
//                }
//                if (NEG != null && NEU != null && POS != null)
//                {
//                    var result = new FeedbackResult()
//                    {
//                        Question = question_list[i],
//                        NEG = (double)NEG,
//                        POS = (double)POS,
//                        NEU = (double)NEU
//                    };
//                    sentiment_result.Add(result);
//                }
//            }

//            return sentiment_result;
//        }

//        // support function

//        private string replace_special_character(string s)
//        {
//            string output = "";
//            output = Regex.Replace(s, SurveyConstant.REGREX_FILE_NAME, "");
//            return output;
//        }

//        private Dictionary<string, List<string>> validate_row()
//        {
//            return null;
//        }

//        private List<string> validateSurveyPhase1(List<string> survey_question)
//        {
//            var clean_list = new List<string>();
//            var no_stopword_string = new List<string>();
//            foreach (string question in survey_question)
//            {
//                bool char_is_duplicate = false;
//                string trimmed_question = question.Trim();
//                string remove_special_character_question = Regex.Replace(trimmed_question, SurveyConstant.REGREX_SPECIAL_CHARACTER, " ");
//                // check if the string only contain special character
//                if (remove_special_character_question.Length == 0)
//                {
//                    continue;
//                }
//                // list of each character in the question
//                var list_character = remove_special_character_question.Split(" ");
//                // check for spam word next to each other
//                for (int i = 0; i < list_character.Length; i++)
//                {
//                    if (i + 1 < list_character.Length)
//                    {
//                        if (remove_special_character_question[i] == remove_special_character_question[i + 1])
//                        {
//                            char_is_duplicate = true;
//                            break;
//                        }
//                    }

//                    // check for duplicate characters in the current string
//                    for (int j = 0; j < list_character[i].Length; j++)
//                    {
//                        if (j + 1 < list_character[i].Length)
//                        {
//                            if (list_character[i][j] == list_character[i][j + 1])
//                            {
//                                char_is_duplicate = true;
//                                break;
//                            }
//                        }
//                    }
//                }

//                if (!char_is_duplicate)
//                {
//                    clean_list.Add(remove_special_character_question);
//                }

//            }
//            return clean_list;
//        }

//        private void validateSurveyPhase2(List<string> survey_question, List<FeedbackResult> sentiment_feedback)
//        {

//        }

//        private void groupSurveyByWorkshopSeries()
//        {

//        }

//        private Exception DataInvalidException()
//        {
//            throw new Exception("Invalid file path");
//        }

//        private List<SurveyAnswer> getPaticipantFeedback(string file_path)
//        {
//            if (!File.Exists(file_path))
//            {
//                throw DataInvalidException();
//            }
//            switch (file_path.Split(".")[^1])
//            {
//                case "xlsx":
//                    return ReadExcelFile(file_path);
//                case "csv":
//                    return ReadCSVFile(file_path);
//                default:
//                    throw DataInvalidException();
//            }
//        }

//        private List<SurveyAnswer> ReadCSVFile(string file_path)
//        {
//            List<SurveyAnswer> survey_answer = new List<SurveyAnswer>();
//            using (var reader = new StreamReader(file_path, Encoding.UTF8))
//            {
//                var current_line = 0;
//                string? name = null;
//                DateTime? timestamp = null;
//                var headers = new List<string>();
//                while (!reader.EndOfStream)
//                {
//                    var line = reader.ReadLine();
//                    var line_split = line.Split(",");
//                    //add header
//                    if (current_line == 0)
//                    {
//                        foreach (var header in line_split.Skip(2).ToList())
//                        {
//                            headers.Add(header);
//                        }
//                    }
//                    else
//                    {
//                        var qa = new Dictionary<string, string>();
//                        timestamp = DateTime.Parse(line_split[0]);
//                        name = line_split[1];
//                        var list_answer = line_split.Skip(2).ToList();
//                        for (int i = 0; i < headers.Count; i++)
//                        {
//                            qa[headers[i]] = list_answer[i];
//                        }
//                        SurveyAnswer answer = new SurveyAnswer()
//                        {
//                            timeStamp = timestamp,
//                            AnswerBy = name,
//                            QA = qa,
//                        };
//                        survey_answer.Add(answer);
//                    }
//                    //next line
//                    current_line++;
//                }
//            }
//            return survey_answer;
//        }

//        private List<SurveyAnswer> ReadExcelFile(string file_path)
//        {
//            throw new NotImplementedException();
//        }


//        // support method
//        private string randomKey(int length)
//        {
//            string randomString = "";
//            Random r = new Random();
//            for (int i = 0; i < length; i++)
//            {
//                int n = r.Next(10);
//                randomString += n;
//            }
//            return randomString;

//        }

//        private async Task<string> GenerateUniqueSurveyKey(int keyLength)
//        {
//            string surveyKey = randomKey(keyLength);
//            while (await _context.WorkshopSurveyUrls.AnyAsync(obj => obj.SurveyKey == surveyKey))
//            {
//                surveyKey = randomKey(keyLength);
//            }
//            return surveyKey;
//        }
//    }
//}
