
namespace Hanum.Pay.Models.Responses;

/// <summary>
/// 한세어울림한마당 트랜잭션정보
/// </summary>
public class EoullimPaymentResponse {
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
    /// 결제후잔액
    /// </summary>
    public ulong BalanceAmount { get; set; }
    /// <summary>
    /// 트랜잭션정보
    /// </summary>
    public EoullimTransactionInfo? Transaction { get; set; }
}