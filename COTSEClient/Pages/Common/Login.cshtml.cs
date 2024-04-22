using COTSEClient.Helper;
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

namespace COTSEClient.Pages.Common
{
    public class LoginModel : PageModel
    {
        private readonly Sep490G17DbContext _context;
        private readonly IRepositoryUser _repository;
        
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

            return Redirect("/Users");
            //if (role.Contains())
            //{
            //    return Redirect("/Users");
            //}
            //else if (role.Contains("User"))
            //{
            //    return Redirect("/Home");
            //}
            //else
            //{
            //    return Redirect("/Index");
            //}
        }

    }
}
