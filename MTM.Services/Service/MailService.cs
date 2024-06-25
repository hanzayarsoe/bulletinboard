using Microsoft.Extensions.Options;
using MimeKit;
using MailKit.Net.Smtp;
using System.Diagnostics;
using MTM.Entities.DTO;
using Microsoft.Extensions.Logging;
using EmailService.Configuration;
using MTM.Services.IService;

namespace MTM.Services.Service
{
    public class MailService : IMailService
    {
        private readonly MailSettings _mailSettings;
        private readonly ILogger<MailService> _logger;

        public MailService(IOptions<MailSettings> mailSettingsOptions, ILogger<MailService> logger)
        {
            _mailSettings = mailSettingsOptions.Value;
            _logger = logger;
        }
        public async Task<bool> SendHTMLMail(HTMLMailData htmlMailData)
        {
            try
            {
                using (MimeMessage emailMessage = new MimeMessage())
                {
                    MailboxAddress emailFrom = new MailboxAddress(_mailSettings.SenderName, _mailSettings.SenderEmail);
                    emailMessage.From.Add(emailFrom);
                    MailboxAddress emailTo = new MailboxAddress(htmlMailData.EmailToName, htmlMailData.EmailToId);
                    emailMessage.To.Add(emailTo);
                    emailMessage.Subject = "Password Reset Request";
                    string filePath = Path.Combine(Directory.GetCurrentDirectory(), "Templates", "index.html");
                    string emailTemplateText = await File.ReadAllTextAsync(filePath);
                    emailTemplateText = emailTemplateText.Replace("{RECIPIENT_NAME}", htmlMailData.EmailToName);
                    emailTemplateText = emailTemplateText.Replace("{CURRENT_DATE}", DateTime.Today.Date.ToShortDateString());
                    emailTemplateText = emailTemplateText.Replace("{RESET_LINK}", htmlMailData.ResetLink);
                    BodyBuilder emailBodyBuilder = new BodyBuilder();
                    emailBodyBuilder.HtmlBody = emailTemplateText;
                    emailBodyBuilder.TextBody = "";
                    emailMessage.Body = emailBodyBuilder.ToMessageBody();

                    using (var mailClient = new SmtpClient())
                    {
                        await mailClient.ConnectAsync(_mailSettings.Server, _mailSettings.Port, MailKit.Security.SecureSocketOptions.StartTls);
                        await mailClient.AuthenticateAsync(_mailSettings.UserName, _mailSettings.Password);
                        await mailClient.SendAsync(emailMessage);
                        await mailClient.DisconnectAsync(true);
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return false;
            }
        }
    }
}
