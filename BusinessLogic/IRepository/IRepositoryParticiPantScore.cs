﻿using COTSEClient.Models;
using DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.IRepository
{
    public interface IRepositoryParticiPantScore
    {
        public int InsertParticiPantScore(ParticiPantScore particiPantScore);
        public List<ParticiPantScore> GetParticiPantScoreByTestId(int testId);
        bool DeleteScoresForTest(int testId);

    }
}
