using COTSEClient.DTO;
using DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.DTO
{
    public class WorkshopSeriesWorkshop
    {
        public int Id { get; set; }
        public string? WorkshopSeriesName { get; set; }
        public DateTime? StartDate { get; set; }
        public List<WorkshopDTO>? workshops { get; set; }
    }
}
