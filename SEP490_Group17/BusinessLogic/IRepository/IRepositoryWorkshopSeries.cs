using DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.IRepository
{
    public interface IRepositoryWorkshopSeries
    {
        public int InsertWorkshopSeries(WorkshopSeries workshopSeries);

        public List<WorkshopSeries> GetAllWorkshopSeries();
    }
}
