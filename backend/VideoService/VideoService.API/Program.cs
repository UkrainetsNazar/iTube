using Elastic.Clients.Elasticsearch;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Shared.Api.Extensions;
using VideoService.Application.Extensions;
using VideoService.Infrastructure;
using VideoService.Infrastructure.Persistence;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

try
{
    Log.Information("Starting VideoService");

    var builder = WebApplication.CreateBuilder(args);

    builder.Host.UseSerilog((context, services, configuration) => configuration
        .ReadFrom.Configuration(context.Configuration)
        .ReadFrom.Services(services)
        .Enrich.FromLogContext()
        .Enrich.WithProperty("Service", "VideoService")
        .WriteTo.Console(outputTemplate:
            "[{Timestamp:HH:mm:ss} {Level:u3}] {Service} {Message:lj}{NewLine}{Exception}"));

    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    builder.Services.AddApplication();
    builder.Services.AddInfrastructure(builder.Configuration);

    var app = builder.Build();

    using (var scope = app.Services.CreateScope())
    {
        var db = scope.ServiceProvider.GetRequiredService<VideoDbContext>();
        db.Database.Migrate();

        var esClient = scope.ServiceProvider.GetRequiredService<ElasticsearchClient>();
        await VideoService.Infrastructure.Search.ElasticIndexInitializer.EnsureIndexAsync(esClient);
    }

    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseSerilogRequestLogging();
    app.UseGlobalExceptionHandling();

    app.UseHttpsRedirection();
    app.UseAuthentication();
    app.UseAuthorization();
    app.MapControllers();

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "VideoService terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}