﻿using COTSEClient.Helper;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data;
using System.Security.Claims;
using DataAccess.Models;
using BusinessLogic.IRepository;
using System.ComponentModel.DataAnnotations;
using static iTextSharp.text.pdf.AcroFields;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using DataAccess.Constants;
using Microsoft.AspNetCore.Http;

namespace COTSEClient.Pages.Common
{
    public class LoginModel : PageModel
    {
        private readonly Sep490G17DbContext _context;
        private readonly IRepositoryUser _repository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public LoginModel(Sep490G17DbContext context , IRepositoryUser repository)
        {
            _context = context;
            _repository = repository;
        }

        [BindProperty]
        [EmailAddress(ErrorMessage = "Invalid primary email address")]
        public string Email { get; set; }
        [BindProperty]
        public string Password { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError(string.Empty, " username or password is empty .");
                return Page();
            }
            string hashPassword = HelperMethods.GenerateSecretKey(Password, 32);
            var user = _repository.getByEmailAndPassword(Email,hashPassword);

            if (user == null || user.IsActive == false || user.IsActive == null)
            {
                ModelState.AddModelError(string.Empty, "invalid username or password.");
                ViewData["ErrorMessage"] = "Invalid username or password";
                return Page();
            }

            var role = _context.SystemUsers.Include(x => x.RoleldNavigation).Where(x => x.Id == user.Id).ToList();

            var claims = new List<Claim>
            {
                 new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                 new Claim(ClaimTypes.Name, Email),
            };
            foreach (var item in role)
            {
                if (item != null)
                {
                    claims.Add(new Claim(ClaimTypes.Role, item.RoleldNavigation.RoleName));
                    TempData["UserRole"] = item.RoleldNavigation.RoleName;
                    HttpContext.Session.SetString("Role", item.RoleldNavigation.RoleName);
                }
            }

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperty = new AuthenticationProperties
            {
                IsPersistent = true,
                ExpiresUtc = DateTime.UtcNow.AddHours(0.2),
                AllowRefresh = true,
                IssuedUtc = DateTime.UtcNow
            };          

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperty);

            if (role.Any(x => x.RoleldNavigation.RoleName == COTSEConstants.ROLE_ORGANIZER))
            {
                return Redirect("/AllSeries");
            }
            else if (role.Any(x => x.RoleldNavigation.RoleName == COTSEConstants.ROLE_ADMIN))
            {
                return Redirect("/DashBoard");
            }
            else if (role.Any(x => x.RoleldNavigation.RoleName == COTSEConstants.ROLE_RESEARCHER))
            {
                return Redirect("/Surveys/All");
            }
            else if (role.Any(x => x.RoleldNavigation.RoleName == COTSEConstants.ROLE_STAFF))
            {
                return Redirect("/PresenterWorkshop");
            }
            ViewData["ErrorMessage"] = "Error";
            return Page();
            
        }

    }
}
