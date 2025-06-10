using Gateway.Middlewares;

namespace Gateway;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        ConfigureServices(builder);

        var app = builder.Build();

        ConfigureAppForDeveloperEnvironment(app);
        ConfigureApp(app);

        app.Run();
    }

    private static void ConfigureServices(WebApplicationBuilder builder)
    {
        var services = builder.Services;

        // services
        //     .RegisterExceptionHandlerMiddleware()
        //     .RegisterGrpcClientsAndConfigure()
        //     .RegisterGrpcClientWrappers()
        //     .RegisterInterceptors()
        //     .RegisterControllersWithNewtonsoft()
        //     .RegisterSwagger()
        //     .RegisterSerilog()
        //     .RegisterTelemetry();
    }

    private static void ConfigureAppForDeveloperEnvironment(WebApplication app)
    {
        if (app.Environment.IsDevelopment() == false) return;

        app
            .UseDeveloperExceptionPage()
            .UseSwagger()
            .UseSwaggerUI();
    }

    private static void ConfigureApp(WebApplication app)
    {
        // app.UseHttpsRedirection();
        app.UseMiddleware<ExceptionHandlerMiddleware>();
        app.UseAuthorization();
        app.MapControllers();
    }
}