using HanumPay.Contexts;
using HanumPay.Models.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HanumPay.Controllers;

[Authorize(AuthenticationSchemes = "HanumBoothAuth")]
[ApiController]
[Route("booths")]
public class EoullinBoothController : ControllerBase {
    readonly ILogger<EoullinBoothController> _logger;
    readonly HanumContext _context;

    public EoullinBoothController(ILogger<EoullinBoothController> logger, HanumContext context) {
        _logger = logger;
        _context = context;
    }

    [Authorize(AuthenticationSchemes = "HanumAuth")]
    [HttpGet("{boothId}")]
    public async Task<APIResponse<EoullimBoothInfoResponse>> GetBoothInfo([FromRoute] ulong boothId) {
        var boothInfo = await _context.EoullimBooths.FindAsync(boothId);

        if (boothInfo is null) {
            _logger.LogWarning("부스정보조회실패: 부스정보가 존재하지 않음 [부스: {BoothId}]", boothId);

            return APIResponse<EoullimBoothInfoResponse>.FromError("BOOTH_NOT_FOUND");
        }

        _logger.LogInformation("부스정보조회성공: [부스: {BoothId}, 이름: {BoothName}]", boothId, boothInfo.Name);

        return APIResponse<EoullimBoothInfoResponse>.FromData(
            new() {
                Id = boothInfo.Id,
                Name = boothInfo.Name,
            }
        );
    }
}
