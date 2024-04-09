using Amazon.Runtime.Internal;
using Amazon.S3.Model;
using BusinessLogic.IRepository;
using BusinessLogic.Validator;
using COTSEClient.DTO;
using DataAccess.Common;
using DataAccess.Constants;
using DataAccess.DTO;
using DataAccess.Models;
using Google.Apis.Sheets.v4.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Newtonsoft.Json;
using OfficeOpenXml;
using System.Drawing;
using System.Text;
using System.Text.RegularExpressions;
using static IronPython.Modules._ast;

namespace BusinessLogic.Repository
{
    public class RepositorySurvey : IRepositorySurvey
    {
        // Repo constant
        private readonly Sep490G17DbContext _context;
        private readonly int key_length = 10;
        private SurveyValidator _validator;
        private readonly string? huggingFaceToken = Environment.GetEnvironmentVariable("huggingfaceToken");

        //constructor
        public RepositorySurvey(Sep490G17DbContext context)
        {
            _context = context;//
            _validator = new SurveyValidator();
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        }

        /// <summary>
        /// CRUD survey
        /// </summary>
        public List<SurveyDTO> getListSurvey(int wss_id, int ws_id)
        {
            var survey_list = _context.WorkshopSurveyUrls
                .Where(survey => survey.WorkshopSeriesId == wss_id && survey.WorkshopId == ws_id)
                .Select(survey => new SurveyDTO
                {
                    Id = survey.Id,
                    wssId = wss_id,
                    wsId = wss_id,
                    survey_name = survey.SurveyName,
                    survey_url = survey.Url != null ? survey.Url : survey.FileByte,
                    added_date = survey.AddedDate
                })
                .ToList();
            return survey_list;
        }

        // add survey with file
        public async Task<int> createNewSurveyFile(WorkshopInfoDTO ws_info)
        {

            string extension = Path.GetExtension(ws_info.url);
            string fileByte = getFileByte(ws_info.url);
            var ws_exist = await _context.WorkshopSeries
                .Join(_context.Workshops, wss => wss.Id, ws => ws.WorkshopSeriesId, (wss, ws) => new
                {
                    series_id = wss.Id,
                    workshop_id = ws.Id,
                }).SingleOrDefaultAsync(value => value.series_id == ws_info.wssId && value.workshop_id == ws_info.wsId);
            if (ws_exist == null)
            {
                throw new NullReferenceException();
            }
            try
            {
                var name = Path.GetFileNameWithoutExtension(ws_info.url);
                int file_supported = _validator.getFileType(extension);
                if (file_supported == COTSEConstants.CSV_INPUT)
                {
                    try
                    {
                        var file_data = ExtractCSVFile(ws_info.url);
                        if (file_data.Count == 0 || file_data == null)
                        {
                            throw new FormatException();
                        }
                    }
                    catch (Exception)
                    {
                        throw new FormatException();
                    }
                }
                else if (file_supported == COTSEConstants.EXCEL_INPUT)
                {
                    try
                    {
                        var file_data = ExtractExcelFile(ws_info.url);
                        if (file_data.Count == 0 || file_data == null)
                        {
                            throw new FormatException();
                        }
                    }
                    catch (Exception)
                    {
                        throw new FormatException();
                    }
                }
                else
                {
                    throw new NotImplementedException();
                }
                WorkshopSurveyUrl new_survey = new WorkshopSurveyUrl
                {
                    WorkshopSeriesId = ws_info.wssId,
                    WorkshopId = ws_info.wsId,
                    FileByte = fileByte,
                    FileType = extension,
                    AddedDate = DateTime.Now,
                    SurveyName = name,
                };
                // check if survey name is exist in database
                var exist_name = await _context.WorkshopSurveyUrls
                    .Where(survey => survey.SurveyName.StartsWith(name))
                    .ToListAsync();
                // change survey name
                if (exist_name.Count > 0)
                {
                    new_survey.SurveyName = $"{name} ({exist_name.Count})";
                }
                // check if the file is the same or not
                //var exist_survey = await _context.WorkshopSurveyUrls
                //    .Where(survey => survey.FileByte == new_survey.FileByte).ToListAsync();
                //if (exist_survey != null)
                //{
                //    throw new Exception();
                //}
                await _context.WorkshopSurveyUrls.AddAsync(new_survey);
                var state = await _context.SaveChangesAsync();
                if (state == 0)
                {
                    throw new FormatException();
                }
                else
                {
                    return state;
                }
            }
            catch (Exception)
            {
                throw new Exception();
            }
            finally
            {
                File.Delete(ws_info.url);
            }
        }

