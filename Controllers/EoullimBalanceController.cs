using System.Security.Claims;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using Hanum.Core.Authentication;
using Hanum.Pay.Contexts;
using Hanum.Pay.Models.DTO.Requests;
using Hanum.Pay.Models.DTO.Responses;

namespace Hanum.Pay.Controllers;

/// <summary>
/// 한세어울림한마당 사용자잔고
/// </summary>
/// <remarks>
/// 한세어울림한마당 사용자잔고 생성자
/// </remarks>
[ApiController]
[Route("eoullim/balance")]
[HanumCommomAuthorize]
public class EoullimPaymentController(ILogger<EoullimPaymentController> logger, HanumContext context) : ControllerBase {
    /// <summary>
    /// 한세어울림한마당 사용자잔액조회
    /// </summary>
    /// <returns>사용자잔액조회응답</returns>
    [HttpGet("amount")]
    public async Task<APIResponse<EoullimBalanceAmountResponse>> GetBalanceAmount() {
        var userId = ulong.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        return APIResponse<EoullimBalanceAmountResponse>.FromData(new() {
            BalanceAmount = await (
                    from b in context.EoullimBalances
                    where b.UserId == userId
                    select b.Amount
                ).FirstOrDefaultAsync()
        });
    }

    /// <summary>
    /// 한세어울림한마당 사용자잔액상세조회
    /// </summary>
    /// <param name="page">페이지</param>
    /// <param name="limit">페이지당 항목수</param>
    /// <returns>사용자잔액상세조회응답</returns>
    [HttpGet("detail")]
    public async Task<APIResponse<EoullimUserPaymentDetailResponse>> GetBalanceDetail(
        [FromQuery] int page = 1,
        [FromQuery] int limit = 20
    ) {
        var userId = ulong.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        return APIResponse<EoullimUserPaymentDetailResponse>.FromData(new() {
            Page = page,
            Limit = limit,
            Total = await context.EoullimPayments.CountAsync(p => p.UserId == userId),
            BalanceAmount = await (
                    from b in context.EoullimBalances
                    where b.UserId == userId
                    select b.Amount
                ).FirstOrDefaultAsync(),
            Payments = await (
                    from p in context.EoullimPayments
                    where p.UserId == userId
                    orderby p.Id descending
                    select new EoullimUserPayment {
                        Id = p.Id,
                        UserId = p.UserId,
                        BoothId = p.BoothId,
                        BoothName = p.Booth.Name,
                        PaidAmount = p.PaidAmount,
                        RefundedAmount = p.RefundedAmount,
                        PaymentComment = p.PaymentComment,
                        RefundComment = p.RefundComment,
                        Status = p.Status,
                        PaidTime = p.PaidTime,
                        RefundedTime = p.RefundedTime
                    }
                )
                .Skip((page - 1) * limit)
                .Take(limit)
                .ToListAsync()
        });
    }

    /// <summary>
    /// 한세어울림한마당 결제요청
    /// </summary>
    /// <param name="paymentRequest">결제요청</param>
    /// <returns>결제응답</returns>
    [HttpPost("payment")]
    public async Task<APIResponse<EoullimPaymentResponse>> PostPayment([FromBody] EoullimPaymentRequest paymentRequest) {
        var userId = ulong.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var paymentResult = await context.EoullimPayment(
            userId: userId,
            boothId: paymentRequest.BoothId,
            transferAmount: paymentRequest.Amount
        );

        if (!paymentResult.Success) {
            logger.LogWarning("결제실패: {ErrorMessage} [결제자: {UserId}, 부스: {BoothId}, 금액: {Amount}]",
                paymentResult.ErrorMessage ?? "Unknown", userId, paymentRequest.BoothId, paymentRequest.Amount);

            Response.StatusCode = 400;

            return APIResponse<EoullimPaymentResponse>.FromError(paymentResult.StatusCode);
        }

        var transaction = paymentResult.Data.Transaction;

        logger.LogInformation("결제성공: [결제자: {UserId}, 부스: {BoothId}, 금액: {Amount}, 잔액: {BalanceAmount}]",
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
