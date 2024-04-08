﻿using BusinessLogic.IRepository;
using COTSEClient.Helper;
using DataAccess.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

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
        public ListAllUserModel(Sep490G17DbContext context,IRepositoryUser repositoryUser)
        {
            _repositoryUser = repositoryUser;
            _context = context;
        }
        public IActionResult OnGet(string departmentName)
        {
            ListUsers = _repositoryUser.getAllUser();
            if(!string.IsNullOrEmpty(departmentName))
            {
                ListUsers = ListUsers.Where(x => x.DepartmentldNavigation.DepartmentName.Contains(departmentName)).ToList();    
            }
            Departments = _context.Departments.ToList();
            Roles = _context.SystemRoles.ToList(); 
            //var systemUser = _repositoryUser.getUserById(Id);
            //User = systemUser;
            return Page();
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
                return RedirectToPage();
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Email is existed");
                ViewData["Error Message"] = "Email is existed";
                return RedirectToPage();
            }                  
        }

        public IActionResult OnPostUpdateUser(SystemUser user)
        {         
            _repositoryUser.updateUser(user);
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
            return RedirectToPage();
        }

    }
}