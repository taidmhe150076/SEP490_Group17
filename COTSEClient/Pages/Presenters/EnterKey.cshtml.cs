using BusinessLogic.IRepository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Runtime.CompilerServices;

namespace COTSEClient.Pages.Presenters
{
    public class EnterKeyModel : PageModel
    {
        [BindProperty]
        public bool Flag { get; set; } = false;
        [BindProperty]
        public string InvitationCode { get; set; }
        public readonly IRepositoryWorkshops _repositoryWorkshops;
        public EnterKeyModel(IRepositoryWorkshops repositoryWorkshops)
        {
            _repositoryWorkshops = repositoryWorkshops;
        }
        public IActionResult OnGet(string invitationCode, bool checkCf)
        {
            if (invitationCode != null && checkCf != false)
            {
                _repositoryWorkshops.UpdateStaus(invitationCode, 3);
                return RedirectToPage("PresenterWorkshop", new { key = invitationCode });
            }
            return Page();
        }

        public IActionResult OnPost(string invitationCode, bool checkCf)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(invitationCode))
                {
                    return Page();
                }
                var checkExist = _repositoryWorkshops.GetWorkshopByKeyPresenter(invitationCode);
                
                if (checkExist != null && checkExist.StatusId == 2)
                {
                    Flag = true;
                    InvitationCode = invitationCode;
                    return Page();
                }
                else
                {
                    return RedirectToPage("PresenterWorkshop", new { key = invitationCode });
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
                      
        }


    }
}
