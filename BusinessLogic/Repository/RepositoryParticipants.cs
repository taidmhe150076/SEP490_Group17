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
        public List<Participant> GetParticipantsOrderBy()
        {
            return _context.Participants.OrderBy(x => x.TimeStamp).ToList();
        }
    }
}
