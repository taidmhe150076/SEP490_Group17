﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Constants
{
    public class COTSEConstants
    {
        public const int STATUS_DEFAULT = 1;
        public const int STATUS_PENDING = 2;
        public const int STATUS_ACCEPT = 3;
        public const string CORRECT_ANSWER = "1";
        public const int TEST_PRE = 1;
        public const int TEST_POST = 2;


        // survey
        public const int CSV_INPUT = 0;
        public const int EXCEL_INPUT = 1;
        public const int MODE_ADD_URL = 1;
        public const int MODE_ADD_FILE = 0;
        public const int DB_STATUS_FAIL = -1;
        public const int DB_STATUS_SUCCESS = 1;
        public const int DB_STATUS_EXIST = 999;
    }
}
