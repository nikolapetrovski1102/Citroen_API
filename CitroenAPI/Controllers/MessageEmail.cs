
using CitroenAPI.Models;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit;
using System.Net.Mail;

namespace CitroenAPI.Controllers
{

    public class MessageEmail
    {
        private List<String> messages;
        public List<MailboxAddress> To { get; set; }
        public string Subject { get; set; }
        public string Content { get; set; }
        private EmailConfiguration _emailConfig;
        public MessageEmail(IEnumerable<string> to, string subject, string content,EmailConfiguration config)
        {
            _emailConfig = config;
            messages = new List<String>();
            messages.Add("hmladenovski@ohanaone.mk");
            messages.Add("aandonovski@ohanaone.mk");
            messages.Add("bstoimenov@ohanaone.mk");
            messages.Add("npetrovski@ohanaone.mk");

            // Create a list to store MailboxAddress instances
            List<MailboxAddress> addresses = new List<MailboxAddress>();

            // Iterate over the messages collection and create MailboxAddress instances
            foreach (var message in messages)
            {
                // Create a MailboxAddress instance with email address and name
                addresses.Add(new MailboxAddress("Recipient Name", message));
            }


            To = new List<MailboxAddress>();
            To.AddRange(addresses);
            Subject = subject;
            Content = content;
        }
        public void SendEmail()
        {
            
            // Create a MimeMessage
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(_emailConfig.UserName, _emailConfig.From));
            message.To.AddRange(To);
            message.Subject = Subject;

            // Create a body part
            var bodyBuilder = new BodyBuilder();
            bodyBuilder.TextBody = Content;

            // Add the body to the message
            message.Body = bodyBuilder.ToMessageBody();

            try
            {
                // Send the message
                using (var client = new MailKit.Net.Smtp.SmtpClient())
                {
                    client.Connect(_emailConfig.SmtpServer, 465, useSsl: true);
                    client.Authenticate(_emailConfig.UserName, _emailConfig.Password);
                    client.Send(message);
                    client.Disconnect(true);
                }
            }
            catch(Exception e)
            {

            }
        }


    }
}
