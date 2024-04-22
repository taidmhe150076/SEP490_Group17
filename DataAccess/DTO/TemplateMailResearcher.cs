using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.DTO
{
    public class TemplateMailResearcher
    {
        public string? WorkshopSeriesName {  get; set; }
        public string? TimeStart { get; set; }
        public List<WorkshopInformation>? WorkshopInformation {  get; set; }
        public string? UrlRoom {  get; set; }
        public string? UrlDownLoadTool {  get; set; }
        public string? UrlWebLogin {  get; set; }
    }
    public class WorkshopInformation
    {
        public string? WorkshopName { get; set; }
        public string? WorkshopKey { get; set; }
    }
}
