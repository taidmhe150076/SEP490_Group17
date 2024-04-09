using DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Repository
{
    public class RepositoryWorkshopQuestions : IRepositoryWorkshopQuestions
    {
        private readonly Sep490G17DbContext _context;
        public RepositoryWorkshopQuestions(Sep490G17DbContext context)
        {
            _context = context;
        }

        public List<WorkshopQuestion> GetWorkshopQuestionsByWsId(int workShopId)
        {
            try
            {
                return _context.WorkshopQuestions.Include(x => x.Workshop).Include(x => x.AnswerQuestions).Where(x => x.WorkshopId == workShopId).ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public int InsertQuestion(WorkshopQuestion workshopQuestion)
        {
            try
            {
                _context.WorkshopQuestions.Add(workshopQuestion);
                return _context.SaveChanges();
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
