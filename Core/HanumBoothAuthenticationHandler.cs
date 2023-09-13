using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using HanumPay.Contexts;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.EntityFrameworkCore;

namespace HanumPay.Core;

public class HanumBoothAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions> {
    private readonly bool _bypassAuth;
    private readonly HanumContext _context;
    private readonly IDistributedCache _cache;

    public HanumBoothAuthenticationHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        ISystemClock clock,
        HanumContext context,
        IDistributedCache cache,
        IConfiguration configuration
    ) : base(options, logger, encoder, clock) {
        _context = context;
        _cache = cache;
        _bypassAuth = configuration.GetValue<bool>("BypassAuth");
    }

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync() {
        var token = Request.Headers.Authorization.ToString().Split(" ");

        if (token.Length != 2 || token[0] != "Bearer")
            return AuthenticateResult.Fail("Token is missing");

        string? boothId = !_bypassAuth ? await _cache.GetStringAsync($"booth:{token[1]}") : token[1];


        if (_bypassAuth) {
            if (!ulong.TryParse(boothId, out _))
                return AuthenticateResult.Fail("Token is invalid");
        } else if (boothId is null) {
            var boothInfo = await _context.EoullimBooths.FirstOrDefaultAsync(booth => booth.Token == token[1]);

            if (boothInfo is null)
                return AuthenticateResult.Fail("Token is invalid");

            await _cache.SetStringAsync($"booth:{token[1]}", boothInfo.Id.ToString(), new DistributedCacheEntryOptions {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30)
            });

            boothId = boothInfo.Id.ToString();
        }

        return AuthenticateResult.Success(new(
            new(
                new ClaimsIdentity(
                    new Claim[] {
                        new(ClaimTypes.NameIdentifier, boothId)
                    },
                    Scheme.Name
                )
            ),
            Scheme.Name
        ));
    }
}
