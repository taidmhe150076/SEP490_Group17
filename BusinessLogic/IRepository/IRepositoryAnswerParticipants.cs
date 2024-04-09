using BusinessLogic.Repository;
using DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.IRepository
{
    public interface IRepositoryAnswerParticipants
    {
        public int InsertRangeAnswerOfParticipants(List<AnswerParticipant> answerParticipants);
    }
}
