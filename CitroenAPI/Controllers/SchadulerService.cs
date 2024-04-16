
using Azure;
using CitroenAPI.Logger;
using CitroenAPI.Models.DbContextModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Org.BouncyCastle.Security;
using System.Diagnostics;

namespace CitroenAPI.Controllers
{
    public class SchadulerService : IHostedService, IDisposable
    {
        private readonly ILogger<SchadulerService> _logger;
        private Timer _timerForNext;
        private readonly CitroenDbContext _context;
        public IConfiguration _configuration { get; set; }
        private IServiceScopeFactory _scopeFactory;
        private Microsoft.AspNetCore.Hosting.IHostingEnvironment _environment;
        public SchadulerService(IServiceScopeFactory serviceScopeFactory,
            Microsoft.AspNetCore.Hosting.IHostingEnvironment environment, IConfiguration configuration, CitroenDbContext context,ILoggerFactory loggerFactory,ILogger<SchadulerService> logger)
        {
            loggerFactory.AddFile(Path.Combine(Directory.GetCurrentDirectory(), "logs"));
            _scopeFactory = serviceScopeFactory;
            _environment = environment;
            _configuration = configuration;
            _context = context;
            _logger = logger;
            _logger.LogInformation("Constructor call in the service");
        }
        public void Dispose()
        {
            _logger.LogInformation("Dispoce method was called inside constructor");
           // throw new NotImplementedException();
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Started the task async method");
            _timerForNext = new Timer(RunAgain, null, TimeSpan.Zero, TimeSpan.FromMinutes(10));
            _logger.LogInformation("Executed the RunAgain method with result");
            return Task.CompletedTask;
        }

        private async void RunAgain(object? state)
        {
            _logger.LogInformation("Started RunAgain method");
            try
            {
                var client = new HttpClient();
                
                HttpRequestMessage request;
                _logger.LogInformation("Created a httpClient and request");
                if (!Debugger.IsAttached)
                {
                    request = new HttpRequestMessage(HttpMethod.Post, "https://cyberlink-001-site29.anytempurl.com/api/CitroenApi");
                    _logger.LogInformation("Debuger is not Attached " + request.ToString());
                }
                else
                {
                    request = new HttpRequestMessage(HttpMethod.Post, "http://localhost:5216/api/CitroenApi");
                    _logger.LogInformation("Debugger is attached " + request.ToString());
                }
                request.Headers.Add("password", "b5267c1e130ec85238d12a4e5f2c85a1b185f7b7");
                _logger.LogInformation("Added Headers");
                var response = await client.SendAsync(request);
                _logger.LogInformation("Response result "+response.Content.ToString());
                response.EnsureSuccessStatusCode();
                _logger.LogInformation("Response result is successfull check");
                Console.WriteLine(await response.Content.ReadAsStringAsync());

                ApiCalls apiCalls = new ApiCalls();

                DateTime dateTimeNow;

                dateTimeNow = TimeZoneInfo.ConvertTime(DateTime.Now, TimeZoneInfo.FindSystemTimeZoneById("Central European Standard Time"));
                apiCalls.Status = response.StatusCode.ToString();
                apiCalls.CallDateTime = dateTimeNow;
                _logger.LogInformation("ApiCalll is created");
                try
                {
                    _logger.LogInformation("ApiCall Trying to save in Database");
                    _context.ApiCalls.Add(apiCalls);
                   
                    await _context.SaveChangesAsync();
                    _logger.LogInformation("ApiCall was saved into DB");
                }
                catch (Exception ex) {
                    _logger.LogError("0 ApiCall error wile saving in DB "+ex.Message);
                       apiCalls.Status = ex.Message.ToString();
                        _logger.LogInformation("1 One more time to store the api calls");
                        _context.ApiCalls.Add(apiCalls);

                        await _context.SaveChangesAsync();
                        _logger.LogInformation("1 Api calls are stored");
                    
                   
                }

            }
            catch (Exception ex)
            {
                _logger.LogError("1 ApiCall error wile saving in DB " + ex.Message);
                try
                {
                    _logger.LogInformation("3 One more time to store the api calls");
                    ApiCalls apiCalls = new ApiCalls();

                    DateTime dateTimeNow;

                    dateTimeNow = TimeZoneInfo.ConvertTime(DateTime.Now, TimeZoneInfo.FindSystemTimeZoneById("Central European Standard Time"));
                    apiCalls.Status = StatusCodes.Status500InternalServerError.ToString();
                    apiCalls.CallDateTime = dateTimeNow;

                    _context.ApiCalls.Add(apiCalls);

                    await _context.SaveChangesAsync();
                    _logger.LogInformation("3 Api Calls are stored");
                }
                catch
                {
                    _logger.LogInformation("3 Api Calls are not stored due to the previous problems");
                }

            }


        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Stop {cancellationToken} - Service was stopped");
            return Task.CompletedTask;
        }
    }
}
