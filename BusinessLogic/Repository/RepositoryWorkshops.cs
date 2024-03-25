using BusinessLogic.IRepository;
using DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Repository
{
    public class RepositoryWorkshops : IRepositoryWorkshops
    {
        private readonly Sep490G17DbContext _context;

        public RepositoryWorkshops(Sep490G17DbContext context)
        {
            _context = context;
        }

        public List<Workshop> GetWorkshops()
        {
            return _context.Workshops.ToList();
        }
    }
}
