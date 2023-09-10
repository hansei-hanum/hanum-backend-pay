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
[Route("eoullim/balances")]
public class EoullimPaymentController : ControllerBase {
    readonly ILogger<EoullimPaymentController> _logger;
    readonly HanumContext _context;

    public EoullimPaymentController(ILogger<EoullimPaymentController> logger, HanumContext context) {
        _logger = logger;
        _context = context;
    }

    [HttpPost("payment")]
    public async Task<APIResponse<EoullimPaymentResponse>> PostPayment([FromBody] EoullimPaymentRequest paymentRequest) {
        var userId = ulong.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var paymentResult = await _context.EoullimPayment(
            userId: userId,
            boothId: paymentRequest.BoothId,
            transferAmount: paymentRequest.Amount
        );

        if (!paymentResult.Success) {
            _logger.LogWarning("결제실패: {ErrorMessage} [결제자: {UserId}, 부스: {BoothId}, 금액: {Amount}]",
                paymentResult.ErrorMessage ?? "Unknown", userId, paymentRequest.BoothId, paymentRequest.Amount);

            return APIResponse<EoullimPaymentResponse>.FromError(paymentResult.ErrorCode ?? "UNKNOWN_ERROR");
        }

        var transaction = paymentResult.Data.Transaction;

        _logger.LogInformation("결제성공: [결제자: {UserId}, 부스: {BoothId}, 금액: {Amount}, 잔액: {BalanceAmount}]",
            userId, paymentRequest.BoothId, paymentRequest.Amount, transaction.SenderAmount);

        return APIResponse<EoullimPaymentResponse>.FromData(
            new() {
                PaymentId = paymentResult.Data.Id,
                UserId = paymentResult.Data.UserId,
                BoothId = paymentResult.Data.BoothId,
                BalanceAmount = transaction.SenderAmount!.Value,
                Transaction = new() {
                    Id = transaction.Id,
                    Time = transaction.Time,
                    SenderId = transaction.SenderId,
                    ReceiverId = transaction.ReceiverId,
                    TransferAmount = transaction.TransferAmount,
                    Message = transaction.Message,
                }
            }
        );
    }
}
