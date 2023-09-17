namespace HanumPay.Models.Responses;

/// <summary>
/// 한세어울림한마당 부스환불내역
/// </summary>
public class EoullimBoothRefundResponse {
    /// <summary>
    /// 결제고유번호
    /// </summary>
    public ulong PaymentId { get; set; }
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
    /// 결제후부스잔액
    /// </summary>
    public ulong BalanceAmount { get; set; }
    /// <summary>
    /// 트랜잭션정보
    /// </summary>
    public required EoullimTransactionInfo Transaction { get; set; }
}
