using BusinessLogic.IRepository;
using DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Repository
{
    public class RepositoryAnswerParticipants : IRepositoryAnswerParticipants
    {
        private readonly Sep490G17DbContext _context;
        public RepositoryAnswerParticipants(Sep490G17DbContext context)
        {
            _context = context;
        }

        public int InsertRangeAnswerOfParticipants(List<AnswerParticipant> answerParticipants)
        {
            try
            {
                _context.AnswerParticipants.AddRange(answerParticipants);
                return _context.SaveChanges();
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
