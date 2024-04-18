using DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.IRepository
{
    public interface IRepositoryParticipants
    {
        public List<Participant> GetParticipantsOrderBy();
        public List<Participant> GetParticipants();
        public int InsertRange(List<Participant> listParticipants);
    }
}
