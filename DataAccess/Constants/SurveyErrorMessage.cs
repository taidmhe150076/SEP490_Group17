using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Constants
{
    public class SurveyErrorMessage
    {

        public const string ERR_OUT_OF_RANGE = "input out of rannge!!";
        public const string ERR_FILENAMES_FORMAT = "files '{files}' not in the correct format";
        public const string ERR_LOAD_FILE = "file '{file_type}' is not supported!";
        public const string ERR_FILES_LOAD = "load files invalid!!";

        // uncommon
        public const string ERR_1 = "";
        public const string ERR_FILE_NAME = "file '{file_name}' is not in the right format";
        public const string ERR_SENTIMENT_COL = "the last col of the file '{file_name}' is not the right format";
        public const string ERR_TOKEN = "token invalid!!";
        public const string ERR_WS_NOT_FOUND = "workshop not found!";
        public const string ERR_WSS_NOT_FOUND = "series not found!";
        public const string ERR_ADD_WS = "failure while adding survey";

        public const string ERR_WS_URL = "surveys not found";
        public const string ERR_HF_API = "service not reach";
        public const string ERR_GOOGLE_API_CALL = "credential failure!";
    }
}
