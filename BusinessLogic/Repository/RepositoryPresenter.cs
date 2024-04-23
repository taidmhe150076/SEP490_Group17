using BusinessLogic.IRepository;
using DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Repository
{
    public class RepositoryPresenter : IRepositoryPresenter
    {
        private readonly Sep490G17DbContext _context;
        public RepositoryPresenter(Sep490G17DbContext context)
        {
            _context = context;
        }

        public Presenter GetPresenterById(int id)
        {
            try
            {
                var findPresenter = _context.Presenters.FirstOrDefault(x => x.PresenterId == id);
                if (findPresenter == null)
                {
                    throw new Exception("Presenter Not Found");
                }
                return findPresenter;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public int InsertPresenter(Presenter presenter)
        {
            try
            {
                _context.Presenters.Add(presenter);
                return _context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public int UpdatePresenter(Presenter presenter)
        {
            try
            {
                _context.Presenters.Update(presenter);
                return _context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
