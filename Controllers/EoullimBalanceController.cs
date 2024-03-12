using System.Security.Claims;

using Microsoft.AspNetCore.Mvc;

using Hanum.Core.Authentication;
using Hanum.Pay.Models.DTO.Requests;
using Hanum.Pay.Models.DTO.Responses;
using Hanum.Pay.Contracts.Services;
using Hanum.Core.Helpers;
using System.ComponentModel.DataAnnotations;
using Hanum.Pay.Exceptions;

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
public class EoullimBalanceController(IEoullimBalanceService balanceService) : ControllerBase {
    /// <summary>
    /// 한세어울림한마당 사용자잔액조회
    /// </summary>
    /// <returns>사용자잔액조회응답</returns>
    [HttpGet("amount")]
    public async Task<APIResponse<EoullimBalanceAmountResponse>> GetBalanceAmount() {
        var userId = this.GetHanumUserClaim();

        return APIResponse<EoullimBalanceAmountResponse>.FromData(new() {
            BalanceAmount = await balanceService.GetBalanceAmountAsync(userId)
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
        [FromQuery] int page = 1, [FromQuery, Range(1, 100)] int limit = 20) {
        var userId = this.GetHanumUserClaim();
        var payments = await balanceService.GetPaymentsDetailAsync(userId, page, limit);

        return APIResponse<EoullimUserPaymentDetailResponse>.FromData(new() {
            Page = page,
            Limit = limit,
            Total = payments.Total,
            BalanceAmount = await balanceService.GetBalanceAmountAsync(userId),
            Payments = payments.Items
        });
    }

    /// <summary>
    /// 한세어울림한마당 결제요청
    /// </summary>
    /// <param name="paymentRequest">결제요청</param>
    /// <returns>결제응답</returns>
    [HttpPost("payment")]
    public async Task<APIResponse<EoullimPaymentResponse>> PostPayment([FromBody] EoullimPaymentRequest paymentRequest) {
        try {
            return APIResponse<EoullimPaymentResponse>.FromData(
                await balanceService.PaymentAsync(ulong.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value), paymentRequest));
        } catch (DbTransctionException ex) {
            return APIResponse<EoullimPaymentResponse>.FromError(ex.StatusCode);
        }
    }
}
