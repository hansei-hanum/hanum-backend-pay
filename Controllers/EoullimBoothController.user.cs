using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using Hanum.Core.Authentication;
using Hanum.Core.Models;
using Hanum.Pay.Models.DTO.Responses;
using Hanum.Core.Models.DTO.Responses;

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
        var boothInfo = await boothService.GetBoothInfoAsync(boothId);

        return boothInfo is not null ?
            APIResponse<EoullimBoothInfo>.FromData(boothInfo) :
            APIResponse<EoullimBoothInfo>.FromError(HanumStatusCode.BoothNotFound);
    }

    /// <summary>
    /// 한세어울림한마당부스상세정보조회
    /// </summary>
    /// <param name="boothId">부스고유번호</param>
    /// <returns>부스상세정보조회응답</returns>
    [HttpGet("{boothId}/detail")]
    [Authorize(AuthenticationSchemes = HanumAuthenticationHandler.SchemeName)]
    public async Task<APIResponse<EoullimBoothDetailResponse>> GetBoothDetail([FromRoute] ulong boothId) {
        var boothDetail = await boothService.GetBoothDetailAsync(boothId);

        return boothDetail is not null ?
            APIResponse<EoullimBoothDetailResponse>.FromData(boothDetail) :
            APIResponse<EoullimBoothDetailResponse>.FromError(HanumStatusCode.BoothNotFound);
    }
}
