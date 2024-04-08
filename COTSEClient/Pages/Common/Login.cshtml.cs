using COTSEClient.Helper;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data;
using System.Security.Claims;
using DataAccess.Models;

namespace COTSEClient.Pages.Common
{
    public class LoginModel : PageModel
    {
        private readonly Sep490G17DbContext _context;
        
        public LoginModel(Sep490G17DbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public string Username { get; set; }
        [BindProperty] 
        public string Password { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            if(!ModelState.IsValid)
            {
                return Page();
            }
            string hashPassword = HelperMethods.GenerateSecretKey(Password, 32);
            var user = _context.SystemUsers.FirstOrDefault(x => x.Email == Username && x.Password == hashPassword);

            if (user == null || user.IsActive == false)
            {
                ModelState.AddModelError(string.Empty, "invalid username or password.");
                ViewData["Error Message"] = "Invalid username or password";
                return Page();
            }

            var role = _context.SystemUsers.Select(x => x.RoleldNavigation.RoleName).ToList();

            var claims = new List<Claim>
            {
                 new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                  new Claim(ClaimTypes.Name, Username),
            };

            foreach (var item in role)
            {
                claims.Add(new Claim(ClaimTypes.Role, item));
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

            if (role.Contains("Admin"))
            {
                return Redirect("/Home");
            }
            else if (role.Contains("Host"))
            {
                return Redirect("/Home");
            }
            else
            {
                return Redirect("/Index");
            }
        }

    }
}
