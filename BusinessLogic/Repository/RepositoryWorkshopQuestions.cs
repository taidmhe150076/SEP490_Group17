using BusinessLogic.IRepository;
using DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Repository
{
    public class RepositoryWorkshopQuestions : IRepositoryWorkshopQuestions
    {
        public List<WorkshopQuestion> GetWorkshopQuestionsByWsIdAndTestId()
        {
            throw new NotImplementedException();
        }
    }
}
