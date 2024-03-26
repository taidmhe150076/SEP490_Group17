using BusinessLogic.IRepository;
using DataAccess.Models;
using Microsoft.EntityFrameworkCore;
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
        public Workshop GetWorkshopByKeyPresenter(string invitationCode)
        {
            return _context.Workshops.FirstOrDefault(x => x.KeyPresenter.Equals(invitationCode.Trim()));
        }

        public List<Workshop> GetParticiPantScoresByWorkshopId(int? workshopId)
        {
            try
            {
                return _context.Workshops.Include(x => x.Tests).ThenInclude(x => x.ParticiPantScores).Where(x => x.Id == workshopId).ToList();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public Workshop GetWorkshopByWorkshopId(int? workshopId)
        {
            try
            {
                return _context.Workshops.FirstOrDefault(x => x.Id == workshopId);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
