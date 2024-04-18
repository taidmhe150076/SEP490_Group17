using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.DTO
{
    public class WorkshopInfoDTO
    {
        public int? wssId { get; set; }
        public string? SeriesName { get; set; }
        public int? wsId { get; set; }
        public string? WorkshopName { get; set; }
        public string? url { get; set; }
    }
}
