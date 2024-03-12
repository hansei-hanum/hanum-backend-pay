
using Hanum.Pay.Exceptions;
using Hanum.Pay.Models.DTO.Requests;
using Hanum.Pay.Models.DTO.Responses;

namespace Hanum.Pay.Contracts.Services;

public interface IEoullimBoothService {
    /// <summary>
    /// 부스 잔고 조회
    /// </summary>
    /// <param name="boothId">부스 고유번호</param>
    /// <param name="page">페이지</param>
    /// <param name="limit">페이지 당 항목 수</param>
    public Task<DbOffsetBasedPagenationResult<EoullimBoothPayment>> GetPaymentDetailAsync(ulong boothId, int page = 1, int limit = 20);
    /// <summary>
    /// 환불
    /// </summary>
    /// <param name="boothId">부스 고유번호</param>
    /// <param name="refundRequest">환불 요청</param>
    /// <returns>환불 응답</returns>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="DbTransctionException"></exception>
    public Task<EoullimBoothRefundResponse> RefundAsync(ulong boothId, EoullimBoothRefundRequest refundRequest);
    /// <summary>
    /// 부스 잔고 조회
    /// </summary>
    /// <param name="boothId">부스 고유번호</param>
    /// <returns>부스 잔고</returns>
    public Task<ulong> GetBalanceAmountAsync(ulong boothId);
    /// <summary>
    /// 부스 정보 조회
    /// </summary>
    /// <param name="boothId">부스 고유번호</param>
    /// <returns>부스 정보</returns>
    public Task<EoullimBoothInfo?> GetBoothInfoAsync(ulong boothId);
    /// <summary>
    /// 부스 상세 정보 조회
    /// </summary>
    /// <param name="boothId">부스 고유번호</param>
    /// <returns>부스 상세 정보</returns>
    public Task<EoullimBoothDetailResponse?> GetBoothDetailAsync(ulong boothId);
}
