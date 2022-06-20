using Barriot.Application.API;
using Barriot.Application.Interactions;
using Barriot.Application.Services;
using Barriot.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddLogging();
builder.Services.AddControllers();

var client = await ServiceExtensions.CreateClientAsync(builder.Configuration["PrivateToken"]);

builder.Services.AddSingleton(client);
builder.Services.AddDatabase(builder.Configuration["DbToken"]);
builder.Services.AddInteractions();
builder.Services.AddImplementations();
builder.Services.AddHttp(builder.Configuration.GetSection("APIs"));

var app = builder.Build();

await InteractionExtensions.ConfigureInteractionsAsync(app);

await app.Services.GetRequiredService<DatabaseManager>()
    .ConfigureAsync();

app.UseAuthorization();

app.MapControllers();

app.Run();
