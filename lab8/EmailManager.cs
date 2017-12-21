using System.Net.Mail;

namespace Laba_8
{
    class EmailManager
    {
        private readonly SmtpClient _smtpClient;
        //Fill with your data
        private const string sender = "dagger131313@yandex.ru";
        private const string password = "";

        private const string host = "smtp.yandex.ru";
        private const int port = 587;

        public EmailManager()
        {
            _smtpClient = new SmtpClient(host, port)
            {
                Credentials = new System.Net.NetworkCredential(sender, password),
                EnableSsl = true
            };
        }

        public void SendEmail(string receiver, string topic, string filePath)
        {
            var mail = new MailMessage(sender, receiver, topic, string.Empty);
            using (var  attachment = new Attachment(filePath))
            {
                mail.Attachments.Add(attachment);
                _smtpClient.Send(mail);
            }
        }
    }
}
