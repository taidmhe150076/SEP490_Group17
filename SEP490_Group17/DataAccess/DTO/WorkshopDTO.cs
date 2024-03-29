namespace COTSEClient.DTO
{
    public class WorkshopDTO
    {
        public int? Id { get; set; }    
        public string? WorkshopName { get; set; }

        public DateTime? DatePresent { get; set; }

        public int? WorkshopSeriesId { get; set; }

        public string? KeyPresenter { get; set; }
        public string? Email { get; set; }
    }
}
