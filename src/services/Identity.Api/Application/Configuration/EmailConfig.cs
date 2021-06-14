using System.Collections.Generic;

namespace Identity.Api.Application.Configuration
{
    public class EmailConfig
    {
        public string ApiKey { get; set; }
        public string FromEmail { get; set; }
        public string From { get; set; }
        public string ConfirmUrl { get; set; }

        public enum Template
        {
            EmailConfirm,
            EmailForgot,
            Invoice
        }

        public readonly Dictionary<Template, string> Templates = new()
        {
            { Template.EmailConfirm, "d-c4805aeb3bb145dc818a009ddca0bded"},
            { Template.EmailForgot, "d-c4805aeb3bb145dc818a009ddca0bded"},
            { Template.Invoice, "d-da275396c2354fb38d56ebb59fa1c63b" }
        };
    }
}