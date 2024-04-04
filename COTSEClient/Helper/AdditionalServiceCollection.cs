using Amazon.S3;
using BusinessLogic.Validator;

namespace COTSEClient.Helper
{
    public class AdditionalServiceCollection
    {

        public static void AdditionalScope(WebApplicationBuilder builder) {
            builder.Services.AddDefaultAWSOptions(builder.Configuration.GetAWSOptions());
            builder.Services.AddAWSService<IAmazonS3>();

            builder.Services.AddScoped<SurveyValidator>();
        }
    }
}
