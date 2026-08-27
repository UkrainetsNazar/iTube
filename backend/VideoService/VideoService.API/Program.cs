using Elastic.Clients.Elasticsearch;
using Microsoft.EntityFrameworkCore;
using VideoService.Application.Extensions;
using VideoService.Infrastructure;
using VideoService.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

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

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();