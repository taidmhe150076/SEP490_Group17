using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Common
{
    public class SurveyConstant
    {
        public const int VALID_NAME_FORMAT = 1;
        public const int INVALID_NAME_FORMAT = 0;
        public const int INVALID_FILE_EXE = -1;
        public const int INVALID_WORKSHOP = 2;
        public const int INVALID_WORKSHOP_SERIES = 3;
        
        
        public const string REGREX_FILE_NAME = "[^a-zA-Z0-9]";
        public const string HUGGINGFACE_API_URL = "https://api-inference.huggingface.co/models/wonrax/phobert-base-vietnamese-sentiment";

        public const string REGREX_SPECIAL_CHARACTER = @"[^\w\s]";

        // remove later
        public const string googleClientId = "173806187355-dor09rj3ea26o8brte06q3v1at6jshsn.apps.googleusercontent.com";
        public const string googleClientSecret = "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";
        public const string HUGGING_FACE_TOKEN = "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";

    }
}
