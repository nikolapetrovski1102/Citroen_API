using System.Net.Mail;
using System.Net;

namespace CitroenAPI.Controllers
{

    public class Emailer
    {
        IConfiguration configuration = (new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json").Build());
        private string MailServer;
        private int Port;
        private string AuthUserName;
        private string AuthPassword;

        public Emailer(string mailServer, int port, string authUserName, string authPassword)
        {
            this.MailServer = mailServer;
            this.Port = port;
            this.AuthUserName = authUserName;
            this.AuthPassword = authPassword;
        }

        public void SendEmail(string subject, string body)
        {
            using (SmtpClient smtpClient = new SmtpClient(MailServer, Port))
            {
                smtpClient.Credentials = new NetworkCredential(AuthUserName, AuthPassword);
                smtpClient.EnableSsl = true;

                using (MailMessage mailMessage = new MailMessage())
                {
                    mailMessage.From = new MailAddress(AuthUserName);
                    foreach (var recipient in configuration.GetSection("Emails:RecipientList").Get<List<string>>())
                    {
                        mailMessage.To.Add(recipient);
                    }
                    mailMessage.Subject = subject;
                    mailMessage.Body = body;

                    try
                    {
                        smtpClient.Send(mailMessage);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error sending email: {ex.Message}");
                    }
                }
            }
        }
    }

}
