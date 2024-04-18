using BusinessLogic.IRepository;
using DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Repository
{
    public class RepositoryParticipantAnswer : IRepositoryParticipantAnswer
    {
		private readonly Sep490G17DbContext _context;
		public RepositoryParticipantAnswer(Sep490G17DbContext context)
        {
            _context = context;
        }

        public int InsertParticipantAnswer(ParticipantAnswer participantAnswer)
        {
			try
			{
                _context.ParticipantAnswers.Add(participantAnswer);
                return _context.SaveChanges();
            }
			catch (Exception)
			{
				throw;
			}
        }
    }
}
