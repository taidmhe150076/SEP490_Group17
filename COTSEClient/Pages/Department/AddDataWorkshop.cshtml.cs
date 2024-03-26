using BusinessLogic.IRepository;
using BusinessLogic.Repository;
using DataAccess.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Cryptography.X509Certificates;

namespace COTSEClient.Pages.Department
{
    public class AddDataWorkshopModel : PageModel
    {
        private readonly IRepositoryParticipants _repositoryParticipants;
        private readonly IConfiguration _configuration;
        public AddDataWorkshopModel(IRepositoryParticipants repositoryParticipants, IConfiguration configuration)
        {
            _repositoryParticipants = repositoryParticipants;
            _configuration = configuration;
        }

        public void OnGet()
        {
        }
    }
}
