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
        public string[] dateFormats = { "M/d/yyyy HH:mm:ss", "M/d/yyyy HH:mm", "dd/MM/yyyy HH:mm:ss", "dd/MM/yyyy HH:mm","MM/dd/yyyy hh:mm:ss", "M/dd/yyyy H:mm:ss" };

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
            string file_type = Path.GetExtension(filePath);
            if (!file_types.Contains(file_type))
            {
                return false;
            }
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
        public bool validateHeader(string[] header, string[] commonHeader)
        {
          
            // check if all of the first common question is identical
            var inputCommonHeader = header.Take(commonHeader.Length).ToList();
            bool isCorrectRequireQuestion = header.Take(commonHeader.Length).Any(h => commonHeader.Any(q => q.StartsWith(h)));
            if (!isCorrectRequireQuestion)
            {
                return false;
            }

            //check if the last question is sentiment question
            if (header[^1].Trim().ToLower() != commonHeader[^1])
            {
                return false;
            }
            return true;
        }

        public bool validateCommonRow(Tuple<string, string?> response, List<Questions> commonQuestion)
        {
            var question = response.Item1.Trim().ToLower();
            string value = response.Item2.ToString();
            var questionObject = commonQuestion.FirstOrDefault(q => q.Question.ToLower() == question);
            if (questionObject != null)
            {
                if (questionObject.Type == "time")
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
                else if (questionObject.Type == "email")
                {
                    if (value.Contains("@"))
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else if (questionObject.Type == "number")
                {
                    // validate int
                    int outputvalue;
                    if (int.TryParse(value, out outputvalue))
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else if (questionObject.Type == "yes/no")
                {
                    if (value == "Có" || value == "Không")
                    {
                        return true;
                    }
                }
                else
                {
                    return true;
                }
            }
            return true;
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
