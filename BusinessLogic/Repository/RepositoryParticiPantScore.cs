using BusinessLogic.IRepository;
using COTSEClient.Models;
using DataAccess.Models;
using Microsoft.EntityFrameworkCore;
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

        public List<ParticiPantScore> GetParticiPantScoreByTestId(int testId)
        {
            try
            {
                return _context.ParticiPantScores.Include(x => x.Participant).Include(x => x.Test).Where(x => x.TestId == testId).ToList();
            }
            catch (Exception)
            {
                throw;
            }
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
