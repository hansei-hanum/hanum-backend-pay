using System.Security.Claims;

using Microsoft.AspNetCore.Mvc;

using Hanum.Core.Authentication;
using Hanum.Pay.Contexts;
using Hanum.Pay.Models.Requests;
using Hanum.Pay.Models.Responses;

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
    HanumContext context,
    IConfiguration configuration) : ControllerBase {
    readonly HashSet<ulong> _allowedUsers = new(configuration.GetSection("AllowedUsers")
            .GetSection("Exchange").Get<ulong[]>()!);

    /// <summary>
    /// 한세어울림한마당사용자잔고충전
    /// </summary>
    /// <param name="transferRequest">환전요청</param>
    /// <returns>환전응답</returns>
    [HttpPost("transfer")]
    public async Task<APIResponse<EoullimExchangeTransferResponse>> PostTransfer([FromBody] EoullimExchangeTransferRequest transferRequest) {
        var userId = ulong.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        if (!_allowedUsers.Contains(userId)) {
            logger.LogWarning("허용되지 않은 사용자가 환전을 시도했습니다. [{UserId}]", userId);
            Response.StatusCode = 403;
            return APIResponse<EoullimExchangeTransferResponse>.FromError("NOT_ALLOWED");
        }

        var exchangeResult = await context.EoullimPersonalBalanceCharge(
            userId: transferRequest.UserId,
            transferAmount: transferRequest.Amount,
            message: transferRequest.Message
        );

        if (!exchangeResult.Success) {
            logger.LogWarning("충전실패: {ErrorMessage} [사용자: {UserId}, 충전금액: {Amount}]",
                exchangeResult.ErrorMessage ?? "Unknown", transferRequest.UserId, transferRequest.Amount);

            return APIResponse<EoullimExchangeTransferResponse>.FromError(exchangeResult.ErrorCode ?? "UNKNOWN_ERROR");
        }

        var transaction = exchangeResult.Data.Transaction;

        logger.LogInformation("충전성공: [사용자: {UserId}, 충전금액: {Amount}, 잔액: {BalanceAmount}, 누적환전금: {TotalExchangeAmount}]",
            transferRequest.UserId, transferRequest.Amount, transaction.ReceiverAmount, exchangeResult.Data.TotalExchangeAmount);

        return APIResponse<EoullimExchangeTransferResponse>.FromData(
            new() {
                TotalExchangeAmount = exchangeResult.Data.TotalExchangeAmount,
                Transaction = new() {
                    Id = transaction.Id,
                    SenderId = transaction.SenderId,
                    ReceiverId = transaction.ReceiverId,
                    TransferAmount = transaction.TransferAmount,
                    Time = transaction.Time,
                    SenderAmount = transaction.SenderAmount,
                    ReceiverAmount = transaction.ReceiverAmount,
                    Message = transaction.Message,
                }
            }
        );
    }
}
