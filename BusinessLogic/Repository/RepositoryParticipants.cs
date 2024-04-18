using BusinessLogic.IRepository;
using DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Repository
{
    public class RepositoryParticipants : IRepositoryParticipants
    {
        private readonly Sep490G17DbContext _context;
        public RepositoryParticipants(Sep490G17DbContext context)
        {
            _context = context;
        }

        public List<Participant> GetParticipants()
        {
            try
            {
                return _context.Participants.OrderBy(x => x.TimeStamp).ToList();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<Participant> GetParticipantsOrderBy()
        {
            return _context.Participants.OrderBy(x => x.TimeStamp).ToList();
        }

        public int InsertRange(List<Participant> listParticipants)
        {
            try
            {
                _context.Participants.AddRange(listParticipants);
                return _context.SaveChanges();
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
