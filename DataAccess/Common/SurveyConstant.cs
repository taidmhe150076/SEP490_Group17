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
        //common 
        public const int VALID_NAME_FORMAT = 1;
        public const int INVALID_NAME_FORMAT = 0;
        public const int INVALID_FILE_EXE = -1;
        public const int INVALID_WORKSHOP = 2;
        public const int INVALID_WORKSHOP_SERIES = 3;
        
        public const string REGREX_FILE_NAME = "[^a-zA-Z0-9]";
        public const string HUGGINGFACE_API_URL = "https://api-inference.huggingface.co/models/wonrax/phobert-base-vietnamese-sentiment";

        public const string REGREX_SPECIAL_CHARACTER = @"[^\w\s]";


        public const string IS_PRESENTER_FORM = "presenter form";
        public const string IS_PATICIPANT_FORM = "participant form";

    }
}
