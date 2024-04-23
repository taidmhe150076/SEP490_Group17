using DataAccess.DTO;

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
        public List<SurveyDTO> Survey { get; set; }
    }
    public class WorkshopUpdateDTO
    {
        public int? Id { get; set; }
        public string? WorkshopName { get; set; }
        public DateTime? DatePresent { get; set; }
        public string? Email { get; set; }
        public int? PresenterIdOld { get; set; }
    }
}
