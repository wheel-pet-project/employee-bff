using System.Reflection;
using Clients;
using Clients.Clients.Check;
using Clients.Clients.DrivingLicense;
using Clients.Clients.Identity;
using Clients.Clients.Rent;
using Clients.Clients.VehicleDocuments;
using Clients.Clients.VehicleFleet;
using Clients.Config;
using Clients.Interceptors;
using Gateway.Middlewares;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using OpenApiContractV1.Formatters;
using OpenApiContractV1.OpenApi;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;

namespace Gateway;

public static class ServiceCollectionExtensions
{
    private static readonly GatewayConfiguration Configuration;

    public static IServiceCollection RegisterGrpcClientsAndConfigure(this IServiceCollection services)
    {
        var configs = new List<ClientChannelConfig?>
        {
            ParseToChannelConfig<IdentityChannelConfig>(Configuration.Identity),
            ParseToChannelConfig<VehicleDocumentsChannelConfig>(Configuration.VehicleDocuments),
            ParseToChannelConfig<VehicleCheckChannelConfig>(Configuration.VehicleCheck),
            ParseToChannelConfig<DrivingLicenseChannelConfig>(Configuration.DrivingLicense),
            ParseToChannelConfig<RentChannelConfig>(Configuration.Rent),
            ParseToChannelConfig<VehicleFleetChannelConfig>(Configuration.VehicleFleet)
        };

        services.AddGrpcClientsForMicroservices(configs);

        return services;

        T ParseToChannelConfig<T>(ChannelConfiguration channelCfg)
            where T : ClientChannelConfig, new()
        {
            return new T { Uri = new Uri(channelCfg.Uri), ApiKey = channelCfg.ApiKey };
        }
    }


    public static IServiceCollection RegisterGrpcClientWrappers(this IServiceCollection services)
    {
        services.AddScoped<IdentityClientWrapper>();
        services.AddScoped<VehicleDocumentsClientWrapper>();
        services.AddScoped<CheckClientWrapper>();
        services.AddScoped<DrivingLicenseClientWrapper>();
        services.AddScoped<RentClientWrapper>();
        services.AddScoped<VehicleFleetClientWrapper>();

        return services;
    }

    public static IServiceCollection RegisterInterceptors(this IServiceCollection services)
    {
        services.AddScoped<CorrelationInterceptor>();

        return services;
    }

    public static IServiceCollection RegisterSwagger(this IServiceCollection services)
    {
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Version = "1.0",
                Title = "Employee BFF",
                Description = "BFF для работы с приложениями работников",
            });
            options.CustomSchemaIds(type => type.FriendlyId(true));
            options.IncludeXmlComments(
                $"{AppContext.BaseDirectory}{Path.DirectorySeparatorChar}{Assembly.GetEntryAssembly()?.GetName().Name}.xml");
        });

        services.AddSwaggerGenNewtonsoftSupport();

        return services;
    }

//
    public static IServiceCollection RegisterControllersWithNewtonsoft(this IServiceCollection services)
    {
        services.AddControllers(options => { options.InputFormatters.Insert(0, new InputFormatterStream()); })
            .AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                options.SerializerSettings.Converters.Add(new StringEnumConverter
                {
                    NamingStrategy = new CamelCaseNamingStrategy()
                });
            });

        return services;
    }

    public static IServiceCollection RegisterTelemetry(this IServiceCollection services)
    {
        services.AddOpenTelemetry()
            .WithMetrics(builder =>
            {
                builder.AddPrometheusExporter();

                builder.AddMeter("Microsoft.AspNetCore.Hosting",
                    "Microsoft.AspNetCore.Server.Kestrel");
                builder.AddView("http.server.request.duration",
                    new ExplicitBucketHistogramConfiguration
                    {
                        Boundaries =
                        [
                            0, 0.005, 0.01, 0.025, 0.05,
                            0.075, 0.1, 0.25, 0.5, 0.75, 1, 2.5, 5, 7.5, 10
                        ]
                    });
            })
            .WithTracing(builder =>
            {
                builder
                    .AddGrpcClientInstrumentation()
                    .AddAspNetCoreInstrumentation()
                    .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService("CustomerBFF"))
                    .AddSource("CustomerBFF")
                    .AddJaegerExporter();
            });

        return services;
    }

    public static IServiceCollection RegisterSerilog(this IServiceCollection services)
    {
        Log.Logger = new LoggerConfiguration()
            .WriteTo.Console(theme: AnsiConsoleTheme.Sixteen)
            .WriteTo.MongoDBBson(Configuration.MongoConnectionString,
                "logs",
                LogEventLevel.Verbose,
                50,
                TimeSpan.FromSeconds(10))
            .CreateLogger();
        services.AddSerilog();

        return services;
    }

    public static IServiceCollection RegisterExceptionHandlerMiddleware(this IServiceCollection services)
    {
        services.AddTransient<ExceptionHandlerMiddleware>();

        return services;
    }

    static ServiceCollectionExtensions()
    {
        var env = Environment.GetEnvironmentVariables();

        var identityCfg =
            new ChannelConfiguration(GetFromEnvOrThrow("IDENTITY_URL"), GetFromEnvOrThrow("IDENTITY_API_KEY"));
        var vehicleDocumentsCfg =
            new ChannelConfiguration(GetFromEnvOrThrow("VEHICLEDOCS_URL"), GetFromEnvOrThrow("VEHICLEDOCS_API_KEY"));
        var vehicleCheckCfg =
            new ChannelConfiguration(GetFromEnvOrThrow("VEHICLECHECK_URL"), GetFromEnvOrThrow("VEHICLECHECK_API_KEY"));
        var drivingLicenseCfg =
            new ChannelConfiguration(GetFromEnvOrThrow("DRIVINGLICENSE_URL"),
                GetFromEnvOrThrow("DRIVINGLICENSE_API_KEY"));
        var rentCfg =
            new ChannelConfiguration(GetFromEnvOrThrow("RENT_URL"), GetFromEnvOrThrow("RENT_API_KEY"));
        var vehicleFleetCfg =
            new ChannelConfiguration(GetFromEnvOrThrow("VEHICLEFLEET_URL"), GetFromEnvOrThrow("VEHICLEFLEET_API_KEY"));
        var mongoConnectionString = GetFromEnvOrThrow("MONGO_CONNECTION_STRING");

        Configuration = new GatewayConfiguration(
            identityCfg,
            vehicleDocumentsCfg,
            vehicleCheckCfg,
            drivingLicenseCfg,
            rentCfg,
            vehicleFleetCfg,
            mongoConnectionString);

        string GetFromEnvOrThrow(string variable)
        {
            var value = env[variable];
            if (value == null) throw new ArgumentException($"'{variable}' isn't set");

            return (string)value;
        }
    }

    private record GatewayConfiguration(
        ChannelConfiguration Identity,
        ChannelConfiguration VehicleDocuments,
        ChannelConfiguration VehicleCheck,
        ChannelConfiguration DrivingLicense,
        ChannelConfiguration Rent,
        ChannelConfiguration VehicleFleet,
        string MongoConnectionString
    );

    private record ChannelConfiguration(string Uri, string ApiKey);
}