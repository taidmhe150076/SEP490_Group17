﻿using DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.IRepository
{
    public interface IRepositorySlideWorkshop
    {

        public List<ImagesWorkShop> GetAllSlideWorkshop(int wsId);
        public List<ImagesWorkShop> GetAllChartVideoWorkshop(int wsId);


    }
}
