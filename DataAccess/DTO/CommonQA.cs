using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.DTO
{
    public class CommonQA
    {
        public string Question { get; set; }
        public Dictionary<string, int> Counts { get; set; }
    }
}
