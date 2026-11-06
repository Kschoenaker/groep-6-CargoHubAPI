
using MyApi.Data;

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

builder.Services.AddControllers();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();

app.Urls.Add("http://localhost:3000");

app.Run();
