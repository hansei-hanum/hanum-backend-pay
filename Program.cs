
using Microsoft.EntityFrameworkCore;

using Hanum.Core.Helpers;
using Hanum.Core.Protos;
using Hanum.Pay.Contexts;
using Hanum.Pay.Services;
using Hanum.Core.Authentication;
using Hanum.Core.Middleware;
using Hanum.Pay.Core.Authentication;
using Microsoft.AspNetCore.Authentication;
using Hanum.Pay.Contracts.Services;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;
var services = builder.Services;

builder.WebHost.UseStaticWebAssets();

services.AddHanumLogging();
services.AddCors(options => {
    options.AddPolicy(
        name: "AllowAll",
        policyBuilder => {
            policyBuilder.AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader();
        }
    );
    options.AddPolicy(
        name: "AllowCors",
        policyBuilder => {
            policyBuilder.WithOrigins(configuration.GetSection("AllowedCorsOrigins").Get<string[]>()!)
                .AllowAnyMethod()
                .AllowAnyHeader();
        }
    );
});

services.AddRazorPages();
services.AddServerSideBlazor();

services.AddControllers();
services.AddHanumSwaggerGen();

// DB Context
services.AddHanumDbContexts<HanumContext>(
    configuration.GetConnectionString("Database.SQL"),
    configuration.GetSection("Database"));

// Redis Cache
services.AddStackExchangeRedisCache(options => {
    options.Configuration = configuration.GetConnectionString("Cache.Redis");
});

// gRPC Client
services.AddHanumAuthGrpcClient(
    configuration.GetConnectionString("AuthService.gRPC")!);

// Authentication Handler
services.AddAuthentication()
    .AddHanumCommonAuthScheme()
    .AddScheme<AuthenticationSchemeOptions, HanumBoothAuthenticationHandler>(HanumBoothAuthenticationHandler.SchemeName, null);

// Services
services.AddTransient<IEoullimBoothService, EoullimBoothService>();
services.AddTransient<IEoullimBalanceService, EoullimBalanceService>();
services.AddSingleton<IEoullimDashboardService, EoullimDashboardService>();

var app = builder.Build();

using (var serviceScope = app.Services.GetService<IServiceScopeFactory>()!.CreateScope()) {
    var context = serviceScope.ServiceProvider.GetRequiredService<HanumContext>();

    foreach (var sqls in Directory.GetFiles("Migrations/hanum", "*.sql").OrderBy(x => x)) {
        var sql = File.ReadAllText(sqls);

        try {
            context.Database.ExecuteSqlRaw(sql);
        } catch {
            if (sqls.Contains('!'))
                throw;
        }
    }
}

if (app.Environment.IsDevelopment()) {
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseHsts();
    app.UseCors("AllowAll");
} else {
    app.UseExceptionHandler("/Error");
    app.UseCors("AllowCors");
}

app.UseMiddleware<RequestLoggingMiddleware>();

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.MapBlazorHub();

app.MapFallbackToPage("/_Host");


app.Run();
