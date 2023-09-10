namespace HanumPay.Models.Results;

/// <summary>
/// 한세어울림한마당 결제 결과
/// </summary>
public class EoullimPaymentResult {
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
    /// 트랜잭션정보
    /// </summary>
    public required EoullimTransactionResult Transaction { get; set; }
}
