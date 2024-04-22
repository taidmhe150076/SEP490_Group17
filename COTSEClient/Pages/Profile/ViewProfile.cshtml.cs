using BusinessLogic.IRepository;
using BusinessLogic.Repository;
using COTSEClient.Helper;
using DataAccess.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace COTSEClient.Pages.Profile
{

    public class ViewProfileModel : PageModel
    {
        private readonly Sep490G17DbContext _context;
        private readonly IRepositoryUser _repository;
        public ViewProfileModel(Sep490G17DbContext context, IRepositoryUser repository)
        {
            _context = context;
            _repository = repository;
        }

        [BindProperty]
        public SystemUser User { get; set; }

        [BindProperty]
        public string OldPassword { get; set; }
        [BindProperty]
        public string NewPassword { get; set; }
        [BindProperty]       
        public string ConfirmNewPassword { get; set; }
        [BindProperty]
        public string Msg { get; set; }
        [BindProperty]
        public IFormFile ImageFile { get; set; }
        public  IActionResult OnGet()
        {
            Msg = TempData["Msg"] as string;

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

        public IActionResult OnPostChangePassword()
        {
            var user = _repository.getUserById(User.Id);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID");
            }
            var hashPassword = HelperMethods.GenerateSecretKey(OldPassword, 32);
            var getPasswordFromDB = user.Password.ToUpper();
            if (hashPassword != getPasswordFromDB)
            {
                ModelState.AddModelError("OldPassword", "Incorrect old password");
                TempData["Msg"] = "Old Password is wrong !";
                return RedirectToPage();
            }
            
            if(ConfirmNewPassword != NewPassword)
            {
                ModelState.AddModelError("Confirm password", "New Password and Confirm password not match");
                TempData["Msg"] = "New Password and ConfirmPassword not match !";
                return RedirectToPage();
            }
            var hashedNewPassword = HelperMethods.GenerateSecretKey(NewPassword, 32);
            
            if(hashedNewPassword == getPasswordFromDB)
            {
                ModelState.AddModelError("Confirm password", "New Password same old password");
                TempData["Msg"] = "New Password can not same Old Password!";
                return RedirectToPage();
            }

            try
            {
               
                _repository.updatePassword(user.Id, hashedNewPassword);
                TempData["Msg"] = "Change Password Success !";
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"An error occurred: {ex.Message}");
                return Page();
            }                                     
             return RedirectToPage();
        }

        public IActionResult OnPostChangeAvatar(IFormFile imageFile)
        {
            var user = _repository.getUserById(User.Id);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID {User.Id}.");
            }

            try
            {
                if (imageFile == null || imageFile.Length == 0)
                {
                    ModelState.AddModelError(string.Empty, "Please select a valid image file.");
                    return Page();
                }

                // Check if the uploaded file is an image
                if (!IsImage(imageFile))
                {
                    ModelState.AddModelError(string.Empty, "Please select a valid image file.");
                    return Page();
                }

                using (var memoryStream = new MemoryStream())
                {
                    imageFile.CopyTo(memoryStream);
                    // Convert image to base64 string
                    string base64Image = Convert.ToBase64String(memoryStream.ToArray());
                    // Update user's avatar
                    user.ImageUrl = base64Image;
                    // Save changes to the repository
                    _repository.updateUserAvatar(user.Id, base64Image);
                }

                return RedirectToPage();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"An error occurred while updating avatar: {ex.Message}");
                return Page();
            }
        }

        private bool IsImage(IFormFile file)
        {
            if (file == null)
            {
                return false;
            }

            string[] allowedImageTypes = { "image/jpeg", "image/png", "image/gif" };
            return allowedImageTypes.Contains(file.ContentType);
        }
    }
}
