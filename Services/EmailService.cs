using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using System.Threading.Tasks;
using BusManagementAPI.Models;
using System.Text.Json;
using System.Security.Cryptography;
using System.Text;

namespace BusManagementAPI.Services
{
    public class EmailService : IEmailService
    {
        private readonly EmailSettings _emailSettings;
        private readonly IConfiguration _configuration;

        public EmailService(IOptions<EmailSettings> emailSettings, IConfiguration configuration)
        {
            _emailSettings = emailSettings.Value;
            _configuration = configuration;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            var email = new MimeMessage();
            email.Sender = MailboxAddress.Parse(_emailSettings.FromEmail);
            email.From.Add(new MailboxAddress(_emailSettings.FromName, _emailSettings.FromEmail));
            email.To.Add(MailboxAddress.Parse(toEmail));
            email.Subject = subject;

            var builder = new BodyBuilder
            {
                HtmlBody = body,
                TextBody = StripHtml(body) // For plain text fallback
            };
            email.Body = builder.ToMessageBody();

            using var smtp = new SmtpClient();
            try
            {
                string key = _configuration["AppSettings:AESKEY"];
                string iv = _configuration["AppSettings:AESIV"];

                string encPassword = Decrypt(_emailSettings.SmtpPassword, key, iv);
                await smtp.ConnectAsync(_emailSettings.SmtpHost, _emailSettings.SmtpPort, SecureSocketOptions.SslOnConnect); // Or .StartTls for port 587
                await smtp.AuthenticateAsync(_emailSettings.SmtpUsername, encPassword);
                await smtp.SendAsync(email);
            }
            finally
            {
                await smtp.DisconnectAsync(true);
            }
        }

        private string StripHtml(string html)
        {
            // Simple HTML stripping for AltBody. For more complex HTML, consider a dedicated library or regex.
            return System.Text.RegularExpressions.Regex.Replace(html, "<.*?>", string.Empty);
        }

        private static string Decrypt(string encryptedBase64, string key, string iv)
        {
            byte[] cipherBytes = Convert.FromBase64String(encryptedBase64);
            byte[] keyBytes = Encoding.UTF8.GetBytes(key);
            byte[] ivBytes = Encoding.UTF8.GetBytes(iv);

            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = keyBytes;
                aesAlg.IV = ivBytes;
                aesAlg.Mode = CipherMode.CBC;
                aesAlg.Padding = PaddingMode.PKCS7;

                using (ICryptoTransform decryptor = aesAlg.CreateDecryptor())
                using (MemoryStream msDecrypt = new MemoryStream(cipherBytes))
                using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                {
                    return srDecrypt.ReadToEnd();
                }
            }
        }

        public string shortURLGeneration(string URL)
        {
            using var client = new HttpClient();
            var content = new FormUrlEncodedContent(new[]
            {
            new KeyValuePair<string, string>("url", URL)
        });

            var response = client.PostAsync("https://cleanuri.com/api/v1/shorten", content).Result;

            if (response.IsSuccessStatusCode)
            {
                var resultJson = response.Content.ReadAsStringAsync().Result;
                using var doc = JsonDocument.Parse(resultJson);
                var shortUrl = doc.RootElement.GetProperty("result_url").GetString();
                return shortUrl;
            }
            else
            {
                return "Error: Unable to shorten URL";
            }
        }
    }
}