using BusinessLogic.IRepository;
using BusinessLogic.Repository;
using DataAccess.Constants;
using DataAccess.DTO;
using DataAccess.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;

namespace COTSEClient.Pages.Survey
{
    [Authorize(Roles = COTSEConstants.ROLE_RESEARCHER)]
    [Authorize(Roles = COTSEConstants.ROLE_ORGANIZER)]
    public class SurveyListModel : PageModel
    {

        private readonly IRepositorySurvey _repositorySurvey;
        private readonly IRepositoryAssign _repositoryAssign;
        public SurveyListModel(IRepositorySurvey repositorySurvey, IRepositoryAssign repositoryAssign)
        {
            _repositorySurvey = repositorySurvey;
            _repositoryAssign = repositoryAssign;
        }

        [BindProperty]
        public List<WorkshopSurveyDTO> list_data { get; set; } = null!;

        [BindProperty]
        public int wssId { get; set; }

        [BindProperty]
        public int wsId { get; set; }

        [BindProperty]
        public int surveyId { get; set; }
        [BindProperty]
        public List<Assign> AssignList { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            //list_data = await _repositorySurvey.surveyList();
            return Page();
        }


        public async Task<IActionResult> OnGetListAsync()
        {
            var user = HttpContext.User;
            var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim != null)
            {
                string userId = userIdClaim.Value;
                AssignList = _repositoryAssign.GetListSeriesWsByUserId(Convert.ToInt32(userId));
            }
            return new JsonResult(await _repositorySurvey.surveyList(AssignList));
        }

        
        public async Task<IActionResult> OnPostDeleteSurveyAsync()
        {
            var state = await _repositorySurvey.deleteSurvey(wssId, wsId, surveyId);
            var result = new { result = state };
            return new JsonResult(result);
        }
    }
}
