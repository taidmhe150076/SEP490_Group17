using Amazon.S3;

namespace COTSEClient.Helper
{
    public class AdditionalServiceCollection
    {

        public static void AdditionalScope(WebApplicationBuilder builder) {
            builder.Services.AddDefaultAWSOptions(builder.Configuration.GetAWSOptions());
            builder.Services.AddAWSService<IAmazonS3>();


        }
    }
}
