using BusinessLogic.IRepository;
using DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Repository
{
    public class RepositoryAssign : IRepositoryAssign
    {
		public readonly Sep490G17DbContext _context;
		public RepositoryAssign(Sep490G17DbContext context)
        {
            _context = context;
        }

        public List<Assign> GetListSeriesWsByUserId(int id)
        {
			try
			{
                if (id == 0)
                {
                    throw new ArgumentException("UserSystemId Invali", nameof(id));
                }
                return _context.Assigns.Where(x => x.UserSystemId == id).ToList();
			}
			catch (Exception)
			{
				throw;
			}
        }

        public int InsertAssignResearch(Assign assign)
        {
			try
			{
                _context.Assigns.Add(assign);
                return _context.SaveChanges();
			}
			catch (Exception)
			{
				throw;
			}
        }
    }
}
