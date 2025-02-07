namespace Hanum.Pay.Models;

/// <summary>
/// 한세어울림한마당 트랜잭션 결과
/// </summary>
public class EoullimTransactionResult {
    /// <summary>
    /// 트랜잭션고유번호
    /// </summary>
    public ulong Id { get; set; }
    /// <summary>
    /// 송금자고유번호, 환전소의 경우 NULL로 설정
    /// </summary>
    public ulong? SenderId { get; set; }
    /// <summary>
    /// 수신자고유번호
    /// </summary>
    public ulong ReceiverId { get; set; }
    /// <summary>
    /// 이체후송금자잔액, 환전소의 경우 NULL로 설정
    /// </summary>
    public ulong? SenderAmount { get; set; }
    /// <summary>
    /// 이체후수신자잔액
    /// </summary>
    public ulong ReceiverAmount { get; set; }
    /// <summary>
    /// 이체금액
    /// </summary>
    public ulong TransferAmount { get; set; }
    /// <summary>
    /// 송금메시지
    /// </summary>
    public string? Message { get; set; }
    /// <summary>
    /// 트랜잭션시간
    /// </summary>
    public DateTime Time { get; set; }
}
