namespace HanumPay.Models;

/// <summary>
/// 잔고 트랜잭션 결과
/// </summary>
public class TransactionResult : StoredProcedureResult {
    /// <summary>
    /// 트랜잭션 ID
    /// </summary>
    public ulong Id { get; set; }
    /// <summary>
    /// 트랜잭션 시각
    /// </summary>
    public DateTime Time { get; set; }
    /// <summary>
    /// 송금자 ID (null일 경우 송금자가 환전소임.)
    /// </summary>
    public ulong? SenderId { get; set; }
    /// <summary>
    /// 송금자 잔고 (null일 경우 송금자가 환전소임.)
    /// </summary>
    public ulong? SenderAmount { get; set; }
    /// <summary>
    /// 수신자 ID
    /// </summary>
    public ulong ReceiverId { get; set; }
    /// <summary>
    /// 수신자 잔고
    /// </summary>
    public ulong ReceiverAmount { get; set; }
    /// <summary>
    /// 송금 금액
    /// </summary>
    public ulong TransferAmount { get; set; }
}


/// <summary>
/// 환전 트랜잭션 결과
/// </summary>
public class ExchangeResult : TransactionResult {
    /// <summary>
    /// 누적환전금
    /// </summary>
    public ulong TotalExchangeAmount { get; set; }
}
