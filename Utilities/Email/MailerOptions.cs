namespace Utilities.Mailer
{
	public class Address
	{
		public string Email { get; set; }
		public string Name { get; set; }

	}

	public class MailerOptions
	{
		public string Server { get; set; }
		public string Login { get; set; }
		public string Password { get; set; }
		public Address From { get; set; }
	}
}