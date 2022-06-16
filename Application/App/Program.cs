using Barriot;
using Barriot.API.Translation;
using Barriot.Data;
using Barriot.Extensions;
using Barriot.Interactions;
using Barriot.Interactions.Converters;
using Barriot.Interactions.Services;
using MongoDB.Bson;
using MongoDB.Driver;

var builder = WebApplication.CreateBuilder(args);

// Set the proper configuration for the application builder.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddRouting();

// Configure the sharded client.
var config = new DiscordRestConfig()
{
    LogLevel = LogSeverity.Verbose,
    FormatUsersInBidirectionalUnicode = false,
    APIOnRestInteractionCreation = false,
};
var client = new DiscordRestClient(config);

await client.LoginAsync(TokenType.Bot, builder.Configuration["Token"]);

// Add all services to the application builder.
builder.Services.AddLogging();
builder.Services.AddSingleton(client);
builder.Services.AddInteractionService();

builder.Services.AddSingleton(new MongoClient(new MongoUrlBuilder(
    builder.Configuration["DbToken"])
    .ToMongoUrl()));

// Configure the HTTP clients
builder.Services.AddHttpClient<ITranslateClient, TranslateClient>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["TranslateAPI"]);
});

// Add logic management
builder.Services.AddSingleton<DatabaseManager>();
builder.Services.AddSingleton<ClientController>();
builder.Services.AddSingleton<PostExecutionHandler>();

// Add services
builder.Services.AddSingleton<InfoService>();
builder.Services.AddSingleton<TranslateService>();
builder.Services.AddSingleton<UserService>();

builder.Services.AddSingleton<ApiController>();

// Create the application.
var app = builder.Build();

var service = app.Services.GetRequiredService<InteractionService>();

service.AddTypeConverter<ulong>(new UlongConverter());
service.AddTypeConverter<TimeSpan>(new TimeSpanConverter());
service.AddTypeConverter<Calculation>(new CalculationConverter());

service.AddComponentTypeConverter<TimeSpan>(new TimeSpanComponentConverter());
service.AddComponentTypeConverter<Color>(new ColorComponentConverter());

service.AddTypeReader<ObjectId>(new ObjectIdComponentConverter());
service.AddTypeReader<Guid>(new Barriot.Interactions.Converters.GuidConverter());

// Register all modules.
await service.AddModulesAsync(typeof(Program).Assembly, app.Services);

// Configure managers.
await app.Services.GetRequiredService<DatabaseManager>()
    .ConfigureAsync();
await app.Services.GetRequiredService<ClientController>()
    .ConfigureAsync();

// Final configuration entries. 
app.UseAuthorization();

// Set up the ability to redirect REST requests.
app.RedirectInteractions("/interactions", builder.Configuration["PBK"]);
app.RedirectVotes("/votes", builder.Configuration["TGGAuthKey"]);

// Run the application
app.Run();