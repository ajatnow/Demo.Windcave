using Demo.Windcave.Models.Configuration;
using Microsoft.Extensions.Configuration;

namespace Demo.Windcave.Repositories
{
    public interface IAppConfigurationRepository
    {
		WindcaveApiSettings? WindcaveApiSettings { get; }
    }

    public class AppConfigurationRepository(IConfiguration configuration) : IAppConfigurationRepository
    {
        private readonly IConfiguration _configuration = configuration;

		public WindcaveApiSettings? WindcaveApiSettings => _configuration.GetSection(WindcaveApiSettings.WindcaveApi).Get<WindcaveApiSettings>();
	}
}
