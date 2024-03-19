using System.Security.Claims;

using Microsoft.AspNetCore.Mvc;

using Hanum.Core.Authentication;
using Hanum.Core.Models;
using Hanum.Pay.Contracts.Services;
using Hanum.Pay.Exceptions;
using Hanum.Pay.Models.DTO.Requests;
using Hanum.Pay.Models.DTO.Responses;
using Hanum.Core.Models.DTO.Responses;

namespace Hanum.Pay.Controllers;

/// <summary>
/// 한세어울림한마당환전소
/// </summary>
/// <remarks>
/// 한세어울림한마당환전소 생성자
/// </remarks>
[ApiController]
[Route("eoullim/exchange")]
[HanumCommomAuthorize]
public class EoullimExchangeController(
    ILogger<EoullimExchangeController> logger,
    IEoullimBalanceService balanceService,
    IConfiguration configuration) : ControllerBase {
    readonly HashSet<ulong> _allowedUsers = new(configuration.GetSection("AllowedUsers")
            .GetSection("Exchange").Get<ulong[]>()!);

    /// <summary>
    /// 한세어울림한마당사용자잔고충전
    /// </summary>
    /// <param name="transferRequest">환전요청</param>
    /// <returns>환전응답</returns>
    [HttpPost("transfer")]
    public async Task<APIResponse<EoullimExchangeTransferResponse>> ExchangeTransfer([FromBody] EoullimExchangeTransferRequest transferRequest) {
        var userId = ulong.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        if (!_allowedUsers.Contains(userId)) {
            logger.LogWarning("허용되지 않은 사용자가 환전을 시도했습니다. [{UserId}]", userId);
            Response.StatusCode = 403;
            return APIResponse<EoullimExchangeTransferResponse>.FromError(HanumStatusCode.NotAllowed);
        }

        try {
            return APIResponse<EoullimExchangeTransferResponse>.FromData(
                await balanceService.ExchangeTransferAsync(transferRequest));
        } catch (DbTransctionException ex) {
            return APIResponse<EoullimExchangeTransferResponse>.FromError(ex.StatusCode);
        }
    }
}
