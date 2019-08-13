using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;

namespace Utilities.Mailer
{
	public static class ServiceCollectionExtensions
	{
		public static void AddMailer(this IServiceCollection services, IConfigurationSection section)
		{
			services.Configure<MailerOptions>(options => section.Bind(options));
			services.AddTransient<Mailer>();
		}
	}
}