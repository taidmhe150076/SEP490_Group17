using BusinessLogic.IRepository;
using COTSEClient.Helper;
using DataAccess.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;

namespace COTSEClient.Pages.User
{
    public class ListAllUserModel : PageModel
    {
        private readonly Sep490G17DbContext _context;
        private readonly IRepositoryUser _repositoryUser;
        [BindProperty]
        public int? RoleId { get; set; }
        [BindProperty]
        public int? DepartmentId { get; set; }
        [BindProperty]
        public List<DataAccess.Models.SystemUser> ListUsers { get; set; }
        [BindProperty]
        public List<DataAccess.Models.Department> Departments { get; set; }
        [BindProperty]
        public List<DataAccess.Models.SystemRole> Roles { get; set; }
        [BindProperty]
        public string email { get; set; }
        [BindProperty]
        public string password { get; set; }
        [BindProperty]
        public DateOnly ValidDate { get; set; }
        [BindProperty]
        public SystemUser User { get; set; }
        [BindProperty]
        public string Msg { get; set; }

        public PageList<SystemUser> SystemUserPage { get; set; }

        public ListAllUserModel(Sep490G17DbContext context,IRepositoryUser repositoryUser)
        {
            _repositoryUser = repositoryUser;
            _context = context;
        }
        public void OnGet(int pageIndex = 1 , int pageSize = 9 )
        {
            Msg = TempData["Msg"] as string;

            var result  = _repositoryUser.getAllUser().AsQueryable();

            SystemUserPage = PageList<SystemUser>.Create(result, pageIndex, pageSize);
            
            Departments = _context.Departments.ToList();
            Roles = _context.SystemRoles.ToList(); 
            
        }

        public void OnPost(string searchInput , int pageIndex = 1, int pageSize = 9)
        {
            var result = _repositoryUser.getAllUser().AsQueryable();

            if (!string.IsNullOrEmpty(searchInput))
            {
                ListUsers = ListUsers.Where(x => x.DepartmentldNavigation.DepartmentName.Contains(searchInput)).ToList();
            }

            SystemUserPage = PageList<SystemUser>.Create(result, pageIndex, pageSize);
        }

        public IActionResult OnPostCreateNewUser(DataAccess.Models.SystemUser user)
        {
            string hashPassword = HelperMethods.GenerateSecretKey(password, 32);
            user.Email = email;
            user.Password = hashPassword;
            user.ValidDate = ValidDate;
            user.Departmentld = DepartmentId;
            user.Roleld = RoleId;
            var checkExistEmail = _context.SystemUsers.FirstOrDefault(x => x.Email == email);
            if (checkExistEmail == null)
            {
                _repositoryUser.addUser(user);
                TempData["Msg"] = "Add Success!";
                return RedirectToPage();
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Email is existed");
                TempData["Msg"] = "Email is existed";
                return RedirectToPage();
            }                  
        }

        public IActionResult OnPostUpdateUser()
        {
            var checkExist = _repositoryUser.getUserById(User.Id);
            if (checkExist == null)
            {
                TempData["Msg"] = "User Not Exists!";
                return RedirectToPage();
            }
            checkExist.Email = User.Email;
            checkExist.ValidDate = User.ValidDate;
            checkExist.Departmentld = User.Departmentld;
            checkExist.Roleld = User.Roleld;

            _repositoryUser.updateUser(checkExist);
            TempData["Msg"] = "Update Success!";
            return RedirectToPage();
        }
       

        public async Task<IActionResult> OnPostBanUser(int userId)
        {
            var user = await _context.SystemUsers.FindAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            user.IsActive = false;
            await _context.SaveChangesAsync();
            TempData["Msg"] = "User is baned";
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostActiveUser(int userId)
        {
            var user = await _context.SystemUsers.FindAsync(userId);
            if(user == null)
            {
                return NotFound();
            }
            user.IsActive = true;
            await _context.SaveChangesAsync();
            TempData["Msg"] = "User is Active Success";
            return RedirectToPage();
        }

    }
}
