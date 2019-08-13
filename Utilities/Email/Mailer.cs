using System;
using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Options;
using Utilities.Logging;

namespace Utilities.Mailer {

	public class Mailer {

		private readonly IOptions<MailerOptions> options;

		public Mailer(IOptions<MailerOptions> options)
		{
			this.options = options;
		}

		public void Send(string email, string subject, string body) {
			try {
				MailMessage mail = new MailMessage();
				SmtpClient server = new SmtpClient(this.options.Value.Server);
				mail.From = new MailAddress(this.options.Value.From.Email, this.options.Value.From.Name);
				mail.To.Add(email);
				mail.Subject = subject;
				mail.Body = body;
				mail.IsBodyHtml = true;
				server.Port = 587;
				server.Credentials = new NetworkCredential(this.options.Value.Login, this.options.Value.Password);
				server.EnableSsl = true;
				server.Send(mail);
				Logger.Debug(typeof(Mailer), "Sent email to " + email);
			} catch (Exception ex) {
				Logger.Error(ex, "Sending email to " + email + " failed");
			}
		}

	}

}