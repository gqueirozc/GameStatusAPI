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

builder.Services.AddSingleton<IMongoClient, MongoClient>(s =>
{
    var connectionString = builder.Configuration["ConnectionStrings:MongoUri"];
    return new MongoClient(connectionString);
});

builder.Services.AddHttpClient<IRegularHttpClient, RegularHttpClient>();
builder.Services.AddSingleton<IUserStatusService, UserStatusService>();

builder.Services.AddSingleton<IBaseRepository, BaseRepository>();
builder.Services.AddTransient<IMongoClient, MongoClient>(_ => new MongoClient(builder.Configuration.GetConnectionString("MongoUri")));
builder.Services.AddSingleton<IMongoFinder, MongoFinder>();
builder.Services.AddSingleton<IUserStatusRepository, UserStatusRepository>();

builder.Services.AddControllers();
builder.Services.Configure<DatabaseSettings>(
    builder.Configuration.GetSection(nameof(MongoDatabaseSettings))
);

builder.Services.AddControllers().AddNewtonsoftJson();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "GameStatusAPI", Version = "v1" });
});

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

    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "GameStatusAPI V1");
    });
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();