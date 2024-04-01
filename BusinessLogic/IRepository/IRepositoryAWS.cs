namespace BusinessLogic.IRepository
{
    public interface IRepositoryAWS
    {
        Task<string> UploadDataToS3(string tmp_path, string file_name);
        Task GetS3Object(string key);
    }
}
