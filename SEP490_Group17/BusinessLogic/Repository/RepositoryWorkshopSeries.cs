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
    }
}
