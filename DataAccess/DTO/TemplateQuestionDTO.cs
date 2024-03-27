using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.DTO
{
    public class TemplateQuestionDTO
    {
       public string? QuestionText { get; set; }
       public string? Answer_text { get; set; }
       public string? Is_correct_answer { get; set; }
    }
}
