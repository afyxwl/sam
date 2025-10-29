global using Microsoft.AspNetCore.Builder;
global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.Extensions.Hosting;

using SAMsa.Data;
using SAMsa.Services;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Configuration: TMDB key should be placed under Tmdb:ApiKey in appsettings or user secrets

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// In-memory DB for simplicity
builder.Services.AddDbContext<AppDbContext>(opt =>
	opt.UseInMemoryDatabase("sam-db"));

// Repository wiring: prefer MongoDB when configured
var mongoConn = builder.Configuration["Mongo:ConnectionString"];
var mongoDbName = builder.Configuration["Mongo:Database"] ?? "samdb";

// If no explicit connection string, try auto-detect local MongoDB and prefer it when available
if (string.IsNullOrWhiteSpace(mongoConn))
{
	var tryConn = "mongodb://localhost:27017";
	try
	{
		var client = new MongoDB.Driver.MongoClient(tryConn);
		// Try a quick list databases call with a small timeout to verify connection
		var task = Task.Run(async () =>
		{
			using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(1));
			await client.ListDatabaseNamesAsync(cts.Token);
		});
		if (task.Wait(TimeSpan.FromSeconds(2)) && task.IsCompletedSuccessfully)
		{
			mongoConn = tryConn;
		}
	}
	catch
	{
		// ignore — will fall back to EF InMemory
	}
}

if (!string.IsNullOrWhiteSpace(mongoConn))
{
	// Register Mongo client and DB
	builder.Services.AddSingleton<MongoDB.Driver.IMongoClient>(sp => new MongoDB.Driver.MongoClient(mongoConn));
	builder.Services.AddScoped(sp => sp.GetRequiredService<MongoDB.Driver.IMongoClient>().GetDatabase(mongoDbName));

	// Register Mongo repositories
	builder.Services.AddScoped<SAMsa.Repositories.IPostRepository, SAMsa.Repositories.MongoPostRepository>();
	builder.Services.AddScoped<SAMsa.Repositories.IReplyRepository, SAMsa.Repositories.MongoReplyRepository>();

	// Register storage info
	builder.Services.AddSingleton(new StorageInfo(true, mongoConn, mongoDbName));
}
else
{
	// Register EF repositories as default
	builder.Services.AddScoped<SAMsa.Repositories.IPostRepository, SAMsa.Repositories.EfPostRepository>();
	builder.Services.AddScoped<SAMsa.Repositories.IReplyRepository, SAMsa.Repositories.EfReplyRepository>();

	builder.Services.AddSingleton(new StorageInfo(false, string.Empty, string.Empty));
}

// HttpClient and OMDB service
builder.Services.AddHttpClient<SAMsa.Services.OmdbService>();

var app = builder.Build();

// Log which storage is in use
try
{
	var storageInfo = app.Services.GetRequiredService<StorageInfo>();
	var logger = app.Services.GetRequiredService<ILogger<Program>>();
	if (storageInfo != null && storageInfo.UsingMongo)
	{
		logger.LogInformation("Using MongoDB at {conn}, database {db}", storageInfo.Connection, storageInfo.Database);
	}
	else
	{
		logger.LogInformation("Using EF InMemory database");
	}
}
catch
{
	// ignore logging failures here
}

if (app.Environment.IsDevelopment())
{
	app.UseDeveloperExceptionPage();
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseRouting();
app.UseAuthorization();

app.MapControllers();

app.Run();
