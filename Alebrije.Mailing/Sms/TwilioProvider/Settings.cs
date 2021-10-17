namespace Alebrije.Communication.Sms.TwilioProvider
{
    public class Settings
    {
        public class ServiceOptions
        {
            public string AccountSid { get; set; }

            public string AuthToken { get; set; }
        }

        public class FromOptions
        {
            public string PhoneNumber { get; set; }
        }


        public ServiceOptions Options { get; set; }

        public FromOptions From { get; set; }
    }
}