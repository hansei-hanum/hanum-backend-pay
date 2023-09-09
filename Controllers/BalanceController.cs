using System.Security.Claims;
using HanumPay.Contexts;
using HanumPay.Models.Requests;
using HanumPay.Models.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models.Responses;

namespace HanumPay.Controllers;

[Authorize(AuthenticationSchemes = "HanumAuth")]
[ApiController]
[Route("balance")]
public class PaymentController : ControllerBase {
    readonly ILogger<PaymentController> _logger;
    readonly HanumContext _context;

    public PaymentController(ILogger<PaymentController> logger, HanumContext context) {
        _logger = logger;
        _context = context;
    }

    [HttpPost("payment")]
    public async Task<APIResponse<PaymentResponse>> PostPayment([FromBody] PaymentRequest paymentRequest) {
        var userId = ulong.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var paymentResult = await _context.PersonalPayment(
           personalUserId: userId,
           businessBalanceId: paymentRequest.BusinessId,
           transferAmount: paymentRequest.Amount
        );

        if (!paymentResult.Success) {
            _logger.LogWarning("결제실패: {ErrorMessage} [송금자: {UserId}, 수신자: {BusinessId}, 결제금액: {Amount}]",
                paymentResult.ErrorMessage ?? "Unknown", userId, paymentRequest.BusinessId, paymentRequest.Amount);

            return APIResponse<PaymentResponse>.FromError(paymentResult.ErrorCode ?? "UNKNOWN_ERROR");
        }

        _logger.LogInformation("결제성공: [송금자: {UserId}, 수신자: {BusinessId}, 결제금액: {Amount}, 잔액: {BalanceAmount}]",
            userId, paymentRequest.BusinessId, paymentRequest.Amount, paymentResult.SenderAmount);

        return APIResponse<PaymentResponse>.FromData(
            new() {
                BalanceAmount = paymentResult.SenderAmount!.Value,
                Transaction = new() {
                    Id = paymentResult.Id,
                    SenderId = paymentResult.SenderId,
                    ReceiverId = paymentResult.ReceiverId,
                    TransferAmount = paymentResult.TransferAmount,
                    Time = paymentResult.Time
                }
            }
        );
    }

    [HttpGet("info/{balanceId}")]
    public async Task<APIResponse<BalanceInfoResponse>> GetBalanceInfo([FromRoute] ulong balanceId) {
        var userId = ulong.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var balanceInfo = await _context.Balances.FindAsync(balanceId);

        if (balanceInfo == null) {
            _logger.LogWarning("잔고정보조회실패: 잔고를 찾을 수 없음 [잔고ID: {BalanceId}, 요청자ID: {UserId}]", balanceId, userId);
            return APIResponse<BalanceInfoResponse>.FromError("BALANCE_NOT_FOUND");
        }

        _logger.LogInformation("잔고정보조회성공: [잔고ID: {BalanceId}, 요청자ID: {UserId}, 잔고이름: {BalanceName}, 잔고타입: {BalanceType}]",
            balanceId, userId, balanceInfo.Type, balanceInfo.Type);

        return APIResponse<BalanceInfoResponse>.FromData(
            new() {
                Id = balanceInfo.Id,
                Label = balanceInfo.Label,
                Type = balanceInfo.Type
            }
        );
    }
}
