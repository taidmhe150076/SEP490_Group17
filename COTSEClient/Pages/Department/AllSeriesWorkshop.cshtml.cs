using BusinessLogic.IRepository;
using COTSEClient.Helper;
using DataAccess.Constants;
using DataAccess.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;

namespace COTSEClient.Pages.Department
{
    [Authorize(Roles = COTSEConstants.ROLE_ORGANIZER)]
    public class AllSeriesWorkshopModel : PageModel
    {

        private readonly IRepositoryWorkshopSeries _repositoryWorkshopSeries;
        private readonly IRepositoryUser _repositoryUser;
        private readonly IRepositoryAssign _repositoryAssign;

        [BindProperty(SupportsGet = true)]
        public string SearchInput { get; set; }
        [BindProperty(SupportsGet = true)]
        public DateTime? CurentDate { get; set; }

        [BindProperty]
        public  IFormFile imageFile { get;set; }

        [BindProperty]
        public WorkshopSeries WorkshopSeries { get; set; }
  
        public PageList<WorkshopSeries> WorkshopSeriesPage { get; set; }

        [BindProperty]
        public int ResearchAssignId { get; set; }
        [BindProperty]
        public string Msg { get; set; }
        [BindProperty]
        public List<SystemUser> SystemUsers { get; set; }

        public AllSeriesWorkshopModel(IRepositoryWorkshopSeries repositoryWorkshopSeries, IRepositoryUser repositoryUser, IRepositoryAssign repositoryAssign)
        {
            _repositoryWorkshopSeries = repositoryWorkshopSeries;
            _repositoryUser = repositoryUser;
            _repositoryAssign = repositoryAssign;
        }

        public IActionResult OnGet( int pageIndex = 1, int pageSize = 9)
        {
            var user = HttpContext.User;
            var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim != null)
            {
                string userId = userIdClaim.Value;
                var checkExits = _repositoryUser.getUserById(Convert.ToInt32(userId));
                if (checkExits == null)
                {
                    Msg = "Username Not Exits In System";
                    return Page();
                }
                SystemUsers = _repositoryUser.getAllResearchByDepartmentId((int)checkExits.Departmentld);
            }
           

            var source = _repositoryWorkshopSeries.GetAllWorkshopSeries().AsQueryable();

             WorkshopSeriesPage = PageList<WorkshopSeries>.Create(source, pageIndex, pageSize);
            return Page();
        }

        public void OnPost( string searchInput, int pageIndex = 1, int pageSize = 9)
        {

            var source = _repositoryWorkshopSeries.GetAllWorkshopSeries().AsQueryable();

            if (!string.IsNullOrEmpty(searchInput))
            {
                source = source.Where(s => s.WorkshopSeriesName.Contains(searchInput, StringComparison.OrdinalIgnoreCase));
            }

            WorkshopSeriesPage = PageList<WorkshopSeries>.Create(source, pageIndex, pageSize);

        }

        public IActionResult OnPostCreateWorkshopSeries(WorkshopSeries workshopSeries )
        {
            try
            {
                if (imageFile != null && imageFile.Length > 0)
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        imageFile.CopyTo(memoryStream);
                 
                        workshopSeries.Image = Convert.ToBase64String(memoryStream.ToArray());
                    }
                }
                
                WorkshopSeries createdWorkshopSeries = _repositoryWorkshopSeries.CreateWorkshopSeries(workshopSeries);
                if (createdWorkshopSeries != null)
                {
                    Assign newAssign = new Assign
                    {
                        UserSystemId = ResearchAssignId,
                        WorkshopSeriesId = workshopSeries.Id,
                    };
                    var resultInsert = _repositoryAssign.InsertAssignResearch(newAssign);

                    if (resultInsert > 0)
                    {
                        return RedirectToPage("AddNewSeries", new { seriesWorkshopId = createdWorkshopSeries.Id });
                    }
                }
            }
            catch (Exception ex)
            {
                Msg = "Add SeriesWorkshop Error";
                return Page();
            }
            Msg = "Add SeriesWorkshop Error";
            return Page();
        }
        public IActionResult OnPostUpdateWorkshopSeries()
        {
            try
            {
                var workshopSeries = _repositoryWorkshopSeries.GetWorkshopSeriesById(WorkshopSeries.Id);

                if (workshopSeries == null)
                {
                    TempData["Msg"] = "User Not Exists!";
                    return RedirectToPage();
                }

                workshopSeries.WorkshopSeriesName = WorkshopSeries.WorkshopSeriesName;
                workshopSeries.Description = WorkshopSeries.Description;
                workshopSeries.StartDate = WorkshopSeries.StartDate;
                workshopSeries.EndDate = WorkshopSeries.EndDate;
                if (imageFile != null && imageFile.Length > 0)
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        imageFile.CopyTo(memoryStream);

                        workshopSeries.Image = Convert.ToBase64String(memoryStream.ToArray());
                    }
                }
                _repositoryWorkshopSeries.UpdateWorkshopSeries(workshopSeries);
                TempData["Msg"] = "Update Success!";
            }
            catch (Exception ex)
            {
                TempData["Msg"] = "Update Success!";
                return RedirectToPage();
                throw;
            }
            return RedirectToPage();
        }
    }
}
