using BusinessLogic.IRepository;
using BusinessLogic.Repository;
using COTSEClient.Hubs;
using COTSEClient.Pages.Quizzes;
using Microsoft.AspNetCore.SignalR;

namespace COTSEClient.Helper
{
    public class AddServiceCollectionDI
    {
        public static void AddScopeServiceCollectionDI(IServiceCollection services)
        {
            services.AddScoped<IRepositoryParticipants, RepositoryParticipants>();
            services.AddScoped<IRepositoryWorkshops, RepositoryWorkshops>();
            services.AddScoped<IRepositoryTests, RepositoryTests>();
            services.AddScoped<IRepositoryWorkshopQuestions, RepositoryWorkshopQuestions>();
            services.AddScoped<IRepositoryParticipantAnswer, RepositoryParticipantAnswer>();
            services.AddScoped<IRepositoryParticiPantScore, RepositoryParticiPantScore>();
            services.AddScoped<IRepositoryWorkshopSeries, RepositoryWorkshopSeries>();
            services.AddScoped<IRepositoryPresenter, RepositoryPresenter>();
            services.AddScoped<IRepositoryTestType, RepositoryTestType>();
            services.AddScoped<IRepositoryAnswerQuestion, RepositoryAnswerQuestion>();
            services.AddScoped<IRepositorySurvey, RepositorySurvey>();
            services.AddScoped<IRepositoryGoogle, RepositoryGoogle>();
            services.AddScoped<IRepositoryAnswerParticipants, RepositoryAnswerParticipants>();
            services.AddScoped<IRepositoryUser, RepositoryUser>();
            services.AddScoped<IRepositoryUrlForm, RepositoryUrlForm>();
        }
    }
}
