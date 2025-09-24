using GameStore.API.Extensions;
using GameStore.API.Helpers;
using GameStore.API.Mappings;
using GameStore.API.Middlewares;
using GameStore.Application.Services;
using GameStore.DAL.Data;
using GameStore.DAL.SeedData;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Infrastructure;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;

// Configure Serilog to read from appsettings.json
builder.Host.UseSerilog((context, loggerConfiguration) =>
{
    var mongoDbSettings = context.Configuration.GetSection("MongoDB");
    var connectionString = mongoDbSettings.GetValue<string>("ConnectionString");
    var logsCollectionName = mongoDbSettings.GetValue<string>("LogsCollection");

    loggerConfiguration
        .ReadFrom.Configuration(context.Configuration)
        .WriteTo.Logger(config =>
            config
                .Filter.ByIncludingOnly(e =>
                    e.MessageTemplate.Text.Contains("EntityChange"))
                .WriteTo.MongoDB(
                    databaseUrl: connectionString!,
                    collectionName: logsCollectionName!));
});

QuestPDF.Settings.License = LicenseType.Community;

// Add services to the container.
builder.Services.AddConfigurations(builder.Configuration);
builder.Services.AddAutoMapper(typeof(MappingProfile));

builder.Services.AddUserManagement(builder.Configuration);
builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddAuthorizationPolicies();

builder.Services.AddBlobStorage(builder.Configuration);
builder.Services.AddRepositories(builder.Configuration);
builder.Services.AddGameStoreServices();
builder.Services.AddNotificationServices(builder.Configuration);

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(new ObjectIdConverter());
});
builder.Services.ConfigureCaching();
builder.Services.AddSwagger();
builder.Services.ConfigureCors();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<GameStoreContext>();

    context.Database.Migrate();
    GameStoreSeedData.Seed(context, 100);

    var productKeyCreator = scope.ServiceProvider.GetRequiredService<ProductKeyCreator>();
    await productKeyCreator.AddKeyToProductsAsync();

    // Міграції для Identity
    var identityContext = services.GetRequiredService<AuthDbContext>();
    identityContext.Database.Migrate();
}

await IdentitySeedData.SeedIdentityDataAsync(app.Services);

// Configure the HTTP request pipeline.
app.ConfigureExceptionHandler(app.Services.GetRequiredService<ILogger<Program>>());

app.UseCors("AllowAngular");

app.UseMiddleware<RequestLoggingMiddleware>();
app.UseMiddleware<RequestLocalizationMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
        c.RoutePrefix = string.Empty;
    });
}

app.UseResponseCaching();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseMiddleware<TotalNumberOfGamesMiddleware>();

app.MapControllers();

app.Run();

Log.CloseAndFlush();