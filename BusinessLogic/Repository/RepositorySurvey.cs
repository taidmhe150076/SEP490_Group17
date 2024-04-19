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
using static Community.CsharpSqlite.Sqlite3;
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

        public WorkshopInfoDTO getCurrentWorkshopInformation(int wssId, int wsId, int surveyId)
        {

            var selected_ws = _context.WorkshopSeries
            .Include(series => series.Workshops)
            .ThenInclude(ws => ws.UrlForms)
            .ThenInclude(form => form.WorkshopSurveyUrlNavigation)
            .Where(series => series.Id == wssId)
            .SelectMany(series => series.Workshops, (series, ws) => new { series, ws })
            .Where(join => join.ws.Id == wsId)
            .SelectMany(join => join.ws.UrlForms, (join, form) => new WorkshopInfoDTO
            {
                wssId = join.series.Id,
                wsId = join.ws.Id,
                SeriesName = join.series.WorkshopSeriesName,
                WorkshopName = join.ws.WorkshopName,
                FormUrl = form.UrlForm1,
                isPresenter = form.IsPresenter,
                survey_id = form.WorkshopSurveyUrl,
                url = form.WorkshopSurveyUrlNavigation.Url ?? form.WorkshopSurveyUrlNavigation.FileByte,
                fileType = form.WorkshopSurveyUrlNavigation.FileType,
                surveyName = form.WorkshopSurveyUrlNavigation.SurveyName
            })

            .Where(dto => dto.survey_id == surveyId)
            .SingleOrDefault(); // Execute the query and get the single result
            if (selected_ws != null)
            {
                return selected_ws;
            }
            else
            {
                throw new Exception("cant find ws");
            }
        }

        public async Task<bool> deleteSurvey(int wssId, int wsId, int surveyId)
        {
            try
            {
                var exist = await _context.Workshops.Include(context => context.WorkshopSeries).SingleOrDefaultAsync(ws => ws.Id == wsId && ws.WorkshopSeries.Id == wssId);
                if (exist == null)
                {
                    throw new NullReferenceException();
                }
                else
                {
                    var url = await _context.UrlForms.SingleOrDefaultAsync(form => form.WorkshopSurveyUrl == surveyId);
                    var survey = await _context.WorkshopSurveyUrls.SingleOrDefaultAsync(survey => survey.Id == surveyId);
                    if (url == null || survey == null)
                    {
                        throw new Exception();
                    }
                    _context.UrlForms.Remove(url);
                    _context.WorkshopSurveyUrls.Remove(survey);
                    var state = await _context.SaveChangesAsync();
                    if (state > 0)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            catch (Exception)
            {
                throw new Exception();
            }
        }

        // add survey with file
        public async Task<int> createNewSurveyFile(WorkshopInfoDTO ws_info)
        {

            string extension = Path.GetExtension(ws_info.url);
            string fileByte = getFileByte(ws_info.url);

            var ws_exist = await _context.Workshops
                .Include(context => context.WorkshopSeries)
                .SingleOrDefaultAsync(ws => ws.Id == ws_info.wsId && ws.WorkshopSeries.Id == ws_info.wssId);
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
                    FileByte = fileByte,
                    FileType = extension,
                    AddedDate = System.DateTime.Now,
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

                var addedEntity = _context.Entry(new_survey).Entity;

                var urlForm = new UrlForm
                {
                    WorkshopId = (int)ws_info.wsId,
                    IsPresenter = ws_info.isPresenter,
                    Workshop = ws_exist,
                    WorkshopSurveyUrlNavigation = addedEntity,
                    UrlForm1 = ws_info.FormUrl,
                };
                await _context.UrlForms.AddAsync(urlForm);

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

        public async Task<int> createNewSurvey(WorkshopInfoDTO ws_info)
        {
            try
            {
                var workshop = await _context.Workshops
                    .Include(context => context.WorkshopSeries)
                    .SingleOrDefaultAsync(ws => ws.Id == ws_info.wsId && ws.WorkshopSeries.Id == ws_info.wssId);
                if (workshop == null)
                {
                    throw new NullReferenceException();
                }
                else
                {
                    var survey = new WorkshopSurveyUrl
                    {
                        AddedDate = System.DateTime.Now,
                    };

                    await _context.WorkshopSurveyUrls.AddAsync(survey);
                    await _context.SaveChangesAsync();
                    var addedEntity = _context.Entry(survey).Entity;

                    var urlForm = new UrlForm
                    {
                        WorkshopId = (int)ws_info.wsId,
                        IsPresenter = ws_info.isPresenter,
                        Workshop = workshop,
                        WorkshopSurveyUrlNavigation = addedEntity,
                        UrlForm1 = ws_info.FormUrl,
                    };
                    await _context.UrlForms.AddAsync(urlForm);
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

            }
            catch
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

        public async Task<List<WorkshopSurveyDTO>> surveyList()
        {
            var workshopSeries = await _context.WorkshopSeries
                .Include(wss => wss.Workshops)
                    .ThenInclude(ws => ws.UrlForms)
                        .ThenInclude(form => form.WorkshopSurveyUrlNavigation)
                .Select(wss => new WorkshopSurveyDTO
                {
                    Id = wss.Id,
                    StartDate = wss.StartDate,
                    WorkshopSeriesName = wss.WorkshopSeriesName,
                    workshops = wss.Workshops.Select(ws => new WorkshopDTO
                    {
                        Id = ws.Id,
                        WorkshopSeriesId = wss.Id,
                        DatePresent = ws.DatePresent,
                        KeyPresenter = ws.KeyPresenter,
                        WorkshopName = ws.WorkshopName,
                        Survey = ws.UrlForms.Select(form => new SurveyDTO
                        {
                            Id = form.Id,
                            survey_form = form.UrlForm1,
                            isPresenter = form.IsPresenter == true ? SurveyConstant.IS_PRESENTER_FORM : SurveyConstant.IS_PATICIPANT_FORM,
                            survey_name = form.WorkshopSurveyUrlNavigation.SurveyName ?? "unname survey",
                            added_date = form.WorkshopSurveyUrlNavigation.AddedDate,
                            survey_path = form.WorkshopSurveyUrlNavigation.Url != null ? "link" : (form.WorkshopSurveyUrlNavigation.FileByte != null ? "file" : null)
                        }).ToList()
                    }).ToList()
                }).ToListAsync();

            return workshopSeries;
        }


        public async Task<WorkshopSurveyUrl> findSurvey(int wssId, int wsId, int surveyId)
        {
            var survey = await _context.WorkshopSurveyUrls
                .Include(survey => survey.UrlForms)
                .ThenInclude(urlForm => urlForm.Workshop)
                .ThenInclude(workshop => workshop.WorkshopSeries)
                .Where(survey => survey.Id == surveyId &&
                     survey.UrlForms.Any(urlForm => urlForm.WorkshopId == wsId && urlForm.Workshop.WorkshopSeries.Id == wssId))
                .SingleOrDefaultAsync();

            if (survey == null)
            {
                throw new NullReferenceException();
            }
            else
            {
                return survey;
            }
        }
        /// <summary>
        ///  Support 
        ///  
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
            string fileFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "temp");
            // coutner feed
            if (!Directory.Exists(fileFolder))
            {
                Directory.CreateDirectory(fileFolder);
            }
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
                        System.DateTime time = System.DateTime.ParseExact(data.Item2, _validator.dateFormats, null);
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

        public async Task<int> updateSurvey(WorkshopInfoDTO info, int mode)
        {
            try
            {
                var survey = await _context.WorkshopSurveyUrls.SingleOrDefaultAsync(survey => survey.Id == info.survey_id);
                var urlForm = await _context.UrlForms.SingleOrDefaultAsync(url => url.WorkshopSurveyUrl == info.survey_id);
                if (info == null || survey == null || urlForm == null)
                {
                    throw new NullReferenceException(SurveyErrorMessage.ERR_UPDATE_SURVEY_FAIL);
                }
                if (mode == COTSEConstants.MODE_ADD_FILE)
                {
                    if (survey.Url != null)
                    {
                        throw new NullReferenceException(SurveyErrorMessage.ERR_UPDATE_SURVEY_FAIL);
                    }
                    if (survey.Url == null)
                    {
                        string file_path = info.url;
                        var file = readFileByte(file_path);
                        survey.FileByte = file.Item1;
                        survey.FileType = file.Item2;
                        string survey_name = Path.GetFileNameWithoutExtension(file_path);
                        if (survey.SurveyName == null)
                        {
                            survey.SurveyName = survey_name;
                        }
                        else if (!survey.SurveyName.Equals(survey_name))
                        {
                            survey.SurveyName = survey_name;
                        }
                    }
                }
                else if (mode == COTSEConstants.MODE_ADD_URL)
                {
                    if (survey.FileByte != null)
                    {
                        survey.FileByte = null;
                        survey.FileType = null;
                        survey.Url = info.url;
                    }
                    else
                    {
                        survey.Url = info.url;
                    }
                }

                urlForm.UrlForm1 = info.FormUrl;
                var state = await _context.SaveChangesAsync();
                return state;
            }
            catch (Exception)
            {
                throw new Exception();
            }

        }


        private (string, string) readFileByte(string file_path)
        {
            try
            {
                var extension = Path.GetExtension(file_path);
                var name = Path.GetFileNameWithoutExtension(file_path);
                int file_supported = _validator.getFileType(extension);
                if (file_supported == COTSEConstants.CSV_INPUT)
                {
                    var file_data = ExtractCSVFile(file_path);
                    if (file_data.Count == 0 || file_data == null)
                    {
                        throw new FormatException();
                    }
                }
                else if (file_supported == COTSEConstants.EXCEL_INPUT)
                {
                    var file_data = ExtractExcelFile(file_path);
                    if (file_data.Count == 0 || file_data == null)
                    {
                        throw new FormatException();
                    }
                }
                else
                {
                    throw new NotImplementedException();
                }

                string fileByte = getFileByte(file_path);
                return (fileByte, extension);
            }
            catch (Exception)
            {
                throw new Exception();
            }
        }

        public int GetWorkshopSurveyUrlIdOfParticipants(int wsId)
        {
            try
            {
                var findUrlForms = _context.UrlForms.FirstOrDefault(x => x.WorkshopId == wsId && x.IsPresenter == false);
                if (findUrlForms == null)
                {
                    throw new Exception();
                }
                return findUrlForms.WorkshopSurveyUrl;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
