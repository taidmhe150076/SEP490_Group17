using BusinessLogic.IRepository;
using DataAccess.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace COTSEClient.Pages.Presenters
{
    public class WorkshopQuestionModel : PageModel
    {
        private readonly Sep490G17DbContext _context;
        private readonly IRepositoryWorkshopQuestions _repository;

        public WorkshopQuestionModel(Sep490G17DbContext context, IRepositoryWorkshopQuestions repository)
        {
            _context = context;
            _repository = repository;
        }

        [BindProperty]
        public List<WorkshopQuestion> listWorkshopQuestion { get; set; }
        [BindProperty]
        public WorkshopQuestion WorkshopQuestion { get; set; }

        public IActionResult OnGet(int workshopId)
        {         
            listWorkshopQuestion = _repository.GetWorkshopQuestionsByWsId(workshopId);
            
            return Page();
        }
    }
}
