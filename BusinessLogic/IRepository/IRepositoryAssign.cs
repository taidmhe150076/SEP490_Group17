using DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.IRepository
{
    public interface IRepositoryAssign
    {
        public int InsertAssignResearch(Assign assign);
        public List<Assign> GetListSeriesWsByUserId(int id);
        public int GetResearchIdBySwsId(int id);

    }
}
