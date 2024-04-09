using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.DTO
{
    public class SurveyContentDTO
    {
        public DateTime? timeStamp { get; set; } = null!;
        public string? AnswerBy { get; set; } = null!;
        public Dictionary<string, string> QA { get; set; } = null!;
        
    }
}
