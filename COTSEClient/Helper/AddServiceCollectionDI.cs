using BusinessLogic.IRepository;
using BusinessLogic.Repository;

namespace COTSEClient.Helper
{
    public class AddServiceCollectionDI
    {
        public static void AddScopeServiceCollectionDI(IServiceCollection services)
        {
            services.AddScoped<IRepositoryParticipants, RepositoryParticipants>();
            services.AddScoped<IRepositoryWorkshops, RepositoryWorkshops>();
            services.AddScoped<IRepositoryTests, RepositoryTests>();
        }
    }
}
