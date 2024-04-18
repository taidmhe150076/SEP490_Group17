using DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.IRepository
{
    public interface IRepositoryWorkshops
    {
        public List<Workshop> GetWorkshops();
        public Workshop GetWorkshopByKeyPresenter(string invitationCode);
        public List<Workshop> GetParticiPantScoresByWorkshopId(int? workshopId);
        public Workshop GetWorkshopByWorkshopId(int? workshopId);
        public Workshop GetWorkshopBySeriesWorkshopIdAndWorkshopName(int? seriesWorkshopId, string workshopName);

        public List<Workshop> GetWorkshopBySeriesWorkshopId(int? seriesWorkshopId);
        public int UpdateDatePresent(Workshop workshop);
        public int InsertWorkshop(Workshop workshop);

    }
}
