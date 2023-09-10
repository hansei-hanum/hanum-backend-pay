using System.Security.Claims;
using HanumPay.Contexts;
using HanumPay.Models.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models.Responses;

namespace HanumPay.Controllers;

[Authorize(AuthenticationSchemes = "HanumBoothAuth")]
[ApiController]
[Route("booth")]
public partial class EoullinBoothController : ControllerBase {
    readonly ILogger<EoullinBoothController> _logger;
    readonly HanumContext _context;

    public EoullinBoothController(ILogger<EoullinBoothController> logger, HanumContext context) {
        _logger = logger;
        _context = context;
    }

    [HttpGet("payment/history")]
    public async Task<APIResponse<EoullimBoothPaymentHistoryResponse>> GetPaymentHistory(
        [FromQuery] int page = 1,
        [FromQuery] int limit = 20
    ) {
        var boothId = ulong.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        return APIResponse<EoullimBoothPaymentHistoryResponse>.FromData(new() {
            Page = page,
            Limit = limit,
            Total = await _context.EoullimPayments.CountAsync(p => p.BoothId == boothId),
            Payments = (await _context.EoullimPayments
                .Where(p => p.BoothId == boothId)
                .OrderByDescending(p => p.Id)
                .Skip((page - 1) * limit)
                .Take(limit)
                .ToListAsync()).Select(p => new EoullimBoothPayment() {
                    Id = p.Id,
                    UserId = p.UserId,
                    BoothId = p.BoothId,
                    PaidAmount = p.PaidAmount,
                    RefundedAmount = p.RefundedAmount,
                    PaymentComment = p.PaymentComment,
                    RefundComment = p.RefundComment,
                    Status = p.Status,
                    PaidTime = p.PaidTime,
                    RefundedTime = p.RefundedTime
                }).ToList()
        });
    }
}
