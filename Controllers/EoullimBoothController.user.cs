using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using Hanum.Core.Authentication;
using Hanum.Core.Models;
using Hanum.Pay.Models.DTO.Responses;

namespace Hanum.Pay.Controllers;

public partial class EoullinBoothController : ControllerBase {
    /// <summary>
    /// 한세어울림한마당부스정보조회
    /// </summary>
    /// <param name="boothId">부스고유번호</param>
    /// <returns>부스정보조회응답</returns>
    [HttpGet("{boothId}")]
    [HanumCommomAuthorize]
    public async Task<APIResponse<EoullimBoothInfo>> GetBoothInfo([FromRoute] ulong boothId) {
        var boothInfo = await context.EoullimBooths.FindAsync(boothId);

        if (boothInfo is null) {
            logger.LogWarning("부스정보조회실패: 부스정보가 존재하지 않음 [부스: {BoothId}]", boothId);

            return APIResponse<EoullimBoothInfo>.FromError(HanumStatusCode.BoothNotFound);
        }

        return APIResponse<EoullimBoothInfo>.FromData(
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
    [HttpGet("{boothId}/detail")]
    [Authorize(AuthenticationSchemes = HanumAuthenticationHandler.SchemeName)]
    public async Task<APIResponse<EoullimBoothDetailResponse>> GetBoothDetail([FromRoute] ulong boothId) {
        var boothInfo = await context.EoullimBooths.FindAsync(boothId);

        if (boothInfo is null) {
            logger.LogWarning("부스정보조회실패: 부스정보가 존재하지 않음 [부스: {BoothId}]", boothId);

            return APIResponse<EoullimBoothDetailResponse>.FromError(HanumStatusCode.BoothNotFound);
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
