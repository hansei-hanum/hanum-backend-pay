namespace Models.Responses;

/// <summary>
/// 한세어울림한마당 환전충전 트랜잭션정보
/// </summary>
public class EoullimExchangeTransferResponse {
    /// <summary>
    /// 시스템 총 환전 금액
    /// </summary>
    public ulong TotalExchangeAmount { get; set; }
    /// <summary>
    /// 결제 트랜잭션 정보
    /// </summary>
    public required EoullimTransactionDetail Transaction { get; set; }
}