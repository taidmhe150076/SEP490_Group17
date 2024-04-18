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
        public WorkshopSeries CreateWorkshopSeries(WorkshopSeries workshopSeries);
        public List<WorkshopSeries> GetAllWorkshopSeries();

        public List<WorkshopSeries> SearchWorkshopSeriesByName(string workshopseriesName);

        public List<WorkshopSeries> GetWorkshopSeriesByDate(DateTime startDate , DateTime endDate);
    }
}
