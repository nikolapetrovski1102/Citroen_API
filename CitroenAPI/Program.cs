using CitroenAPI.Controllers;
using CitroenAPI.Models.DbContextModels;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
IConfiguration configuration = (new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json").Build());

builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<CitroenDbContext>(options =>
    options.UseSqlServer(configuration["ConnectionStrings:ConnectionString"].ToString()), ServiceLifetime.Singleton);


//builder.Services.AddSingleton<IHostedService, SchadulerService>();
// Register the log cleanup service
string logDirectory = Path.Combine(Directory.GetCurrentDirectory(), "logs");
builder.Services.AddSingleton(new LogCleanupService(logDirectory));

// Register the background service
builder.Services.AddHostedService<LogCleanupBackgroundService>();

var app = builder.Build();

app.UseStaticFiles();

app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthorization();

app.MapControllers();

app.Run();
