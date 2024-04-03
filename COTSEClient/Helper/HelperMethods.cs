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
            var filePath = "wwwroot/Image/LogoTeam.png";
            QRCodeLogo qrCodeLogo = new QRCodeLogo(filePath);
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
        public static void SendMail(string body, string subject, string mailFrom, string passMail,string mailTo)
        {
            using (var client = new SmtpClient("smtp.office365.com"))
            {
                client.Port = 587;
                client.EnableSsl = true;
                client.Credentials = new NetworkCredential(mailFrom, passMail);

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(mailFrom),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true,
                };
                mailMessage.To.Add(mailTo);
                client.Send(mailMessage);
            }
        }
    }
}
