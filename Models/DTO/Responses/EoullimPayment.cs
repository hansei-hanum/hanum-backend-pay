

namespace Hanum.Pay.Models.DTO.Responses;

/// <summary>
/// 한세어울림한마당 결제내역
/// </summary>
public partial class EoullimPayment {
    /// <summary>
    /// 결제고유변호
    /// </summary>
    public ulong Id { get; set; }
    /// <summary>
    /// 결제자고유번호
    /// </summary>
    public ulong UserId { get; set; }
    /// <summary>
    /// 결제대상
    /// </summary>
    public ulong BoothId { get; set; }
    /// <summary>
    /// 결제금액
    /// </summary>
    public ulong PaidAmount { get; set; }
    /// <summary>
    /// 결제취소금액
    /// </summary>
    public ulong? RefundedAmount { get; set; }
    /// <summary>
    /// 결제메시지
    /// </summary>
    public string? PaymentComment { get; set; }
    /// <summary>
    /// 환불메시지
    /// </summary>
    public string? RefundComment { get; set; }
    /// <summary>
    /// 결제상태
    /// </summary>
    public string Status { get; set; } = null!;
    /// <summary>
    /// 결제시간
    /// </summary>
    public DateTime PaidTime { get; set; }
    /// <summary>
    /// 결제취소시간
    /// </summary>
    public DateTime? RefundedTime { get; set; }
}
