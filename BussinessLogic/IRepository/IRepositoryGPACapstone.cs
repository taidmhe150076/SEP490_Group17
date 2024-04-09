using LiveCharts.Defaults;
using LiveCharts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccess.Models;

namespace BusinessLogic.IRepository
{
    public interface IRepositoryGPACapstone
    {
        List<GPAScoreRange> GetValueGPACapstoneByMajorAndYear(string major, int year);
    }
}
