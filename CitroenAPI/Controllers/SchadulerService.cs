
using CitroenAPI.Models.DbContextModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace CitroenAPI.Controllers
{
    public class SchadulerService : IHostedService, IDisposable
    {
        private Timer _timerForNext;
        public IConfiguration _configuration { get; set; }
        private IServiceScopeFactory _scopeFactory;
        private Microsoft.AspNetCore.Hosting.IHostingEnvironment _environment;
        public SchadulerService(IServiceScopeFactory serviceScopeFactory, Microsoft.AspNetCore.Hosting.IHostingEnvironment environment, IConfiguration configuration)
        {
            _scopeFactory = serviceScopeFactory;
            _environment = environment;
            _configuration = configuration;

        }
        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _timerForNext = new Timer(RunAgain, null, TimeSpan.Zero, TimeSpan.FromHours(1));
            return Task.CompletedTask;
        }

        private async void RunAgain(object? state)
        {

            var client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Post, "https://cyberlink-001-site29.anytempurl.com/api/CitroenApi");
            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();
           

        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
