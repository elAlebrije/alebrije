using System;
using Alebrije.Communication.Twilio;
using Microsoft.Extensions.Logging.Abstractions;

namespace SandBox
{
    class Program
    {
        static void Main(string[] args)
        {
            var twilioSender = new SmsService(new NullLogger<SmsService>(), new Settings
            {
                From = new Settings.FromOptions
                {
                    PhoneNumber = "+18482943405"
                },
                Options = new Settings.ServiceOptions
                {
                    AccountSid = "ACf49ae7cac595bf9ed8e0654a3d890726",
                    AuthToken = "5183a7e2144791c33cfe143aeb8aeec3"
                }
            });
            _ = twilioSender.SendSimpleSmsAsync("+525630738877", "Your Blok Activation Code is 987135");
        }
    }
}
