using Hanum.Pay.Contexts;
using Hanum.Pay.Models.DTO.Responses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Hanum.Pay.Services;

public class EoullimBoothService(IDbContextFactory<HanumContext> factory) : ControllerBase {
    /// <summary>
    /// 부스잔액순위조회
    /// </summary>
    /// <param name="page">페이지번호</param>
    /// <param name="limit">항목수</param>
    /// <param name="descending">정렬방식</param>
    /// <returns>조회결과</returns>
    public async Task<EoullimBoothRankResponse> GetBoothRankingAsync(int page = 1, int limit = 10, bool descending = true) {
        using var context = factory.CreateDbContext();
        return new EoullimBoothRankResponse {
            Ranks = (await context.EoullimBooths
                    .Where(booth => booth.EoullimBalance != null)
                    .OrderByDescending(booth => booth.EoullimBalance!.Amount)
                    .Skip((page - 1) * limit)
                    .Take(limit)
                    .Select(b => new {
                        b.Id,
                        b.Name,
                        b.EoullimBalance!.Amount
                    })
                    .ToListAsync()
                )
                .Select((booth, index) => new EoullimBoothRankItem {
                    Id = booth.Id,
                    Rank = index + 1,
                    Name = booth.Name,
                    Amount = booth.Amount
                })
                .ToList(),
            Descending = descending,
            Page = page,
            Limit = limit,
            Total = await context.EoullimBooths.CountAsync(booth => booth.EoullimBalance != null)
        };
    }
}
