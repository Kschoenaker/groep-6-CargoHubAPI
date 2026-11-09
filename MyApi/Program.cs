using Microsoft.EntityFrameworkCore;
using MyApi.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

if (args.Length == 1 && args[0].Equals("--import-json-brondata", StringComparison.OrdinalIgnoreCase))
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
        ?? throw new InvalidOperationException(
            "De connection string 'DefaultConnection' ontbreekt.");

    await JsonBronDataImporter.ImportAsync(connectionString, AppContext.BaseDirectory);
    Console.WriteLine("Alle JSON-bronbestanden zijn geïmporteerd in de tabel json_brondata.");
    return;
}

if (args.Length == 1 && args[0].Equals("--seed-business-data", StringComparison.OrdinalIgnoreCase))
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
        ?? throw new InvalidOperationException(
            "De connection string 'DefaultConnection' ontbreekt.");

    await BusinessDataSeeder.SeedAsync(connectionString, AppContext.BaseDirectory);
    return;
}

// connection string ophalen uit appsettings.json
var apiConnectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException(
        "De connection string 'DefaultConnection' ontbreekt.");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(apiConnectionString));

builder.Services.AddControllers();

// Database: PostgreSQL
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection")));

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();

app.Urls.Add("http://localhost:3000");

app.Run();

// Nodig zodat de integratietests de API kunnen opstarten
public partial class Program { }
