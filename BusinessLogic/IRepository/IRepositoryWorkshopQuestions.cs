﻿using DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.IRepository
{
    public interface IRepositoryWorkshopQuestions
    {
        public List<WorkshopQuestion> GetWorkshopQuestionsByWsId(int workShopId);
        public int InsertQuestion(WorkshopQuestion workshopQuestion);
        public void DeleteQuestion(int id);       
    }
}
