using BusinessLogic.IRepository;
using DataAccess.Constants;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Sheets.v4;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Repository
{
    public class RepositoryGoogle : IRepositoryGoogle
    {
        private readonly string? googleClientId = Environment.GetEnvironmentVariable("googleClientId");
        private readonly string? googleClientSecret = Environment.GetEnvironmentVariable("googleClientSecret");

        //get goole api data
        public Task GoogleSheetApi()
        {
            Login();
            return null;
        }


        private UserCredential Login()
        {
            string[] scopes = new[] { SheetsService.Scope.Spreadsheets };
            ClientSecrets secret = new ClientSecrets()
            {
                ClientId = googleClientId,
                ClientSecret = googleClientSecret,
            };
            try
            {
                Console.WriteLine("login successful");
                return GoogleWebAuthorizationBroker.AuthorizeAsync(secret, scopes, "user", CancellationToken.None).Result;
            }
            catch (Exception)
            {
                throw new Exception(SurveyErrorMessage.ERR_GOOGLE_API_CALL);
            }
        }

    }
}
