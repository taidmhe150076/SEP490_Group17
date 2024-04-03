namespace BusinessLogic.IRepository
{
    public interface IRepositoryAWS
    {
        Task<int> UploadDataToS3(string tmp_path, string file_name);
        Task GetS3Object(string key);
    }

    public interface IRepositoryGoogle
    {
        Task GoogleSheetApi();
    }
}
