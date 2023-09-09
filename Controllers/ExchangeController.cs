using System.Security.Claims;
using HanumPay.Contexts;
using HanumPay.Models.Requests;
using HanumPay.Models.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.Responses;

namespace HanumPay.Controllers;

[Authorize(AuthenticationSchemes = "HanumAuth")]
[ApiController]
[Route("exchange")]
public class ExchangeController : ControllerBase {
    readonly ILogger<ExchangeController> _logger;
    readonly HanumContext _context;
    readonly HashSet<ulong> _allowedUsers;

    public ExchangeController(
        ILogger<ExchangeController> logger,
        HanumContext context,
        IConfiguration configuration
    ) {
        _logger = logger;
        _context = context;
        _allowedUsers = new(configuration.GetSection("AllowedUsers")
            .GetSection("Exchange").Get<ulong[]>()!);
    }

    [HttpPost("transfer")]
    public async Task<APIResponse<ExchangeTransferResponse>> PostTransfer([FromBody] ExchangeTransferRequest transferRequest) {
        var userId = ulong.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        if (!_allowedUsers.Contains(userId)) {
            _logger.LogWarning("허용되지 않은 사용자가 환전을 시도했습니다. [{UserId}]", userId);
            Response.StatusCode = 403;
            return APIResponse<ExchangeTransferResponse>.FromError("NOT_ALLOWED");
        }
        
        var exchangeResult = await _context.PersonalExchange(
            personalUserId: transferRequest.UserId,
            transferAmount: transferRequest.Amount
        );
                
        if (!exchangeResult.Success) {
            _logger.LogWarning("충전실패: {ErrorMessage} [사용자: {UserId}, 충전금액: {Amount}]",
                exchangeResult.ErrorMessage ?? "Unknown", transferRequest.UserId, transferRequest.Amount);

            return APIResponse<ExchangeTransferResponse>.FromError(exchangeResult.ErrorCode ?? "UNKNOWN_ERROR");
        }

        _logger.LogInformation("충전성공: [사용자: {UserId}, 충전금액: {Amount}, 잔액: {BalanceAmount}, 누적환전금: {TotalExchangeAmount}]",
            transferRequest.UserId, transferRequest.Amount, exchangeResult.ReceiverAmount, exchangeResult.TotalExchangeAmount);

        return APIResponse<ExchangeTransferResponse>.FromData(
            new() {
                TotalExchangeAmount = exchangeResult.TotalExchangeAmount,
                Transaction = new() {
                    Id = exchangeResult.Id,
                    SenderId = exchangeResult.SenderId,
                    ReceiverId = exchangeResult.ReceiverId,
                    TransferAmount = exchangeResult.TransferAmount,
                    Time = exchangeResult.Time
                }
            }
        );
    }
}
