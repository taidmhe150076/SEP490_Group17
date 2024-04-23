using DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.IRepository
{
    public interface IRepositoryPresenter
    {
        public int InsertPresenter(Presenter presenter);
        public Presenter GetPresenterById(int id);
        public int UpdatePresenter(Presenter presenter);
    }
}
