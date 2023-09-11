using Auth;
using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

namespace HanumPay.Core;

public class HanumAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions> {
    private readonly bool _bypassAuth;
    private readonly AuthService.AuthServiceClient _authServiceClient;

    public HanumAuthenticationHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        ISystemClock clock,
        IConfiguration configuration,
        AuthService.AuthServiceClient authServiceClient
    ) : base(options, logger, encoder, clock) {
        _authServiceClient = authServiceClient;
        _bypassAuth = configuration.GetValue<bool>("BypassAuth");
    }

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync() {
        var token = Request.Headers.Authorization.ToString().Split(" ");

        if (token.Length != 2 || token[0] != "Bearer")
            return AuthenticateResult.Fail("Token is missing");

        string? userId = null;

        if (!_bypassAuth) {
            var response = await _authServiceClient.AuthorizeAsync(new AuthorizeRequest { Token = token[1] });

            if (response.Success || response.HasUserid)
                userId = response.Userid.ToString();
        } else {
            userId = token[1];

            if (!ulong.TryParse(userId, out var _))
                userId = null;
        }

        if (string.IsNullOrEmpty(userId))
            return AuthenticateResult.Fail("Token is invalid");

        return AuthenticateResult.Success(new(
            new(
                new ClaimsIdentity(
                    new Claim[] {
                        new(ClaimTypes.NameIdentifier, userId)
                    },
                    Scheme.Name
                )
            ),
            Scheme.Name
        ));
    }
}
