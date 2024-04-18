using BusinessLogic.IRepository;
using DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Repository
{
    public class RepositoryUrlForm : IRepositoryUrlForm
    {
        private readonly Sep490G17DbContext _context;
        public RepositoryUrlForm(Sep490G17DbContext context)
        {
            _context = context;
        }
        public string GetUrlFormForParticipantsByWsId(int wsId)
        {
			try
			{
                return _context.UrlForms.FirstOrDefault(x => x.WorkshopId == wsId && x.IsPresenter == false).UrlForm1;
			}
			catch (Exception)
			{
				throw;
			}
        }
    }
}
