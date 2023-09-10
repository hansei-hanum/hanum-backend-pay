namespace HanumPay.Models.Results;

/// <summary>
/// 한세어울림한마당 환불 결과
/// </summary>
public class EoullimRefundResult {
    /// <summary>
    /// 결제고유번호
    /// </summary>
    public ulong Id { get; set; }
    /// <summary>
    /// 사용자고유번호
    /// </summary>
    public ulong UserId { get; set; }
    /// <summary>
    /// 부스고유번호
    /// </summary>
    public ulong BoothId { get; set; }
    /// <summary>
    /// 결제금액
    /// </summary>
    public ulong PaidAmount { get; set; }
    /// <summary>
    /// 환불금액
    /// </summary>
    public ulong RefundedAmount { get; set; }
    /// <summary>
    /// 트랜잭션정보
    /// </summary>
    public required EoullimTransactionResult Transaction { get; set; }
}
