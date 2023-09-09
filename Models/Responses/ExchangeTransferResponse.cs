namespace Models.Responses;

/// <summary>
/// 환전 송금 응답 DTO
/// </summary>
public class ExchangeTransferResponse {
    /// <summary>
    /// 시스템 총 환전 금액
    /// </summary>
    public ulong TotalExchangeAmount { get; set; }
    /// <summary>
    /// 결제 트랜잭션 정보
    /// </summary>
    public required TransactionInfo Transaction { get; set; }
}