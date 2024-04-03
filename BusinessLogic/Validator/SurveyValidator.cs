using DataAccess.Common;
using DataAccess.Constants;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Validator
{
    public class SurveyValidator
    {
        private List<string> file_types = new List<string>() {
            "csv",
            "xlsx",
        };

        public bool validateUrlPath(string url)
        {
            string header = "https://docs.google.com/spreadsheets";
            if (!url.Contains(header))
            {
                return false;
            }
            return true;
        }

        public bool validateFile(string fileName)
        {
            var file_part = fileName.Split('.');
            if (!file_types.Contains(file_part[^1]))
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

    }
}
