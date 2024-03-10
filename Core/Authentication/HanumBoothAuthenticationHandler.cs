using System.Security.Claims;
using System.Text.Encodings.Web;

using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;

using Hanum.Pay.Contexts;

namespace Hanum.Pay.Core.Authentication;

public class HanumBoothAuthenticationHandler(
    IOptionsMonitor<AuthenticationSchemeOptions> options,
    ILoggerFactory logger,
    UrlEncoder encoder,
    HanumContext context,
    IDistributedCache cache,
    IConfiguration configuration
    ) : AuthenticationHandler<AuthenticationSchemeOptions>(options, logger, encoder) {
    public const string SchemeName = "HanumBoothAuth";
    public const string CookieName = "EoullimBoothToken";

    private readonly bool _bypassAuth = configuration.GetValue<bool>("Hanum:BypassAuth");

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync() {
        var tokenString = Request.Headers.Authorization.ToString();

        if (string.IsNullOrEmpty(tokenString))
            tokenString = Request.Cookies[CookieName] ?? string.Empty;

        var token = tokenString.Split(" ");

        if (token.Length != 2 || token[0] != "Bearer")
            return AuthenticateResult.Fail("Token is missing");

        string? boothId = !_bypassAuth ? await cache.GetStringAsync($"booth:{token[1]}") : token[1];

        if (_bypassAuth) {
            if (!ulong.TryParse(boothId, out _))
                return AuthenticateResult.Fail("Token is invalid");
        } else if (boothId is null) {
            var boothInfo = await context.EoullimBooths.FirstOrDefaultAsync(booth => booth.Token == token[1]);

            if (boothInfo is null)
                return AuthenticateResult.Fail("Token is invalid");

            await cache.SetStringAsync($"booth:{token[1]}", boothInfo.Id.ToString(), new DistributedCacheEntryOptions {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30)
            });

            boothId = boothInfo.Id.ToString();
        }

        return AuthenticateResult.Success(new(
            new(
                new ClaimsIdentity(
                    [
                        new(ClaimTypes.NameIdentifier, boothId)
                    ],
                    Scheme.Name
                )
            ),
            Scheme.Name
        ));
    }
}
