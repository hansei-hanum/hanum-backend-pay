
using Hanum.Pay.Models.DTO.Responses;

namespace Hanum.Pay.Contracts.Services;

public interface IEoullimDashboardService {
    /// <summary>
    /// 부스 잔액 순위 조회
    /// </summary>
    /// <param name="page">페이지 번호</param>
    /// <param name="limit">항목 수</param>
    /// <param name="descending">정렬 방식</param>
    /// <returns>조회 결과</returns>
    public Task<EoullimBoothRankResponse> GetBoothRankingAsync(int page = 1, int limit = 10, bool descending = true);
}
