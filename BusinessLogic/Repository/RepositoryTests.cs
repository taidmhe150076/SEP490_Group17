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
        public Test GetTestById(int testId)
        {
            return _context.Tests.FirstOrDefault(t => t.Id == testId);
        }
        public List<Test> GetTestByWorkshopId(int? workshopId)
        {
            try
            {
                if (workshopId == 0)
                {
                    throw new ArgumentNullException(nameof(workshopId));
                }
                return _context.Tests.Include(x => x.TestType).Where(x => x.WorkshopId == workshopId).ToList();
            }
            catch (Exception)
            {
                throw;
            }
            
        }

        public List<Test> GetScoresTestsByWorkshopId(int? workshopId)
        {
            try
            {
                return _context.Tests.Include(x => x.ParticiPantScores).ThenInclude(x => x.Participant).Where(x => x.WorkshopId == workshopId).ToList();
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

        public string GetTestTypeByTestId(int testId)
        {
            try
            {
                return _context.Tests.Include(x => x.TestType).FirstOrDefault(x => x.Id == testId).TestType.TypeName;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public bool DeleteTest(int testId)
        {
            try
            {
                var testToDelete = _context.Tests.FirstOrDefault(t => t.Id == testId);
                if (testToDelete != null)
                {
                    _context.Tests.Remove(testToDelete);
                    _context.SaveChanges();
                    return true; // Trả về true nếu xóa thành công
                }
                return false; // Trả về false nếu không tìm thấy bài kiểm tra để xóa
            }
            catch (Exception ex)
            {
                // Xử lý lỗi nếu cần thiết
                return false; // Trả về false nếu có lỗi xảy ra khi xóa
            }
        }
    }
}
