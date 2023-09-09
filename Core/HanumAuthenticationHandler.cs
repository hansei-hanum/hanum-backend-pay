using Auth;
using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

namespace HanumPay.Core;

public class HanumAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions> {
    private readonly AuthService.AuthServiceClient _authServiceClient;

    public HanumAuthenticationHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        ISystemClock clock,
        AuthService.AuthServiceClient authServiceClient
    ) : base(options, logger, encoder, clock) {
        _authServiceClient = authServiceClient;
    }

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync() {
        var token = Request.Headers.Authorization.ToString().Split(" ");

        if (token.Length != 2 || token[0] != "Bearer")
            return AuthenticateResult.Fail("Token is missing");

        var response = await _authServiceClient.AuthorizeAsync(new AuthorizeRequest { Token = token[1] });

        if (!response.Success || !response.HasUserid)
            return AuthenticateResult.Fail("Token is invalid");

        return AuthenticateResult.Success(new(
            new(
                new ClaimsIdentity(
                    new Claim[] {
                        new(ClaimTypes.NameIdentifier, response.Userid.ToString())
                    },
                    Scheme.Name
                )
            ),
            Scheme.Name
        ));
    }
}
