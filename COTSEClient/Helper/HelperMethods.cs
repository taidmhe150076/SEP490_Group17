using System.Drawing;
using System.Net.Mail;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using IronBarCode;

namespace COTSEClient.Helper
{
    public class HelperMethods
    {
        public static string GenerateQRCode(string content)
        {
            QRCodeLogo qrCodeLogo = new QRCodeLogo("C:\\Users\\DoanManhTai\\Documents\\SEP490_Group17\\COTSEClient\\wwwroot\\Image\\LogoTeam.png");
            GeneratedBarcode myQRCodeWithLogo = IronBarCode.QRCodeWriter.CreateQrCodeWithLogo(content, qrCodeLogo);
            myQRCodeWithLogo.ResizeTo(250, 250).SetMargins(10).ChangeBarCodeColor(Color.Black);
            return myQRCodeWithLogo.ToDataUrl();
        }
        public static string GenerateSecretKey(string input, int keyLength)
        {
            using (MD5 md5 = MD5.Create())
            {
                byte[] inputBytes = Encoding.UTF8.GetBytes(input);
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                StringBuilder stringBuilder = new StringBuilder();
                for (int i = 0; i < Math.Min(keyLength, hashBytes.Length); i++)
                {
                    stringBuilder.Append(hashBytes[i].ToString("X2"));
                }

                return stringBuilder.ToString();
            }
        }
        public static void SendMail(string workshopName, string time, string address, string urlRoom, string MKM, string linkCf, string mailTo)
        {
            using (var client = new SmtpClient("smtp.office365.com"))
            {
                client.Port = 587;
                client.EnableSsl = true;
                client.Credentials = new NetworkCredential("NgocAnhNT2010@outlook.com", "@Ngocanh2010");

                var mailMessage = new MailMessage
                {
                    From = new MailAddress("NgocAnhNT2010@outlook.com"),
                    Subject = "Happy Birthday!",
                    IsBodyHtml = true
                };
                string body = $"<html>\r\n\r\n<head>\r\n    <title>Mời tham dự diễn giả workshop</title>\r\n</head>\r\n\r\n<body>\r\n    <h1>Chào bạn,</h1>\r\n    <p>Chúng tôi mời bạn tham gia với vai trò làm diễn giả cho workshop của chúng tôi.</p>\r\n    <p>Với Chủ Đề là : {workshopName}</p>\r\n    <p>Thời gian: {time}</p>\r\n    <p>Địa điểm: Lab318 FPT </p>\r\n    <p>Link Room: {urlRoom}</p>\r\n\r\n    <p>Vui lòng xác nhận tham dự bằng cách vào Link phía dưới và đăng nhập mã khách khán giả để xác nhận và Import\r\n        Document cho buổi workshop gồm bộ câu hỏi và Link slide.</p>\r\n    <p>Mã Khách Mời: {MKM}</p>\r\n    <P>link Comfirm và Submit : {linkCf}</P>\r\n    <p>Trân trọng,</p>\r\n    <p>Phòng Nghiên Cứu Lab318 FPT</p>\r\n</body>\r\n\r\n</html>";
                mailMessage.Body = body;
                mailMessage.To.Add(mailTo);
                client.Send(mailMessage);
            }
        }
    }
}
