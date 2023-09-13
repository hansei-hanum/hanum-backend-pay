using System.Security.Claims;
using HanumPay.Contexts;
using HanumPay.Models.Requests;
using HanumPay.Models.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models.Responses;

namespace HanumPay.Controllers;

/// <summary>
/// 한세어울림한마당 부스
/// </summary>
[Authorize(AuthenticationSchemes = "HanumBoothAuth")]
[ApiController]
[Route("eoullim/booth")]
public partial class EoullinBoothController : ControllerBase {
    readonly ILogger<EoullinBoothController> _logger;
    readonly HanumContext _context;

    /// <summary>
    /// 한세어울림한마당 부스 생성자
    /// </summary>
    public EoullinBoothController(ILogger<EoullinBoothController> logger, HanumContext context) {
        _logger = logger;
        _context = context;
    }

    /// <summary>
    /// 한세어울림한마당부스잔고조회
    /// </summary>
    /// <param name="page">페이지</param>
    /// <param name="limit">페이지당 항목수</param>
    /// <returns></returns>
    [HttpGet("payment/detail")]
    public async Task<APIResponse<EoullimBoothPaymentDetailResponse>> GetPaymentDetail(
        [FromQuery] int page = 1,
        [FromQuery] int limit = 20
    ) {
        var boothId = ulong.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        return APIResponse<EoullimBoothPaymentDetailResponse>.FromData(new() {
            Page = page,
            Limit = limit,
            Total = await _context.EoullimPayments.CountAsync(p => p.BoothId == boothId),
            BalanceAmount = await (
                    from b in _context.EoullimBalances
                    where b.BoothId == boothId
                    select b.Amount
                ).FirstOrDefaultAsync(),
            Payments = await (
                    from p in _context.EoullimPayments
                    where p.BoothId == boothId
                    orderby p.Id descending
                    select new EoullimBoothPayment {
                        Id = p.Id,
                        UserId = p.UserId,
                        UserName = p.User.Name,
                        BoothId = p.BoothId,
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
    /// 한세어울림한마당환불요청
    /// </summary>
    /// <param name="refundRequest">환불요청</param>
    /// <returns>환불응답</returns>
    [HttpPost("payment/refund")]
    public async Task<APIResponse<EoullimBoothRefundResponse>> PostRefundHistory(
        [FromBody] EoullimBoothRefundRequest refundRequest) {
        var boothId = ulong.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var paymentInfo = await (
            from p in _context.EoullimPayments
            where p.Id == refundRequest.PaymentId && p.BoothId == boothId
            select new {
                p.UserId,
                p.PaidAmount,
            }
        ).FirstOrDefaultAsync();

        if (paymentInfo == null)
            return APIResponse<EoullimBoothRefundResponse>.FromError("PAYMENT_NOT_FOUND");

        var refundResult = await _context.EoullimPaymentCancel(
            paymentId: refundRequest.PaymentId,
            message: refundRequest.Message
        );

        if (!refundResult.Success) {
            _logger.LogWarning("환불실패: {ErrorMessage} [결제: {PaymentId}, 부스: {BoothId}, 사용자: {UserId}, 금액: {Amount}]",
                refundResult.ErrorMessage ?? "Unknown", refundRequest.PaymentId, boothId, paymentInfo.UserId, paymentInfo.PaidAmount);

            return APIResponse<EoullimBoothRefundResponse>.FromError(refundResult.ErrorCode ?? "UNKNOWN_ERROR");
        }

        var transaction = refundResult.Data.Transaction;

        return APIResponse<EoullimBoothRefundResponse>.FromData(new() {
            PaymentId = refundResult.Data.Id,
            UserId = refundResult.Data.UserId,
            BoothId = refundResult.Data.BoothId,
            PaidAmount = refundResult.Data.PaidAmount,
            RefundedAmount = refundResult.Data.RefundedAmount,
            BalanceAmount = transaction.ReceiverAmount,
            Transaction = new() {
                Id = transaction.Id,
                SenderId = transaction.SenderId,
                ReceiverId = transaction.ReceiverId,
                TransferAmount = transaction.TransferAmount,
                Message = transaction.Message,
                Time = transaction.Time
            }
        });
    }
}
