using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using MimeKit;
using MimeKit.Cryptography;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.OpenSsl;

namespace Core.Mailing.MailKitImplementations
{
    /// <summary>
    /// Implementation of the IMailService interface using MailKit library for sending emails.
    /// </summary>
    public class MailKitMailService : IMailService
    {
        private readonly MailSettings _mailSettings;
        private DkimSigner? _signer;

        /// <summary>
        /// Initializes a new instance of the MailKitMailService class.
        /// </summary>
        /// <param name="configuration">The configuration containing mail settings.</param>
        public MailKitMailService(IConfiguration configuration)
        {
            _mailSettings = configuration.GetSection("MailSettings").Get<MailSettings>();
            _signer = null;
        }

        /// <summary>
        /// Sends an email synchronously.
        /// </summary>
        /// <param name="mail">The Mail object containing email details.</param>
        public void SendMail(Mail mail)
        {
            if (mail.ToList == null || mail.ToList.Count < 1) return;
            EmailPrepare(mail, out MimeMessage email, out SmtpClient smtp);
            smtp.Send(email);
            smtp.Disconnect(true);
            email.Dispose();
            smtp.Dispose();
        }

        /// <summary>
        /// Sends an email asynchronously.
        /// </summary>
        /// <param name="mail">The Mail object containing email details.</param>
        /// <returns>A Task representing the asynchronous operation.</returns>
        public async Task SendEmailAsync(Mail mail)
        {
            if (mail.ToList == null || mail.ToList.Count < 1) return;
            EmailPrepare(mail, out MimeMessage email, out SmtpClient smtp);
            await smtp.SendAsync(email);
            smtp.Disconnect(true);
            email.Dispose();
            smtp.Dispose();
        }

        private void EmailPrepare(Mail mail, out MimeMessage email, out SmtpClient smtp)
        {
            // Prepare the email message
            email = new MimeMessage();
            email.From.Add(new MailboxAddress(_mailSettings.SenderFullName, _mailSettings.SenderEmail));
            email.To.AddRange(mail.ToList);
            if (mail.CcList != null && mail.CcList.Any()) email.Cc.AddRange(mail.CcList);
            if (mail.BccList != null && mail.BccList.Any()) email.Bcc.AddRange(mail.BccList);

            email.Subject = mail.Subject;
            if (mail.UnscribeLink != null) email.Headers.Add("List-Unsubscribe", $"<{mail.UnscribeLink}>");
            var bodyBuilder = new BodyBuilder
            {
                TextBody = mail.TextBody,
                HtmlBody = mail.HtmlBody
            };

            if (mail.Attachments != null)
                foreach (var attachment in mail.Attachments)
                    if (attachment != null)
                        bodyBuilder.Attachments.Add(attachment);

            email.Body = bodyBuilder.ToMessageBody();
            email.Prepare(EncodingConstraint.SevenBit);

            if (_mailSettings.DkimPrivateKey != null && _mailSettings.DkimSelector != null && _mailSettings.DomainName != null)
            {
                // Sign the email using DKIM if the necessary settings are provided
                _signer = new DkimSigner(ReadPrivateKeyFromPemEncodedString(), _mailSettings.DomainName, _mailSettings.DkimSelector)
                {
                    HeaderCanonicalizationAlgorithm = DkimCanonicalizationAlgorithm.Simple,
                    BodyCanonicalizationAlgorithm = DkimCanonicalizationAlgorithm.Simple,
                    AgentOrUserIdentifier = $"@{_mailSettings.DomainName}",
                    QueryMethod = "dns/txt"
                };
                var headers = new HeaderId[] { HeaderId.From, HeaderId.Subject, HeaderId.To };
                _signer.Sign(email, headers);
            }

            // Connect to the SMTP server and authenticate if necessary
            smtp = new SmtpClient();
            smtp.Connect(_mailSettings.Server, _mailSettings.Port, SecureSocketOptions.Auto);
            if (_mailSettings.AuthenticationRequired) smtp.Authenticate(_mailSettings.UserName, _mailSettings.Password);
        }

        private AsymmetricKeyParameter ReadPrivateKeyFromPemEncodedString()
        {
            AsymmetricKeyParameter result;
            var pemEncodedKey = "-----BEGIN RSA PRIVATE KEY-----\n" + _mailSettings.DkimPrivateKey + "\n-----END RSA PRIVATE KEY-----";
            using (var stringReader = new StringReader(pemEncodedKey))
            {
                var pemReader = new PemReader(stringReader);
                var pemObject = pemReader.ReadObject();
                result = ((AsymmetricCipherKeyPair)pemObject).Private;
            }

            return result;
        }
    }
}
