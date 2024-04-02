using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.DTO
{
    public class SurveyDTO
    {
        public int Id { get; set; }
        public int wssId { get; set; }
        public int wsId { get; set; }
        public string? survey_name { get; set; } = null!;
        public string? survey_url { get; set; } = null!;
        public DateTime? added_date { get; set; } = null!;
    }
}
