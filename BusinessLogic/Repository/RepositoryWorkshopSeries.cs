using BusinessLogic.IRepository;
using DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Repository
{
    public class RepositoryWorkshopSeries : IRepositoryWorkshopSeries
    {
		private readonly Sep490G17DbContext _context;
		public RepositoryWorkshopSeries(Sep490G17DbContext context)
        {
            _context = context;
        }

        public List<WorkshopSeries> GetAllWorkshopSeries()
        {
            try
            {
                var workshopseriesList = _context.WorkshopSeries.ToList();
                return workshopseriesList;

            }
            catch (Exception) 
            {
                throw;
            }
            
        }

        public List<WorkshopSeries> GetWorkshopSeriesByDate(DateTime startDate, DateTime endDate)
        {
            try
            {
                var list = _context.WorkshopSeries.Where(ws => ws.StartDate >= startDate && ws.EndDate <= endDate).ToList();
                return list;

            }
            catch (Exception) 
            {
                throw;
            }
        }

        public int InsertWorkshopSeries(WorkshopSeries workshopSeries)
        {
			try
			{
                _context.WorkshopSeries.Add(workshopSeries);
                return _context.SaveChanges();
            }
			catch (Exception)
			{

				throw;
			}
        }

        public WorkshopSeries CreateWorkshopSeries(WorkshopSeries workshopSeries)
        {
            try
            {
                _context.WorkshopSeries.Add(workshopSeries);
                _context.SaveChanges();

                return workshopSeries;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<WorkshopSeries> SearchWorkshopSeriesByName(string workshopseriesName)
        {
            try
            {
                var searchResults = _context.WorkshopSeries.Where(ws => ws.WorkshopSeriesName.Contains(workshopseriesName)).ToList();
                return searchResults;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
