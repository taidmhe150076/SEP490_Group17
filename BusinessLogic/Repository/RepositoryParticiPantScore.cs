using BusinessLogic.IRepository;
using DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Repository
{
    public class RepositoryParticiPantScore : IRepositoryParticiPantScore
    {
        private readonly Sep490G17DbContext _context;
        public RepositoryParticiPantScore(Sep490G17DbContext context)
        {
            _context = context;
        }

        public int InsertParticiPantScore(ParticiPantScore particiPantScore)
        {
            try
            {
                _context.ParticiPantScores.Add(particiPantScore);
                return _context.SaveChanges();
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
