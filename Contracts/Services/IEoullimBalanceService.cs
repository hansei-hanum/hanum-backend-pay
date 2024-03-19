using Hanum.Core.Models;
using Hanum.Pay.Exceptions;
using Hanum.Pay.Models.DTO.Requests;
using Hanum.Pay.Models.DTO.Responses;

namespace Hanum.Pay.Contracts.Services;

public interface IEoullimBalanceService {
    /// <summary>
    /// 한세어울림한마당사용자잔고충전
    /// </summary>
    /// <param name="transferRequest">환전요청</param>
    /// <returns>환전응답</returns>
    /// <exception cref="DbTransctionException"></exception>
    public Task<EoullimExchangeTransferResponse> ExchangeTransferAsync(EoullimExchangeTransferRequest transferRequest);
    /// <summary>
    /// 한세어울림한마당 사용자잔액조회
    /// </summary>
    /// <returns>사용자잔액조회응답</returns>
    public Task<ulong> GetBalanceAmountAsync(ulong userId);
    /// <summary>
    /// 한세어울림한마당 사용자잔액상세조회
    /// </summary>
    /// <param name="userId">사용자아이디</param>
    /// <param name="page">페이지</param>
    /// <param name="limit">페이지당 항목수</param>
    /// <returns>사용자잔액상세조회응답</returns>
    public Task<DbOffsetBasedPaginationResult<EoullimUserPayment>> GetPaymentsDetailAsync(ulong userId, int page = 1, int limit = 20);
    /// <summary>
    /// 한세어울림한마당 결제요청
    /// </summary>
    /// <param name="userId">사용자아이디</param>
    /// <param name="paymentRequest">결제요청</param>
    /// <returns>결제응답</returns>
    public Task<EoullimPaymentResponse> PaymentAsync(ulong userId, EoullimPaymentRequest paymentRequest);
}
