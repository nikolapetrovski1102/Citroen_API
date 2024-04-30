using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using CitroenAPI.Models;


namespace CitroenAPI.Controllers
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            // Retrieve EmailConfiguration from appsettings.json
            var emailConfig = Configuration
                .GetSection("EmailConfiguration")
                .Get<EmailConfiguration>();

            // Register EmailConfiguration as a singleton service
            services.AddSingleton(emailConfig);

            // Add controllers
            services.AddControllers();
        }
    }
}