using BusinessLogic.IRepository;
using BusinessLogic.Repository;
using DataAccess.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;

namespace COTSEClient.Pages.Profile
{

    public class ViewProfileModel : PageModel
    {
        private readonly Sep490G17DbContext _context ;
        private readonly IRepositoryUser _repository;
        public ViewProfileModel(Sep490G17DbContext context, IRepositoryUser repository)
        {
            _context = context;
            _repository = repository;          
        }

        [BindProperty]
        public SystemUser User { get; set; }

        public  IActionResult OnGet()
        {
            if (!HttpContext.User.Identity.IsAuthenticated)
            {
                return RedirectToPage("Login");
            }

            var userIdClaim = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || string.IsNullOrEmpty(userIdClaim.Value))
            {
                return BadRequest("User ID not found in claims ");
            }

            if (!int.TryParse(userIdClaim.Value, out int userId))
            {
                return BadRequest("Invalid user ID format");
            }

            User =  _repository.getUserById(userId);
           
            if(User == null)
            {
                return NotFound();
            }
            return Page();
        }


        public IActionResult OnPostUpdateInformation()
        {
            var checkExist = _repository.getUserById(User.Id);
            if (checkExist == null)
            {
                TempData["Msg"] = "User Not Exists!";
                return RedirectToPage();
            }
            checkExist.FirstName = User.FirstName;
            checkExist.LastName = User.LastName;
            checkExist.Dob = User.Dob;
            checkExist.Email = User.Email;

            _repository.updateUser(checkExist);
            TempData["Msg"] = "Update Success!";
            return RedirectToPage();
        }
    }
}
