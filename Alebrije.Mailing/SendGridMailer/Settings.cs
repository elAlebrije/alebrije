using System.Collections.Generic;
using Alebrije.Abstractions.Mailing;

namespace Alebrije.Mailing.SendGridMailer
{
    public class Settings
    {
        public enum TemplateNames
        {
            None,
            EmailConfirmation
        }

        public class ServiceOptions
        {
            public string ApiKey { get; set; }

            public List<TemplateOptions> Templates { get; set; }
        }

        public class TemplateOptions
        {
            public TemplateNames Name { get; set; }
            public string Key { get; set; }
        }

        public MailingAddress.BasicInformation From { get; set; }

        public ServiceOptions Options { get; set; }
    }


}