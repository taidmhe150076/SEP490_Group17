using BusinessLogic.IRepository;
using DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Repository
{
    public class RepositoryTestType : IRepositoryTestType
    {
		private readonly Sep490G17DbContext _context;
		public RepositoryTestType(Sep490G17DbContext context)
        {
            _context = context;
        }

        public List<TestType> GetTypes()
        {
			try
			{
                return _context.TestTypes.ToList();
            }
			catch (Exception)
			{
				throw;
			}
        }
    }
}
