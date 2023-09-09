using Auth;
using HanumPay.Contexts;
using HanumPay.Core;
using Microsoft.AspNetCore.Authentication;
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
builder.Services.AddDbContext<HanumContext>();

// gRPC Client
builder.Services.AddGrpcClient<AuthService.AuthServiceClient>(options => {
    options.Address = new(builder.Configuration.GetConnectionString("AuthService.gRPC")!);
});

// Authentication Handler
builder.Services.AddAuthentication("HanumAuth")
    .AddScheme<AuthenticationSchemeOptions, HanumAuthenticationHandler>("HanumAuth", null);

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
