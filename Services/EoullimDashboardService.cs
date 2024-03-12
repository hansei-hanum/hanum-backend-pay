
using Hanum.Pay.Contexts;
using Hanum.Pay.Contracts.Services;
using Hanum.Pay.Models.DTO.Responses;
using Microsoft.EntityFrameworkCore;

namespace Hanum.Pay.Services;

public class EoullimDashboardService(IDbContextFactory<HanumContext> factory) : IEoullimDashboardService {
    public async Task<EoullimBoothRankResponse> GetBoothRankingAsync(int page = 1, int limit = 10, bool descending = true) {
        await using var context = factory.CreateDbContext();
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