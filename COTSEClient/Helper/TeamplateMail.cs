using DataAccess.DTO;
using System.Text;

namespace COTSEClient.Helper
{
    public class TeamplateMail
    {
        public static string TeamplateMailPresenter(TeamplatePresenter teamplatePresenter)
        {
            string html = $@"
                            <!DOCTYPE html>
                            <html>
                            <head>
                                <title>Mời tham dự diễn giả workshop</title>
                            </head>

                            <body>
                                <h1>Chào bạn,</h1>
                                <p>Chúng tôi mời bạn tham gia với vai trò làm diễn giả cho workshop của chúng tôi.</p>
                                <p>Với Chủ Đề là : {teamplatePresenter.WorkshopName}</p>
                                <p>Thời gian: {teamplatePresenter.TimePresenter}</p>
                                <p>Địa điểm: Lab318 FPT </p>
                                <p>Link Room: {teamplatePresenter.UrlRoom}</p>

                                <p>Vui lòng xác nhận tham dự bằng cách vào Link phía dưới và đăng nhập mã khách khán giả để xác nhận và Import
                                    Document cho buổi workshop gồm bộ câu hỏi và Link slide.</p>
                                <p>Mã Khách Mời: {teamplatePresenter.KeyPresent}</p>
                                <p>Link Comfirm và Submit : {teamplatePresenter.LinkCF}</p>
                                <p>Link Tải Template Question : {teamplatePresenter.LinkTemplateQuestion}</p>
                                <p>Trân trọng,</p>
                                <p>Phòng Nghiên Cứu Lab318 FPT</p>
                            </body>

                            </html>";
            return html;
        }

        public static string TeamplateMailResearch(TemplateMailResearcher templateMailResearcher)
        {
            StringBuilder htmlBuilder = new StringBuilder();
            htmlBuilder.Append($@"
            <!DOCTYPE html>
            <html>
                <head>
                    <title>Thư Mời Làm Researcher Series {templateMailResearcher.WorkshopSeriesName}</title>
                </head>

                <body>
                    <h1>Chào bạn,</h1>
                    <p>Chúng tôi mời bạn tham gia với vai trò làm researcher cho series {templateMailResearcher.WorkshopSeriesName}.</p>
                    <p>Thời gian: {templateMailResearcher.TimeStart}</p>
                    <p>Infomation WorkShops: </p>");

            foreach (var workshopInfo in templateMailResearcher.WorkshopInformation)
            {
                htmlBuilder.AppendLine($"<p>WorkshopName: {workshopInfo.WorkshopName} - Workshop Key {workshopInfo.WorkshopKey}</p>");
            }
            htmlBuilder.Append($@"
                    <p>Link Room: {templateMailResearcher.UrlRoom}</p>
                    <p>Link Download chuỗi công cụ đánh giá : {templateMailResearcher.UrlDownLoadTool}</p>
                    <p>Link Download Template cho chuỗi công cụ đánh giá : {templateMailResearcher.UrlTemplate}</p>
                    <p>Đăng nhập vào website để tiến hành nghiên cứu: {templateMailResearcher.UrlWebLogin}</p>
                    <p>Trân trọng,</p>
                    <p>Phòng Nghiên Cứu Lab318 FPT</p>
                </body>
            </html>");

            return htmlBuilder.ToString();
        }
    }
}
