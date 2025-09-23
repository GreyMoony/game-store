using Azure.Messaging.ServiceBus;
using Azure.Storage.Blobs;
using GameStore.API.Helpers.RequestContext;
using GameStore.API.Middlewares;
using GameStore.Application.Helpers;
using GameStore.Application.Services;
using GameStore.Application.Services.BlobStorageService;
using GameStore.Application.Services.CommentServices;
using GameStore.Application.Services.EmailSender;
using GameStore.Application.Services.GameServices;
using GameStore.Application.Services.GenreServices;
using GameStore.Application.Services.NotificationServices;
using GameStore.Application.Services.NotificationServices.NotificationStrategies;
using GameStore.Application.Services.OrderServices;
using GameStore.Application.Services.PaymentStrategies;
using GameStore.Application.Services.PlatformServices;
using GameStore.Application.Services.PublisherService;
using GameStore.Application.Services.UserServices;
using GameStore.DAL.Data;
using GameStore.DAL.Interfaces;
using GameStore.DAL.Repositories;
using GameStore.DAL.Repositories.MongoRepositories;
using GameStore.Domain.Settings;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using MongoDB.Driver;
using SendGrid;

namespace GameStore.API.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddBlobStorage(this IServiceCollection services, IConfiguration config)
    {
        services.AddSingleton(sp =>
        {
            var connectionString = config["AZURE_STORAGE_CONNECTION_STRING"]
                ?? throw new InvalidOperationException("AZURE_STORAGE_CONNECTION_STRING is not set.");

            return new BlobServiceClient(connectionString);
        });
        services.AddSingleton<IBlobStorageService, BlobStorageService>();

        return services;
    }

    public static IServiceCollection AddRepositories(this IServiceCollection services, IConfiguration config)
    {
        services.AddDbContext<GameStoreContext>(options =>
            options.UseSqlServer(
                config.GetConnectionString("GameStoreDb"),
                o =>
                {
                    o.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
                    o.CommandTimeout(120);

                    o.EnableRetryOnFailure(
                        maxRetryCount: 5,
                        maxRetryDelay: TimeSpan.FromSeconds(10),
                        errorNumbersToAdd: null);
                }));
        services.AddScoped<IGameRepository, GameRepository>();
        services.AddScoped<IGenreRepository, GenreRepository>();
        services.AddScoped<IPlatformRepository, PlatformRepository>();
        services.AddScoped<IPublisherRepository, PublisherRepository>();
        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddScoped<ICommentRepository, CommentRepository>();
        services.AddScoped<INotificationsRepository, NotificationsRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        services.AddSingleton<IMongoClient>(serviceProvider =>
        {
            var connectionString = config["MongoDB:ConnectionString"]
                ?? throw new InvalidOperationException("MongoDB connection string is not set.");
            return new MongoClient(connectionString);
        });
        services.AddScoped(serviceProvider =>
        {
            var databaseName = config["MongoDB:DatabaseName"]
                ?? throw new InvalidOperationException("MongoDB database name is not set.");
            var client = serviceProvider.GetRequiredService<IMongoClient>();
            return client.GetDatabase(databaseName);
        });
        services.AddScoped<NorthwindDbContext>();
        services.AddScoped<IShipperRepository, ShipperRepository>();
        services.AddScoped<IOrderMongoRepository, OrderMongoRepository>();
        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<ISupplierRepository, SupplierRepository>();
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<IUnitOfWorkMongo, UnitOfWorkNorthwind>();

        return services;
    }

    public static IServiceCollection AddGameStoreServices(this IServiceCollection services)
    {
        services.AddScoped<ProductKeyCreator>();

        services.AddHttpClient();

        services.AddScoped<EntityChangeLogger>();
        services.AddScoped<IGameService, GameService>();
        services.AddScoped<IGenreService, GenreService>();
        services.AddScoped<IPlatformService, PlatformService>();
        services.AddScoped<IPublisherService, PublisherService>();
        services.AddScoped<IExternalAuthService, ExternalAuthService>();
        services.AddScoped<IUserService, UserIdentityService>();
        services.AddScoped<IRoleService, RoleService>();
        services.AddScoped<VisaPaymentStrategy>();
        services.AddScoped<BoxPaymentStrategy>();
        services.AddScoped<BankPaymentStrategy>();
        services.AddSingleton<IPaymentStrategyFactory, PaymentStrategyFactory>();
        services.AddScoped<IOrderService, OrderService>();
        services.AddScoped<ICommentService, CommentService>();
        services.AddScoped<IBanService, BanUserIdentityService>();

        services.AddScoped<IRequestLocalizationContext, RequestLocalizationContext>();
        return services;
    }

    public static IServiceCollection AddNotificationServices(this IServiceCollection services, IConfiguration config)
    {
        services.AddSingleton(provider =>
            new ServiceBusClient(config["AzureServiceBus:ConnectionString"]));

        services.AddSingleton<EmailNotificationSender>();
        services.AddSingleton<SmsNotificationSender>();
        services.AddSingleton<PushNotificationSender>();
        services.AddSingleton<INotificationSenderFactory, NotificationSenderFactory>();
        services.AddScoped<INotificationService, NotificationService>();

        services.AddSingleton(sp =>
        {
            var config = sp.GetRequiredService<IOptions<SendGridSettings>>().Value;
            return new SendGridClient(config.ApiKey);
        });
        services.AddSingleton<IEmailSender, EmailSender>();
        services.AddHostedService<EmailNotificationBackgroundService>();

        return services;
    }

    public static IServiceCollection AddSwagger(this IServiceCollection services)
    {
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });

            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                Scheme = "Bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "Enter 'Bearer {your_token}' to authenticate.",
            });

            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer",
                        },
                    },
                    Array.Empty<string>()
                },
            });
        });

        return services;
    }

    public static IServiceCollection ConfigureCors(this IServiceCollection services)
    {
        services.AddCors(options =>
        {
            options.AddPolicy("AllowAngular", builder =>
            {
                builder.WithOrigins("http://localhost:8080")
                       .AllowAnyHeader()
                       .AllowAnyMethod()
                       .AllowCredentials()
                       .WithExposedHeaders(TotalNumberOfGamesMiddleware.GamesCountHeader)
                       .SetPreflightMaxAge(TimeSpan.FromMinutes(10));
            });
        });

        return services;
    }

    public static IServiceCollection ConfigureCaching(this IServiceCollection services)
    {
        services.AddResponseCaching();
        services.AddMemoryCache(options =>
        {
            options.SizeLimit = 100;
        });

        return services;
    }
}
