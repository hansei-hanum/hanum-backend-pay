using System.Security.Claims;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Hanum.Pay.Contexts;
using Hanum.Pay.Core.Authentication;
using Hanum.Pay.Models.DTO.Requests;
using Hanum.Pay.Models.DTO.Responses;
using Hanum.Pay.Contracts.Services;
using Hanum.Pay.Exceptions;
using Hanum.Core.Models;
using System.ComponentModel.DataAnnotations;

namespace Hanum.Pay.Controllers;

/// <summary>
/// 한세어울림한마당 부스
/// </summary>
/// <remarks>
/// 한세어울림한마당 부스 생성자
/// </remarks>
[ApiController]
[Route("eoullim/booth")]
public partial class EoullinBoothController(IEoullimBoothService boothService) : ControllerBase {
    /// <summary>
    /// 한세어울림한마당부스잔고조회
    /// </summary>
    /// <param name="page">페이지</param>
    /// <param name="limit">페이지당 항목수</param>
    /// <returns></returns>
    [HttpGet("payment/detail")]
    [Authorize(AuthenticationSchemes = HanumBoothAuthenticationHandler.SchemeName)]
    public async Task<APIResponse<EoullimBoothPaymentDetailResponse>> GetPaymentDetail([FromQuery] int page = 1, [FromQuery, Range(1, 100)] int limit = 20) {
        var boothId = ulong.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var result = await boothService.GetPaymentDetailAsync(boothId, page, limit);

        return APIResponse<EoullimBoothPaymentDetailResponse>.FromData(new() {
            Page = page,
            Limit = limit,
            Total = result.Total,
            Payments = result.Items,
            BoothInfo = (await boothService.GetBoothDetailAsync(boothId))!,
            BalanceAmount = await boothService.GetBalanceAmountAsync(boothId)
        });
    }

    /// <summary>
    /// 한세어울림한마당환불요청
    /// </summary>
    /// <param name="refundRequest">환불요청</param>
    /// <returns>환불응답</returns>
    [HttpPost("payment/refund")]
    [Authorize(AuthenticationSchemes = HanumBoothAuthenticationHandler.SchemeName)]
    public async Task<APIResponse<EoullimBoothRefundResponse>> PostRefund(
        [FromBody] EoullimBoothRefundRequest refundRequest) {
        try {
            return APIResponse<EoullimBoothRefundResponse>.FromData(
                await boothService.RefundAsync(ulong.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value), refundRequest));
        } catch (ArgumentException ex) when (ex.ParamName == "refundRequest") {
            return APIResponse<EoullimBoothRefundResponse>.FromError(HanumStatusCode.PaymentNotFound);
        } catch (DbTransctionException ex) {
            return APIResponse<EoullimBoothRefundResponse>.FromError(ex.StatusCode);
        }
    }
}
