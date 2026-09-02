using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using Shared.Api.Extensions;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

try
{
    Log.Information("Starting ApiGateway");

    var builder = WebApplication.CreateBuilder(args);

    var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>()
    ?? ["http://localhost:5020"];

    builder.Host.UseSerilog((context, services, configuration) => configuration
        .ReadFrom.Configuration(context.Configuration)
        .ReadFrom.Services(services)
        .Enrich.FromLogContext()
        .Enrich.WithProperty("Service", "ApiGateway")
        .WriteTo.Console(outputTemplate:
            "[{Timestamp:HH:mm:ss} {Level:u3}] {Service} {Message:lj}{NewLine}{Exception}"));

    builder.Configuration.AddJsonFile("yarp.json", optional: false, reloadOnChange: true);

    builder.Services.AddReverseProxy()
        .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

    builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = builder.Configuration["Jwt:Issuer"],
                ValidateAudience = true,
                ValidAudience = builder.Configuration["Jwt:Audience"],
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                    builder.Configuration["Jwt:Secret"] ?? throw new InvalidOperationException("Jwt:Secret is not configured."))),
                ValidateLifetime = true
            };
        });

    builder.Services.AddAuthorization();

    builder.WebHost.ConfigureKestrel(options =>
    {
        options.Limits.MaxRequestBodySize = 2_000_000_000;
    });

    builder.Services.AddRateLimiter(options =>
    {
        options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

        options.AddFixedWindowLimiter("AuthPolicy", opt =>
        {
            opt.Window = TimeSpan.FromMinutes(1);
            opt.PermitLimit = 5;
            opt.QueueLimit = 0;
        });

        options.AddFixedWindowLimiter("UploadPolicy", opt =>
        {
            opt.Window = TimeSpan.FromMinutes(1);
            opt.PermitLimit = 10;
            opt.QueueLimit = 0;
        });
    });

    builder.Services.AddCors(options =>
    {
        options.AddPolicy("Frontend", policy =>
        {
            policy.WithOrigins(allowedOrigins)
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials();
        });
    });

    var app = builder.Build();

    app.UseSerilogRequestLogging();
    app.UseGlobalExceptionHandling();

    app.UseCors("Frontend");

    app.UseAuthentication();
    app.UseAuthorization();
    app.UseRateLimiter();

    app.MapReverseProxy(proxyPipeline =>
    {
        proxyPipeline.Use(async (context, next) =>
        {
            if (context.User.Identity?.IsAuthenticated == true)
            {
                var userId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var role = context.User.FindFirst(ClaimTypes.Role)?.Value;

                if (userId is not null)
                    context.Request.Headers["X-User-Id"] = userId;
                if (role is not null)
                    context.Request.Headers["X-User-Role"] = role;
            }

            await next();
        });
    });

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "ApiGateway terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}