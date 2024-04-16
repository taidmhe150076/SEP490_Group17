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
        public string? survey_form { get; set; } = null!;
        public string? isPresenter { get; set; }
        public string? survey_name { get; set; } = null!;
        public string? survey_path { get; set; } = null!;
        public DateTime? added_date { get; set; } = null!;
    }
}
