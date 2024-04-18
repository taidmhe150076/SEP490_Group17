using DataAccess.Common;
using DataAccess.Constants;
using DataAccess.DTO;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Validator
{
    public class SurveyValidator
    {
        private List<string> file_types = new List<string>() {
            ".csv",
            ".xlsx",
            "csv",
            "xlsx",
        };

        public string[] dateFormats = { "M/d/yyyy HH:mm:ss", "M/d/yyyy HH:mm", "dd/MM/yyyy HH:mm:ss", "dd/MM/yyyy HH:mm"};

        // list common question
        public List<string> common_question = new List<string>()
        {
            "Timestamp",
            "Email address",
            "bạn có học được nhiều từ series workshop này không?",
            "thời gian và địa điểm của workshop có hợp lý không?",
            "bạn thấy chất lượng của workshop có tốt không?",
            "bạn thấy sao về cách chia sẻ của các diễn giả?",
            "bạn thấy thông tin mà diễn giả trình bày có dễ hiểu không?",
            "bạn có muốn được gặp lại các diễn giả trong các buổi workshop sau hay không?",
            "nếu được tổ chức thêm các sự kiện workshop khác liên quan thì bạn có muốn tham gia không?",
        }.ConvertAll(question => question.ToLower());

        public const string sentiment_question = "cảm nghĩ của bạn về series workshop này như thế nào?";

        public int getFileType(string file_extension) {
            var index = file_types.IndexOf(file_extension);
            if (index == COTSEConstants.EXCEL_INPUT || index == 3)
            {
                return COTSEConstants.EXCEL_INPUT;
            }
            else if (index == COTSEConstants.CSV_INPUT || index == 4)
            {
                return COTSEConstants.CSV_INPUT;
            }
            else {
                throw new NotImplementedException();   
            }
        }

        public bool validateUrlPath(string url)
        {
            string header = "https://docs.google.com/spreadsheets";
            if (!url.Contains(header))
            {
                return false;
            }
            return true;
        }

        public bool validateFileExtension(string extension)
        {
            if (extension == null)
            {
                return false;
            }
            if (!file_types.Contains(extension))
            {
                return false;
            }
            return true;
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

        public List<(string, int)> validateFilesName(List<string> filesName)
        {
            var output = new List<(string, int)>();
            foreach (string file_name in filesName)
            {
                int valid_point = validate_file_name(file_name);
                output.Add((file_name, 
                    valid_point));
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
            return SurveyConstant.VALID_NAME_FORMAT;
        }


        //random
        //add file to temp folder (dev)
        public string GetsaveFileToTemp(string fileName, int? saveMode)
        {
            string error_message = string.Empty;

            var file_extension = Path.GetExtension(fileName);

            if (!file_types.Contains(file_extension))
            {
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

        // validate excel header
        public bool validateHeader(string[] header)
        {
            // default value in excel file
            if (header.Length < common_question.Count + 1)
            {
                return false;
            }
            // check if all of the first common question is identical
            var inputCommonHeader = header.Take(common_question.Count).Select(question => question.Trim().ToLower()).ToList();
            var x = header.Take(common_question.Count).Any(h => common_question.Any(q => q.StartsWith(h)));

            bool isCorrectRequireQuestion = inputCommonHeader.SequenceEqual(common_question.Select(cq => cq.Trim().ToLower()).ToList());
            if (!isCorrectRequireQuestion)
            {
                return false;
            }

            //check if the last question is sentiment question
            if (header[^1].Trim().ToLower() != sentiment_question)
            {
                return false;
            }
            return true;
        }

        public bool validateCommonRow(Tuple<string, string?> response)
        {
            var question = response.Item1.Trim().ToLower();
            var questionIndex = common_question.IndexOf(question);

            // if it is not the common question then skip validate
            if (questionIndex == -1)
            {
                if (question == sentiment_question)
                {
                    // do some thing with sentiment
                    return true;
                }
                else
                {
                    //other dont do anything
                    return true;
                }
            }
            else
            {
                string value = response.Item2.ToString();
                if (questionIndex == 0)
                {
                    DateTime test;
                    if (DateTime.TryParseExact(value, dateFormats, CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out test))
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else if (questionIndex == 1)
                {
                    // validate email
                    return true;
                }
                else
                {
                    // validate int
                    int outputvalue;
                    if (int.TryParse(value, out outputvalue))
                    {
                        return true;
                    }
                    else
                    {
                        //validate 2 string question
                        if (value == "Có" || value == "Không")
                        {
                            return true;
                        }
                        return false;
                    }
                }
            }
        }


        public bool validateCSVFile() {
            return true;
        }

        internal bool ValidateFieldFormat(List<SurveyContentDTO> file_data)
        {
            throw new NotImplementedException();
        }
    }
}
