using BusinessLogic.IRepository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace COTSEClient.Pages.Presenters
{
    public class EnterKeyModel : PageModel
    {
        public readonly IRepositoryWorkshops _repositoryWorkshops;
        public EnterKeyModel(IRepositoryWorkshops repositoryWorkshops)
        {
            _repositoryWorkshops = repositoryWorkshops;
        }
        public IActionResult OnGet()
        {
            return Page();
        }
        public IActionResult OnPost(string invitationCode)
        {
            if (string.IsNullOrWhiteSpace(invitationCode))
            {
                return Page();
            }
            var checkExist = _repositoryWorkshops.GetWorkshopByKeyPresenter(invitationCode);
            if (checkExist != null)
            {
                return RedirectToPage("PresenterWorkshop", new { key = invitationCode });
            }
            return Page();
        }
    }
}