        public async Task<int> createNewSurveyUrl(WorkshopInfoDTO ws_info)
        {
            throw new NotImplementedException("not support google sheet yet!");
        }

        public async Task<string> getWorkshopData(int wssId, int wsId, string key)
        {

            try
            {
                int state = -1;
                WorkshopSurveyUrl? current_ws = null;
                var data = await _context.WorkshopSurveyUrls.Where(survey => survey.WorkshopId == wsId && survey.WorkshopSeriesId == wssId).ToListAsync();
                if (data.Any(survey => survey.Url == key))
                {
                    current_ws = data.SingleOrDefault(survey => survey.Url == key);
                    state = COTSEConstants.MODE_ADD_URL;
                }
                else
                {
                    current_ws = data.SingleOrDefault(survey => survey.FileByte == key);
                    state = COTSEConstants.MODE_ADD_FILE;
                }
                if (state == -1)
                {
                    throw new Exception();
                }
                else if (state == COTSEConstants.MODE_ADD_URL)
                {
                    throw new Exception("not support yet");
                }
                else if (state == COTSEConstants.MODE_ADD_FILE)
                {
                    string fileByte = current_ws?.FileByte;
                    string? name = current_ws.SurveyName;
                    if (name == null)
                    {
                        name = "";
                    }
                    string tmp_name = $"{name}.{current_ws.FileType}";
                    throw new Exception();
                }
                else
                {

                    throw new Exception();
                }


            }
            catch (Exception)
            {
                throw new Exception();
            }
        }

        // get workshop information
        public WorkshopInfoDTO getWorkshopInformation(int wss_id, int ws_id)
        {
            var selected_ws = _context.WorkshopSeries
                .Join(_context.Workshops,
                series => series.Id,
                ws => ws.WorkshopSeriesId,
                (series, ws) => new WorkshopInfoDTO
                {
                    wssId = series.Id,
                    wsId = ws.Id,
                    SeriesName = series.WorkshopSeriesName,
                    WorkshopName = ws.WorkshopName
                }).SingleOrDefault(ws_info => ws_info.wssId == wss_id && ws_info.wsId == ws_id);
            if (selected_ws != null)
            {
                return selected_ws;
            }
            else
            {
                throw new Exception("cant find ws");
            }
        }

