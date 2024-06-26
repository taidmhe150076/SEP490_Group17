﻿using DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.IRepository
{
    public interface IRepositoryTests
    {
        public List<Test> GetTestByWorkshopId(int? workshopId);
        public int InsertTest(Test test);
        public int UpdateTest(Test test);
        public List<Test> GetScoresTestsByWorkshopId(int? workshopId);
        public string GetTestTypeByTestId(int testId);
        bool DeleteTest(int testId);
        Test GetTestById(int testId);
    }
}
