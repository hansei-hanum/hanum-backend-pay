using HanumPay.Core.Authentication;
using HanumPay.Models.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HanumPay.Controllers;

public partial class EoullinBoothController : ControllerBase {
    /// <summary>
    /// 한세어울림한마당부스정보조회
    /// </summary>
    /// <param name="boothId">부스고유번호</param>
    /// <returns>부스정보조회응답</returns>
    [Authorize(AuthenticationSchemes = HanumAuthenticationHandler.SchemeName)]
    [HttpGet("{boothId}")]
    public async Task<APIResponse<EoullimBoothInfoResponse>> GetBoothInfo([FromRoute] ulong boothId) {
        var boothInfo = await _context.EoullimBooths.FindAsync(boothId);

        if (boothInfo is null) {
            _logger.LogWarning("부스정보조회실패: 부스정보가 존재하지 않음 [부스: {BoothId}]", boothId);

            return APIResponse<EoullimBoothInfoResponse>.FromError("BOOTH_NOT_FOUND");
        }

        return APIResponse<EoullimBoothInfoResponse>.FromData(
            new() {
                Id = boothInfo.Id,
                Name = boothInfo.Name,
            }
        );
    }

    /// <summary>
    /// 한세어울림한마당부스상세정보조회
    /// </summary>
    /// <param name="boothId">부스고유번호</param>
    /// <returns>부스상세정보조회응답</returns>
    [Authorize(AuthenticationSchemes = HanumAuthenticationHandler.SchemeName)]
    [HttpGet("{boothId}/detail")]
    public async Task<APIResponse<EoullimBoothDetailResponse>> GetBoothDetail([FromRoute] ulong boothId) {
        var boothInfo = await _context.EoullimBooths.FindAsync(boothId);

        if (boothInfo is null) {
            _logger.LogWarning("부스정보조회실패: 부스정보가 존재하지 않음 [부스: {BoothId}]", boothId);

            return APIResponse<EoullimBoothDetailResponse>.FromError("BOOTH_NOT_FOUND");
        }

        return APIResponse<EoullimBoothDetailResponse>.FromData(
            new() {
                Id = boothInfo.Id,
                Name = boothInfo.Name,
                Classification = boothInfo.Class,
                Location = boothInfo.Location,
            }
        );
    }
}
