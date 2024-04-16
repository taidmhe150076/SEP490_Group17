namespace COTSEClient.DTO
{
    public class SeriesWorkshopForm
    {
        public DateTime Timestamp { get; set; }
        public string? Email { get; set; }
        public string? NameParticipant { get; set; }
        public string? CourseNumber{ get; set; }
        public string? Major { get; set; }
        public string? FavoriteTopics { get; set; }
        public int? WorkshopSeriesId { get; set; }
    }
}
