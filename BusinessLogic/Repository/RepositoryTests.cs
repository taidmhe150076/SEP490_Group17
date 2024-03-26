using BusinessLogic.IRepository;
using DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Repository
{
    public class RepositoryTests : IRepositoryTests
    {
        private readonly Sep490G17DbContext _context;
        public RepositoryTests(Sep490G17DbContext context)
        {
            _context = context;
        }

        public List<Test> GetTestByWorkshopId(int? workshopId)
        {
            try
            {
                if (workshopId == 0)
                {
                    throw new ArgumentNullException(nameof(workshopId));
                }
                return _context.Tests.Where(x => x.WorkshopId == workshopId).ToList();
            }
            catch (Exception)
            {
                throw;
            }
            
        }

        public int InsertTest(Test test)
        {
            try
            {
                _context.Tests.Add(test);
                return _context.SaveChanges();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public int UpdateTest(Test test)
        {
            try
            {
                _context.Tests.Update(test);
                return _context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
