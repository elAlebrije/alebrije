using System.Linq;
using System.Threading.Tasks;
using Alebrije.Abstractions.Mailing;
using Microsoft.Extensions.Logging;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace Alebrije.Mailing.SendGridMailer
{
    public class MailingService : IMailingService
    {
        private readonly ILogger<MailingService> _logger;
        private readonly Settings _settings;

        public MailingService(ILogger<MailingService> logger, Settings settings)
        {
            _logger = logger;
            _settings = settings;
        }

        public async Task SendEmailAsync(EmailContent emailInfo)
        {
            var client = GetMailOutputInfo(emailInfo, out var from, out var to);

            var msg = MailHelper.CreateSingleEmail(from, to, emailInfo.Subject, emailInfo.PlainText, emailInfo.HtmlContent);

            _logger.LogInformation($"Attempting to send email [{emailInfo.Subject}] to [{_settings.From.EmailAddress}]");
            await SendMessageAsync(client, msg);
        }

        public async Task SendTemplateEmailAsync(EmailTemplateContent emailInfo)
        {
            var client = GetMailOutputInfo(emailInfo, out var from, out var to);

            var templateId = _settings.Options.Templates.SingleOrDefault(x => x.Name.ToString() == emailInfo.TemplateName)?.Key;

            _logger.LogInformation($"Attempting to send email [{emailInfo.TemplateName}] to [{_settings.From.EmailAddress}]");
            var msg = MailHelper.CreateSingleTemplateEmail(from, to, templateId, emailInfo.InputParams);
            await SendMessageAsync(client, msg);
        }

        private SendGridClient GetMailOutputInfo(IOutputEmail emailInfo, out EmailAddress from, out EmailAddress to)
        {
            var client = new SendGridClient(_settings.Options.ApiKey);

            from = new EmailAddress(_settings.From.EmailAddress, _settings.From.DisplayName);
            to = new EmailAddress(emailInfo.To.Address, emailInfo.To.DisplayName);
            return client;
        }

        private async Task SendMessageAsync(SendGridClient client, SendGridMessage message)
        {
            var response = await client.SendEmailAsync(message);

            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation($"Email sent successfully.");
            }
            else
            {
                _logger.LogWarning($"Email was not sent.");
                _logger.LogWarning(response.Body.ToString());
            }
        }
    }
}