using BusinessLogic.IRepository;
using DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Repository
{
    public class RepositoryAnswerQuestion : IRepositoryAnswerQuestion
    {
        private readonly Sep490G17DbContext _context;
        public RepositoryAnswerQuestion(Sep490G17DbContext context)
        {
            _context = context;
        }
    
        public int InsertRangeQuestionAnwser(List<AnswerQuestion> answerQuestions)
        {
            try
            {
                _context.AnswerQuestions.AddRange(answerQuestions);
                return _context.SaveChanges();
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
