using DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.IRepository
{
    public interface IRepositoryTests
    {
        public List<Test> GetTestByWorkshopId(int? workshopId);
        public int InsertTest(Test test);
    }
}
