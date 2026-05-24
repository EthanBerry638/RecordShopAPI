using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using RecordShop.Api.Data;
using RecordShop.Api.Health;
using RecordShop.Api.Middleware;
using RecordShop.Api.Repositories;
using RecordShop.Api.Services;

var builder = WebApplication.CreateBuilder(args);

if (builder.Environment.IsDevelopment())
{
    var connectionString = builder.Configuration.GetConnectionString("SqliteConnection");

    var keepAliveConnection = new SqliteConnection(connectionString);
    keepAliveConnection.Open();

    builder.Services.AddDbContext<RecordShopContext>(options =>
             options.UseSqlite(keepAliveConnection));
}
else
{
    builder.Configuration.AddUserSecrets<Program>();
    var connectionString = builder.Configuration.GetConnectionString("SqlServerConnection");
    
    builder.Services.AddDbContext<RecordShopContext>(options =>
             options.UseSqlServer(connectionString));
}

builder.Services.AddHealthChecks()
        .AddCheck<DatabaseHealthCheck>("Database_Check");

builder.Services.AddScoped<IAlbumRepository, AlbumRepository>();
builder.Services.AddScoped<IAlbumService, AlbumService>();
builder.Services.AddScoped<IArtistService, ArtistService>();
builder.Services.AddScoped<IArtistRepository, ArtistRepository>();

builder.Services.AddTransient<ExceptionMiddleware>();
builder.Services.AddTransient<LoggingMiddleware>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddControllers(options =>
{
    options.SuppressAsyncSuffixInActionNames = false;
});

var app = builder.Build();

app.UseMiddleware<ExceptionMiddleware>();
app.UseMiddleware<LoggingMiddleware>();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<RecordShopContext>();

    if (app.Environment.IsProduction())
    {
        dbContext.Database.Migrate();
    }
    else
    {
        dbContext.Database.EnsureCreated();
    }

    dbContext.SeedData();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapControllers();

app.MapHealthChecks("/health");

app.Run();

public partial class Program { }