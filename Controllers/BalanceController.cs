using System.Security.Claims;
using HanumPay.Contexts;
using HanumPay.Models.Requests;
using HanumPay.Models.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;
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
            _logger.LogWarning("결제실패: {ErrorMessage} [{UserId} -> {BusinessId}, 결제금액: {Amount}]",
                paymentResult.ErrorMessage ?? "Unknown", userId, paymentRequest.BusinessId, paymentRequest.Amount);

            return APIResponse<PaymentResponse>.FromError(paymentResult.ErrorCode ?? "UNKNOWN_ERROR");
        }

        _logger.LogInformation("결제성공: [{UserId} -> {BusinessId}, 결제금액: {Amount}, 잔액: {BalanceAmount}]",
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
}
