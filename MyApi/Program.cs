
var builder = WebApplication.CreateBuilder(args);

// Registreer de ondersteuning voor API-controllers.
builder.Services.AddControllers();

// Bouw de applicatie.
var app = builder.Build();

// Registreer de routes van de controllers.
app.MapControllers();

// Start de API op poort 3000, zoals in de Python-versie.
app.Urls.Add("http://localhost:3000");

app.Run();
