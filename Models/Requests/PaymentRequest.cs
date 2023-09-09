
using System.ComponentModel.DataAnnotations;

namespace HanumPay.Models.Requests;

/// <summary>
/// 결제 DTO (개인 -> 비즈니스)
/// </summary>
public class PaymentRequest {
    /// <summary>
    /// 비즈니스 잔고 ID
    /// </summary>
    [Required]
    [Range(1, ulong.MaxValue)]
    public ulong BusinessId { get; set; }

    /// <summary>
    /// 결제 금액
    /// </summary>
    [Required]
    [Range(1, ulong.MaxValue)]
    public ulong Amount { get; set; }
}