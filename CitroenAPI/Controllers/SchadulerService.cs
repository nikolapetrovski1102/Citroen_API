
using Azure;
using CitroenAPI.Models.DbContextModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics;

namespace CitroenAPI.Controllers
{
    public class SchadulerService : IHostedService, IDisposable
    {
        private Timer _timerForNext;
        private readonly CitroenDbContext _context;
        public IConfiguration _configuration { get; set; }
        private IServiceScopeFactory _scopeFactory;
        private Microsoft.AspNetCore.Hosting.IHostingEnvironment _environment;
        public SchadulerService(IServiceScopeFactory serviceScopeFactory,
            Microsoft.AspNetCore.Hosting.IHostingEnvironment environment, IConfiguration configuration, CitroenDbContext context)
        {
            _scopeFactory = serviceScopeFactory;
            _environment = environment;
            _configuration = configuration;
            _context = context;
        }
        public void Dispose()
        {
           // throw new NotImplementedException();
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _timerForNext = new Timer(RunAgain, null, TimeSpan.Zero, TimeSpan.FromMinutes(10));
            return Task.CompletedTask;
        }

        private async void RunAgain(object? state)
        {
            try
            {
                var client = new HttpClient();
                HttpRequestMessage request;
                if (!Debugger.IsAttached)
                    request = new HttpRequestMessage(HttpMethod.Post, "https://cyberlink-001-site29.anytempurl.com/api/CitroenApi");
                else
                    request = new HttpRequestMessage(HttpMethod.Post, "http://localhost:5216/api/CitroenApi");
                request.Headers.Add("password", "b5267c1e130ec85238d12a4e5f2c85a1b185f7b7");
                var response = await client.SendAsync(request);
                response.EnsureSuccessStatusCode();
                Console.WriteLine(await response.Content.ReadAsStringAsync());

                ApiCalls apiCalls = new ApiCalls();

                DateTime dateTimeNow;

                dateTimeNow = TimeZoneInfo.ConvertTime(DateTime.Now, TimeZoneInfo.FindSystemTimeZoneById("Central European Standard Time"));
                apiCalls.Status = response.StatusCode.ToString();
                apiCalls.CallDateTime = dateTimeNow;

                try
                {
                    _context.ApiCalls.Add(apiCalls);
                    await _context.SaveChangesAsync();
                }
                catch (Exception ex) {
                    apiCalls.Status = ex.Message.ToString();
                    _context.ApiCalls.Add(apiCalls);

                    await _context.SaveChangesAsync();
                }

            }
            catch (Exception ex)
            {
                ApiCalls apiCalls = new ApiCalls();

                DateTime dateTimeNow;

                dateTimeNow = TimeZoneInfo.ConvertTime(DateTime.Now, TimeZoneInfo.FindSystemTimeZoneById("Central European Standard Time"));
                apiCalls.Status = StatusCodes.Status500InternalServerError.ToString();
                apiCalls.CallDateTime = dateTimeNow;

                _context.ApiCalls.Add(apiCalls);

                await _context.SaveChangesAsync();


            }


        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
