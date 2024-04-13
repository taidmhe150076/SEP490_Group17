﻿using BusinessLogic.IRepository;
using DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Repository
{
    public class RepositorySlideWorkshop : IRepositorySlideWorkshop
    {
        private readonly Sep490G17DbContext _context;

        public RepositorySlideWorkshop(Sep490G17DbContext context)
        {
            _context = context;
        }
        public List<SlideWorkShop> GetAllSlideWorkshop(int wsId)
        {
            try
            {
                var slideworkshopList = _context.SlideWorkShops.Include(x => x.Image).Where(x => x.WorkshopId == wsId).ToList();
                return slideworkshopList;

            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}