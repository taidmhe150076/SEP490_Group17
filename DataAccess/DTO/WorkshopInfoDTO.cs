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
        public string? FormUrl { get; set; }
        public string? surveyName { get; set; }
        public bool? isPresenter { get; set; }
        public string? url { get; set; }
        public int? survey_id { get; set; }
        public string? fileType { get; set; }
    }
}
