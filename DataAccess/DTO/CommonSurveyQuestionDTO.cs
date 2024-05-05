using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.DTO
{
    public class QuestionsWrapper
    {
        public List<Questions> language { get; set; }
    }

    public class Questions
    {
        public string Question { get; set; }
        public string Type { get; set; }
    }
}
