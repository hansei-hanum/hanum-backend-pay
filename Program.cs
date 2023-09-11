using Auth;
using HanumPay.Contexts;
using HanumPay.Core;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Console;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddLogging(logging =>
    logging.AddSimpleConsole(options => {
        options.SingleLine = true;
        options.TimestampFormat = "HH:mm:ss ";
        options.ColorBehavior = LoggerColorBehavior.Enabled;
    })
);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// DB Context
var databaseConfig = builder.Configuration.GetSection("Database");

builder.Services.AddDbContextPool<HanumContext>(options => {
    var connectionString = builder.Configuration.GetConnectionString("Database.SQL");
    options.UseMySql(
        connectionString,
        ServerVersion.AutoDetect(connectionString),
        options => options.EnableRetryOnFailure(
            maxRetryCount: databaseConfig.GetValue<int>("MaxRetryCount"),
            maxRetryDelay: TimeSpan.FromSeconds(databaseConfig.GetValue<int>("MaxRetryDelay")),
            errorNumbersToAdd: null
        )
    );
}, databaseConfig.GetValue<int>("MaxPoolSize"));
// Redis Cache
builder.Services.AddStackExchangeRedisCache(options => {
    options.Configuration = builder.Configuration.GetConnectionString("Cache.Redis");
});

// gRPC Client
builder.Services.AddGrpcClient<AuthService.AuthServiceClient>(options => {
    options.Address = new(builder.Configuration.GetConnectionString("AuthService.gRPC")!);
});

// Authentication Handler
builder.Services.AddAuthentication()
    .AddScheme<AuthenticationSchemeOptions, HanumAuthenticationHandler>("HanumAuth", null)
    .AddScheme<AuthenticationSchemeOptions, HanumBoothAuthenticationHandler>("HanumBoothAuth", null);

var app = builder.Build();

if (app.Environment.IsDevelopment()) {
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<RequestLoggingMiddleware>();
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();

public partial class Program { }