using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.DTO
{
    public class ParticipantDTO
    {
        public int Id { get; set; }

        public string? FullName { get; set; }

        public string Email { get; set; } = null!;

        public int? WorkshopSeriesId { get; set; }

        public DateTime? TimeStamp { get; set; }

        public string? Major { get; set; }

        public string? FavoriteTopics { get; set; }
    }
}
