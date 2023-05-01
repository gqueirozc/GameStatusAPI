using GameStatusAPI.Database;
using GameStatusAPI.Factories;
using GameStatusAPI.Helpers;
using GameStatusAPI.Interfaces;
using GameStatusAPI.Services;
using Microsoft.OpenApi.Models;
using Microsoft.Practices.EnterpriseLibrary.Data.Configuration;
using MongoDB.Driver;

var builder = WebApplication.CreateBuilder(args);
const string allowOrigins = "_allowAllOrigins";

// Add services to the container.
builder.Services.AddSingleton<IMongoClient, MongoClient>(s =>
{
    var connectionString = builder.Configuration["ConnectionStrings:MongoUri"];
    return new MongoClient(connectionString);
});

//Application
builder.Services.AddHttpClient<IRegularHttpClient, RegularHttpClient>();
builder.Services.AddSingleton<IUserStatusService, UserStatusService>();

//Infrastructure
builder.Services.AddSingleton<IBaseRepository, BaseRepository>();
builder.Services.AddTransient<IMongoClient, MongoClient>(_ => new MongoClient
    (builder.Configuration.GetConnectionString("MongoUri")));
builder.Services.AddSingleton<IMongoFinder, MongoFinder>();
builder.Services.AddSingleton<IUserStatusRepository, UserStatusRepository>();

//JwtConfig
builder.Services.AddControllers();
builder.Services.Configure<DatabaseSettings>(
    builder.Configuration.GetSection(nameof(MongoDatabaseSettings))
);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "apiagenda", Version = "v1" });
});


//CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy(allowOrigins, builder =>
    {
        builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
    });
});

var app = builder.Build();
app.UseCors(allowOrigins);

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
