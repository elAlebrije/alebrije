using System.Threading.Tasks;
using Alebrije.Abstractions.Communication;
using Alebrije.Extensions;
using Microsoft.Extensions.Logging;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

namespace Alebrije.Communication.Twilio
{
    public class SmsService : ISmsService
    {
        private readonly ILogger<SmsService> _logger;
        private readonly Settings _settings;
        public SmsService(ILogger<SmsService> logger, Settings settings)
        {
            _logger = logger;
            _settings = settings;
        }

        public async Task SendSimpleSmsAsync(string toPhoneNumber, string content)
        {
            TwilioClient.Init(_settings.Options.AccountSid, _settings.Options.AuthToken);

            var response = await MessageResource.CreateAsync(
                body: content,
                @from: new PhoneNumber(_settings.From.PhoneNumber),
                to: new PhoneNumber(toPhoneNumber)
            );

            _logger.Info(response.Status.ToString());
        }
    }
}