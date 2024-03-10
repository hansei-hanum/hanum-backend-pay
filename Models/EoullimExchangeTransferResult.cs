namespace Hanum.Pay.Models;

/// <summary>
/// 한세어울림한마당 환전충전 결과
/// </summary>
public class EoullimExchangeTransferResult {
    /// <summary>
    /// 시스템 총 환전 금액
    /// </summary>
    public ulong TotalExchangeAmount { get; set; }
    /// <summary>
    /// 트랜잭션정보
    /// </summary>
    public required EoullimTransactionResult Transaction { get; set; }
}