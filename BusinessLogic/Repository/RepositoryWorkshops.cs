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

        public List<Workshop> GetWorkshopBySeriesWorkshopId(int? seriesWorkshopId)
        {
            try
            {
                return _context.Workshops.Include(x => x.Presenter).Include(x => x.Status).Where(x => x.WorkshopSeriesId == seriesWorkshopId).ToList();
            }
            catch (Exception)
            {
                throw;
            }
        }
        public Workshop GetWorkshopBySeriesWorkshopIdAndWorkshopName(int? seriesWorkshopId, string workshopName)
        {
            try
            {
                return _context.Workshops.FirstOrDefault(x => x.WorkshopSeriesId == seriesWorkshopId && x.WorkshopName.Equals(workshopName));
            }
            catch (Exception)
            {

                throw;
            }
        }

        public int UpdateDatePresent(Workshop workshop)
        {
            try
            {
                _context.Workshops.Update(workshop);
                return _context.SaveChanges();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public int InsertWorkshop(Workshop workshop)
        {
            try
            {
                _context.Workshops.Add(workshop);
                return _context.SaveChanges();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void UpdateStaus(string invitationCode, int status)
        {
            var workShop = _context.Workshops.FirstOrDefault(x => x.KeyPresenter == invitationCode);
            try
            {
                if (workShop != null)
                {
                    workShop.StatusId = status;
                    _context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            
        }
    }
}
