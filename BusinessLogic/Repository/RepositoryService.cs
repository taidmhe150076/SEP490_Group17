using Amazon;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using BusinessLogic.IRepository;
using DataAccess.Constants;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Sheets.v4;

namespace BusinessLogic.Repository
{
    public class RepositoryAWS : IRepositoryAWS
    {
        private readonly string? access_key = Environment.GetEnvironmentVariable("awsAccessKey");

        private readonly string? secret_key = Environment.GetEnvironmentVariable("awsSecretKey");

        private readonly string bucket_name = "cotse-sep490";

        private readonly BasicAWSCredentials _aws_cred;
        private IAmazonS3 _client;
        private readonly RegionEndpoint region = RegionEndpoint.APSoutheast1;

        public string? Access_key => access_key;

        public string? Secret_key => secret_key;

        public RepositoryAWS()
        {
            try
            {
                _aws_cred = new BasicAWSCredentials(Access_key, Secret_key);
                _client = new AmazonS3Client(_aws_cred, region);
            }
            catch (Exception e)
            {
                throw new Exception("xDD");
            }
        }

        public async Task<int> UploadDataToS3(string tmp_path, string file_name)
        {
            try
            {
                PutObjectRequest req = new PutObjectRequest();
                req.BucketName = bucket_name;
                req.Key = file_name;
                req.FilePath = tmp_path;
                await _client.PutObjectAsync(req);
                return COTSEConstants.DB_STATUS_SUCCESS;
            }
            catch (Exception e)
            {
                throw new Exception("not working please help");
            }
        }

        public async Task GetS3Object(string key) {
            try
            {
                GetObjectRequest request = new GetObjectRequest {
                    BucketName = bucket_name,
                    Key= key
                };
                using (var response = await _client.GetObjectAsync(request)) {
                    using(var responseStream = response.ResponseStream)
                    {
                        await WriteToTemp(key, responseStream);
                    }
                }

            }
            catch (Exception) {
                throw new Exception("xDDD");
            }
        }

        private async Task<bool> WriteToTemp(string key_name, Stream responseStream) {
            string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "temp", key_name);
            using (var fileStream = File.Create(filePath)) {
                await responseStream.CopyToAsync(fileStream);
                return true;
            }
        }

        private async Task keeper(string tmp_path)
        {
            _client = new AmazonS3Client(_aws_cred, region);
            var tu = new TransferUtility(_client);
            try
            {
                var tu_request = new TransferUtilityUploadRequest
                {
                    BucketName = bucket_name,
                    FilePath = tmp_path,
                    StorageClass = S3StorageClass.Standard,
                    PartSize = 6291456, //6mb
                    Key = "test.csv",
                    CannedACL = S3CannedACL.PublicRead
                };
                await tu.UploadAsync(tu_request);
                tu.Dispose();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }
    }

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
