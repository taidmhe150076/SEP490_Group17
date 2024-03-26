using COTSEClient.Helper;
using DataAccess.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;

namespace COTSEClient.Pages.Common
{
    public class LoginModel : PageModel
    {
        private readonly IConfiguration _configuration;
        private readonly Sep490G17DbContext _context;
        private readonly ILogger<LoginModel> _logger;
        

        public LoginModel(IConfiguration configuration,Sep490G17DbContext context , ILogger<LoginModel> logger )
        {
            _configuration = configuration;
            _context = context;
            _logger = logger;

        }

        [BindProperty]
        public string Username { get; set; }

        [BindProperty] 
        public string Password { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            string password = HelperMethods.GenerateSecretKey(Password, 32);
            var user = _context.Accounts.FirstOrDefault(x => x.UserName == Username && x.Password == password);

            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Invalid username or password");
                ViewData["ErrorMessage"] = "Invalid user or password";
                return Page();
            }

            var roles = _context.Accounts.Select(x => x.User.Role.Name).ToList();

            var claim = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, Username)
            };

            foreach(var role in roles)
            {
                claim.Add(new Claim(ClaimTypes.Role, role));
            }
            var claimIdentity = new ClaimsIdentity(claim , CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperty = new AuthenticationProperties
            {
                IsPersistent = true,
                ExpiresUtc = DateTime.UtcNow.AddHours(0.5),
                AllowRefresh = true,
                IssuedUtc = DateTime.UtcNow
            };
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimIdentity),authProperty);
            if (roles.Contains("Admin"))
            {
                return Redirect("/Dashboard");
            }else if(roles.Contains("Host")) {
                return Redirect("/Home");
            }
            else 
            {
                return Redirect("/Index");
            }
        }
    }
}
