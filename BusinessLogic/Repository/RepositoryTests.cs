using BusinessLogic.IRepository;
using DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Repository
{
    public class RepositoryTests : IRepositoryTests
    {
        private readonly Sep490G17DbContext _context;
        public RepositoryTests(Sep490G17DbContext context)
        {
            _context = context;
        }

        public List<Test> GetTestByWorkshopId(int? workshopId)
        {
            try
            {
                if (workshopId == 0)
                {
                    throw new ArgumentNullException(nameof(workshopId));
                }
                return _context.Tests.Include(x => x.TestType).Where(x => x.WorkshopId == workshopId).ToList();
            }
            catch (Exception)
            {
                throw;
            }

        }

        public async Task<WorkshopSurveyUrl> getSurvey(int wssId, int wsId, int surveyId)
        {
            try {
                var data = await _context.WorkshopSurveyUrls
                    .Include(survey => survey.UrlForms)
                    .ThenInclude(urlForm => urlForm.Workshop)
                    .ThenInclude(workshop => workshop.WorkshopSeries)
                    .Where(survey => survey.Id == surveyId && survey.UrlForms.Any(urlForm => urlForm.WorkshopId == wsId))
                    .SingleOrDefaultAsync();

                if (data == null) {
                    throw new NullReferenceException();
                }
                return data;

            } catch (Exception) {
                throw new Exception();
            }
        }

        public List<Test> GetScoresTestsByWorkshopId(int? workshopId)
        {
            try
            {
                return _context.Tests.Include(x => x.ParticiPantScores).ThenInclude(x => x.Participant).Where(x => x.WorkshopId == workshopId).ToList();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public int InsertTest(Test test)
        {
            try
            {
                _context.Tests.Add(test);
                return _context.SaveChanges();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public int UpdateTest(Test test)
        {
            try
            {
                _context.Tests.Update(test);
                return _context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