        //get survey in workshop series
        public async Task<List<WorkshopSeriesWorkshopDTO>> seriesSurvey()
        {
            var allSeries = await _context.WorkshopSeries.Select(wss => new WorkshopSeriesWorkshopDTO
            {
                Id = wss.Id,
                WorkshopSeriesName = wss.WorkshopSeriesName,
                StartDate = wss.StartDate
            }).Distinct().ToListAsync();

            foreach (var series in allSeries)
            {
                var allWorkshops = await _context.Workshops.Where(ws => ws.WorkshopSeriesId == series.Id)
                    .Select(wsDTO => new WorkshopDTO
                    {
                        Id = wsDTO.Id,
                        DatePresent = wsDTO.DatePresent,
                        WorkshopName = wsDTO.WorkshopName,
                        KeyPresenter = wsDTO.KeyPresenter != null ? wsDTO.KeyPresenter.Replace(wsDTO.KeyPresenter, new string('*', wsDTO.KeyPresenter.Count())) : "",
                    })
                    .ToListAsync();
                foreach (var workshop in allWorkshops)
                {
                    var allSurveys = await _context.WorkshopSurveyUrls
                        .Where(survey => survey.WorkshopId == workshop.Id && survey.WorkshopSeriesId == series.Id)
                        .Select(data => new SurveyDTO
                        {
                            Id = data.Id,
                            added_date = data.AddedDate,
                            survey_name = data.SurveyName
                        })
                        .ToListAsync();
                    workshop.Survey = allSurveys;
                }
                series.workshops = allWorkshops;
            }

            return allSeries;
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

        public async Task<SurveyDTO> getSurey(int surveyId)
        {
            try
            {
                var survey = await _context.WorkshopSurveyUrls.Select(survey_info => new SurveyDTO
                {
                    Id = survey_info.Id,
                    survey_name = survey_info.SurveyName,
                    added_date = survey_info.AddedDate,

                })
                    .SingleOrDefaultAsync(survey => survey.Id == surveyId);
                if (survey == null)
                {
                    throw new NullReferenceException();
                }
                else
                {
                    return survey;
                }

            }
            catch (Exception)
            {
                throw new Exception();
            }
        }

        /// <summary>
        /// get survey display content
        /// </summary>
        public async Task<List<FeedbackResult>> getSurveySentimentResult(int surveyId)
        {
            string temp_path = "";
            try
            {
                var survey = _context.WorkshopSurveyUrls.Find(surveyId);
                if (survey != null)
                {
                    temp_path = readFile(survey.FileByte, $"{survey.SurveyName}{survey.FileType}");
                    var questions = await GetSentimentAnswer(temp_path);
                    var json_data = await GetJsonSentiment(questions);
                    var result = Rate(questions, json_data);
                    return result;
                }
                else
                {
                    throw new NullReferenceException();
                }
            }
            catch (Exception)
            {
                throw new Exception();
            }
        }

        public async Task<Dictionary<string, int>> CountFeedback(List<FeedbackResult> survey_sentiment)
        {
            Dictionary<string, int> data = new Dictionary<string, int>
            {
                { "Positive", survey_sentiment.Where(feedback => feedback.getResult() == "Positive").Count() },
                { "Negative", survey_sentiment.Where(feedback => feedback.getResult() == "Negative").Count() },
                { "Neutral", survey_sentiment.Where(feedback => feedback.getResult() == "Neutral").Count() }
            };
            return data;
        }

        public async Task<List<CommonQA>> getOtherData(int surveyId)
        {
            string temp_path = "";
            try
            {
                var output = new List<Dictionary<string, string>>();
                var survey = _context.WorkshopSurveyUrls.Find(surveyId);
                if (survey != null)
                {
                    var survey_content = new List<SurveyContentDTO>();
                    temp_path = readFile(survey.FileByte, $"{survey.SurveyName}{survey.FileType}");
                    if (_validator.getFileType(survey.FileType) == COTSEConstants.EXCEL_INPUT)
                    {
                        survey_content = ExtractExcelFile(temp_path);
                    }
                    else if (_validator.getFileType(survey.FileType) == COTSEConstants.CSV_INPUT)
                    {
                        survey_content = ExtractCSVFile(temp_path);
                    }
                    var commonqa = new List<CommonQA>();

                    // Define possible answer options for range 1-5
                    var rangeAnswers = new List<string> { "1", "2", "3", "4", "5" };

                    var groupedRangeQA = survey_content
                        .SelectMany(dto => dto.QA)
                        .Where(pair => _validator.common_question.Skip(2).Take(_validator.common_question.Count - 4).Contains(pair.Key))
                        .GroupBy(pair => pair.Key)
                        .ToDictionary(group => group.Key,
                                      group => group.Select(pair => pair.Value).ToList());

                    // Define possible answer options for "yes" or "no"
                    var yesNoAnswers = new List<string> { "có", "không" };

                    var groupedYesNoQA = survey_content
                        .SelectMany(dto => dto.QA)
                        .Where(pair => (_validator.common_question.Skip(_validator.common_question.Count - 2).Contains(pair.Key)))
                        .GroupBy(pair => pair.Key)
                        .ToDictionary(group => group.Key,
                                      group => group.Select(pair => pair.Value).ToList());

                    // Process range 1-5 questions
                    foreach (var key in _validator.common_question.Skip(2).Take(_validator.common_question.Count - 4))
                    {
                        // Create dictionary for counting answers
                        var answerCounts = new Dictionary<string, int>();

                        // Initialize counts to 0 for all possible answers
                        foreach (var answer in rangeAnswers)
                        {
                            answerCounts[answer] = 0;
                        }

                        // Update counts based on groupedQA
                        if (groupedRangeQA.ContainsKey(key))
                        {
                            foreach (var value in groupedRangeQA[key])
                            {
                                if (!answerCounts.ContainsKey(value))
                                    answerCounts[value] = 0;
                                answerCounts[value]++;
                            }
                        }

                        var answerCount = new CommonQA
                        {
                            Question = key,
                            Counts = answerCounts
                        };
                        commonqa.Add(answerCount);
                    }

                    // Process "yes" or "no" questions
                    foreach (var key in _validator.common_question.Skip(_validator.common_question.Count - 2))
                    {
                        // Create dictionary for counting answers
                        var answerCounts = new Dictionary<string, int>();

                        // Initialize counts to 0 for all possible answers
                        foreach (var answer in yesNoAnswers)
                        {
                            answerCounts[answer] = 0;
                        }

                        // Update counts based on groupedQA
                        if (groupedYesNoQA.ContainsKey(key))
                        {
                            foreach (var value in groupedYesNoQA[key])
                            {
                                if (!answerCounts.ContainsKey(value))
                                    answerCounts[value] = 0;
                                answerCounts[value]++;
                            }
                        }

                        var answerCount = new CommonQA
                        {
                            Question = key,
                            Counts = answerCounts
                        };
                        commonqa.Add(answerCount);
                    }
                    return commonqa;
                }
                else
                {
                    throw new NullReferenceException();
                }
            }
            catch (Exception)
            {
                throw new Exception();
            }
            finally
            {
                File.Delete(temp_path);
            }
        }





        /// <summary>
        ///  Support function
        /// </summary>
        private async Task<string> GetJsonSentiment(List<string> sentiment_data_list)
        {
            var json_input = JsonConvert.SerializeObject(sentiment_data_list);
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {huggingFaceToken} ");
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

        // get last question
        private async Task<List<string>> GetSentimentAnswer(string file_path)
        {
            try
            {

                List<SurveyContentDTO> survey = getPaticipantFeedback(file_path);
                var unchecked_answer = new List<string>();
                foreach (var answer in survey)
                {
                    var sentiment_string = answer.QA.Last().Value;
                    unchecked_answer.Add(sentiment_string);
                }
                var clean_1 = validateSurveyPhase1(unchecked_answer);
                return clean_1;

            }
            catch (Exception)
            {
                throw new Exception();
            }
        }

        // put rate to list
        private List<FeedbackResult> Rate(List<string> question_list, string json_string)
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


        // convert string file byte to byte
        private string readFile(string file_byte, string file_name)
        {
            var fileByte = Convert.FromBase64String(file_byte);
            string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "temp", $"{file_name}");
            using (FileStream fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
            {
                fileStream.Write(fileByte, 0, fileByte.Length);
            }
            return filePath;
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

        private List<SurveyContentDTO> getPaticipantFeedback(string file_path)
        {
            if (!File.Exists(file_path))
            {
                throw new FileNotFoundException();
            }
            switch (file_path.Split(".")[^1])
            {
                case "xlsx":
                    return ExtractExcelFile(file_path);
                case "csv":
                    return ExtractCSVFile(file_path);
                default:
                    throw new NotImplementedException();
            }
        }

        // convert fileByte to string byte
        private string getFileByte(string file_path)
        {
            try
            {
                if (File.Exists(file_path))
                {
                    byte[] fileByte = File.ReadAllBytes(file_path);
                    return Convert.ToBase64String(fileByte);
                }
                else
                {
                    throw new FileNotFoundException();
                }
            }
            catch (FileNotFoundException)
            {
                throw new FileNotFoundException();
            }
        }


        // extract value from file

        private List<SurveyContentDTO> ExtractExcelFile(string file_path)
        {
            List<SurveyContentDTO> list_response = new List<SurveyContentDTO>();
            using (var package = new ExcelPackage(new FileInfo(file_path)))
            {
                int total_worksheet = package.Workbook.Worksheets.Count();
                if (total_worksheet > 1)
                {
                    throw new Exception();
                }
                ExcelWorksheet worksheet = package.Workbook.Worksheets[0];
                int rowCount = worksheet.Dimension.Rows;
                int colCount = worksheet.Dimension.Columns;
                //start count from 1
                var header = new string[colCount + 1];
                // get list header
                for (int col = 1; col <= colCount; col++)
                {

                    var rol_value = worksheet.Cells[1, col].Text;
                    header[col] = rol_value;
                }
                header = header.Skip(1).ToArray();
                //validate header
                if (!_validator.validateHeader(header))
                {
                    throw new FormatException();
                }
                // count from the first response
                for (int row = 2; row <= rowCount - 1; row++)
                {
                    var list_row = new List<Tuple<string, string?>>();
                    for (int col = 1; col <= colCount; col++)
                    {
                        var content = worksheet.Cells[row, col].Text;
                        var response = new Tuple<string, string?>(header[col - 1], content.ToString());
                        if (!_validator.validateCommonRow(response))
                        {
                            throw new FormatException();
                        }
                        list_row.Add(response);
                    }
                    var response_output = PutData(list_row);
                    list_response.Add(response_output);
                }
            }
            return list_response;
        }


        private List<SurveyContentDTO> ExtractCSVFile(string file_path)
        {
            List<SurveyContentDTO> list_response = new List<SurveyContentDTO>();
            using (var stream = new StreamReader(file_path))
            {
                var headerLine = stream.ReadLine();
                string[] header = headerLine.Split(",");
                if (!_validator.validateHeader(header))
                {
                    throw new FormatException();
                }
                string line;
                while ((line = stream.ReadLine()) != null)
                {
                    string[] body_content = line.Split(",");
                    var list_row = new List<Tuple<string, string?>>();
                    for (int i = 0; i < body_content.Length; i++)
                    {
                        var response = new Tuple<string, string?>(header[i], body_content[i]);
                        if (!_validator.validateCommonRow(response))
                        {
                            throw new FormatException();
                        }
                        list_row.Add(response);
                    }
                    var response_output = PutData(list_row);
                    list_response.Add(response_output);
                }
            }
            return list_response;
        }

        private SurveyContentDTO PutData(List<Tuple<string, string?>> list_row)
        {
            var question_list = _validator.common_question;
            SurveyContentDTO survey = new SurveyContentDTO();
            Dictionary<string, string> qa = new Dictionary<string, string>();
            //list response
            foreach (var data in list_row)
            {
                //every single col in the response
                if (_validator.common_question.Contains(data.Item1.ToLower()))
                {
                    if (_validator.common_question.IndexOf(data.Item1.ToLower()) == 0)
                    {
                        // time stamp
                        DateTime time = DateTime.ParseExact(data.Item2, _validator.dateFormats, null);
                        survey.timeStamp = time;
                    }
                    else if (_validator.common_question.IndexOf(data.Item1.ToLower()) == 1)
                    {
                        //email
                        survey.AnswerBy = data.Item2;
                    }
                    else
                    {
                        qa.Add(data.Item1, data.Item2);
                    }
                }
                else
                {
                    //sentiment question
                    qa.Add(data.Item1, data.Item2);
                }
            }
            survey.QA = qa;

            if (survey.QA.Count <= 0)
            {
                throw new Exception();
            }
            else
            {
                return survey;
            }
        }

    }
}