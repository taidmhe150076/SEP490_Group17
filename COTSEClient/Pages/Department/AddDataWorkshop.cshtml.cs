using BusinessLogic.IRepository;
using BusinessLogic.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Cryptography.X509Certificates;

namespace COTSEClient.Pages.Department
{
    public class AddDataWorkshopModel : PageModel
    {
        private readonly IRepositoryParticipants _repositoryParticipants;

        public AddDataWorkshopModel(IRepositoryParticipants repositoryParticipants)
        {
            _repositoryParticipants = repositoryParticipants;
        }

        public void OnGet()
        {
        }
    }
}
